using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Fractal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is responsible for generating the initial Perlin noise we use to create 
// the world and modifying it to have discrete elevations, flatter areas, and a gradient 
// that places ocean in the south and mountains in the north. Methods are exposed for 
// debug visualization.

public class WorldNoiseGenerator {
    public int Seed { get; private set; }

    public WorldNoiseGenerator(int seed) {
        Seed = seed;
    }

    // Run the full noise generation algorithm.
    public float[,] GenerateWorldNoise(int width, int height, int elevations) {
        float[,] worldNoise = GenerateRawNoise(width, height);

        worldNoise = NormalizeAverage(worldNoise);
        worldNoise = ApplyCubicWeight(worldNoise);
        worldNoise = Blockify(worldNoise);

        // This shift is just to make sure we don't have any > 1 values.
        worldNoise = ShiftNoise(0, 1, 0, 1, worldNoise);
        worldNoise = DiscretizeNormalizedNoise(worldNoise, elevations);

        return worldNoise;
    }

    // Generates some nice-looking Perlin noise.
    public float[,] GenerateRawNoise(int width, int height) {
        float[,] samples = new float[width, height];
        
        //Generator generator = new ValueNoise(Random.Range(1, 65536), SCurve.Cubic);
        Generator generator = new PinkNoise(Seed);

        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                float xCoord = (float)x / width;
                float yCoord = (float)y / height;
                
                float sample = generator.GetValue(xCoord, yCoord, 0);

                // PinkNoise usually returns value in [-1,1] but this is not guaranteed.
                sample = Mathf.Clamp(sample, -1, 1);
                
                // Convert value from [-1,1] to [0,1].
                sample = MathUtils.ConvertRange(-1, 1, 0, 1, sample);
                
                samples[x, y] = sample;
            }
        }
        
        return samples;
    }

    // Discretize noise in the [0,1] range into the specified number of elevations.
    public float[,] DiscretizeNormalizedNoise(float[,] samples, int elevations) {
        float[,] output = new float[samples.GetLength(0), samples.GetLength(1)];

        float unit = 1.0f / elevations;

        List<float> bounds = new List<float>();

        for(int i = 0; i <= elevations; i++)
            bounds.Add(i * unit);

        for(int y = 0; y < samples.GetLength(1); y++) {
            for(int x = 0; x < samples.GetLength(0); x++) {
                // TODO: Shouldn't this is biasing our noise upwards by quite a bit?
                // I don't see how there would be many zero values left afterwards...
                int boundIndex = bounds.FindIndex(b => b >= samples[x, y]);
                // TODO: I VERY RARELY GET OUTOFBOUNDS ERROR HERE
                output[x, y] = bounds[boundIndex - 1];
            }
        }
        
        return output;
    }

    // Discretize noise to the nearest integer, intended for use with noise that
    // has already been shifted from the [0,1] range.
    // TODO: use an array if int or byte here to save space.
    public float[,] DiscretizeDenormalizedNoise(float[,] samples) {
        float[,] output = new float[samples.GetLength(0), samples.GetLength(1)];

        for(int y = 0; y < samples.GetLength(1); y++) {
            for(int x = 0; x < samples.GetLength(0); x++) {
                output[x, y] = Mathf.Floor(samples[x, y]);
            }
        }

        return output;
    }

    // Shift noise from one range of values to another.
    public float[,] ShiftNoise(float oldMin, float oldMax, float newMin, float newMax, float[,] samples) {
        float[,] output = new float[samples.GetLength(0), samples.GetLength(1)];

        for(int y = 0; y < samples.GetLength(1); y++) {
            for(int x = 0; x < samples.GetLength(0); x++) {
                output[x, y] = MathUtils.ConvertRange(oldMin, oldMax, newMin, newMax, samples[x, y]);
            }
        }

        return output;
    }

    // In order to create a world where we are much more likely to have ocean along 
    // the bottom and mountains along the top, we apply a cubic curve to weight the
    // noise in such a way that lower values (on the y-axis) are scaled down, higher
    // values are scaled up, and central values are left mostly untouched.
    public float[,] ApplyCubicWeight(float[,] samples) {
        float[,] output = new float[samples.GetLength(0), samples.GetLength(1)];
        
        for(int y = 0; y < samples.GetLength(1); y++) {
            // Figure out our vertical position in the image.
            float heightRatio = (float)y / (samples.GetLength(1) - 1);

            // Plug position into cubic equation (2x - 1)^3 + 1
            float scale = Mathf.Pow(2 * heightRatio - 1, 3) + 1;

            for(int x = 0; x < samples.GetLength(0); x++) {
                float weighted = samples[x, y] * scale;

                // Value can go above 1, so we must take min.
                output[x, y] = Mathf.Min(1, weighted);
            }
        }
        
        return output;
    }

    // Modify the noise values such that they produce the target average. This helps
    // prevent the world from having too much ocean or mountainous area.
    public float[,] NormalizeAverage(float[,] samples, float targetAverage = 0.6f) {
        float[,] output = new float[samples.GetLength(0), samples.GetLength(1)];

        int count = samples.GetLength(0) * samples.GetLength(1);
        float total = 0;

        for(int y = 0; y < samples.GetLength(1); y++) {
            for(int x = 0; x < samples.GetLength(0); x++) {
                total += samples[x, y];
            }
        }

        float average = total / count;
        float diff = targetAverage - average;

        for(int y = 0; y < samples.GetLength(1); y++) {
            for(int x = 0; x < samples.GetLength(0); x++) {
                output[x, y] = Mathf.Min(samples[x, y] + diff, 1);
            }
        }

        return output;
    }

    // Take the max of the surrounding values for each noise sample. This reduces
    // the elevation variation of the map to produce flatter areas.
    public float[,] Blockify(float[,] samples) {
        float[,] output = new float[samples.GetLength(0), samples.GetLength(1)];

        int width = samples.GetLength(0);
        int height = samples.GetLength(1);
        float max = 0;

        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                max = 0;

                if(x > 0)
                    max = Mathf.Max(max, samples[x - 1, y]);

                if(x < width - 1)
                    max = Mathf.Max(max, samples[x + 1, y]);

                if(y > 0)
                    max = Mathf.Max(max, samples[x, y - 1]);

                if(y < height - 1)
                    max = Mathf.Max(max, samples[x, y + 1]);

                output[x, y] = max;
            }
        }

        return output;
    }
}
