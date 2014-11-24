using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Fractal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldNoiseGenerator {
    public int Seed { get; private set; }

    public WorldNoiseGenerator(int seed) {
        Seed = seed;
    }

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

    // Return an array of noise values in the range [0,1]
    public float[,] GenerateRawNoise(int width, int height) {
        float[,] samples = new float[width, height];
        
        //Generator generator = new ValueNoise(Random.Range(1, 65536), SCurve.Cubic);
        Generator generator = new PinkNoise(Seed);

        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                float xCoord = (float)x / width;
                float yCoord = (float)y / height;
                
                float sample = generator.GetValue(xCoord, yCoord, 0);
                
                // PinkNoise usually returns value in range[-1,1] but this is not guaranteed.
                sample = Mathf.Clamp(sample, -1, 1);
                
                // Convert values from range [-1,1] to range [0,1].
                sample = MathUtils.ConvertRange(-1, 1, 0, 1, sample);
                
                samples[x, y] = sample;
                
                //Debug.Log(x + "," + y + " => " + sample);
            }
        }
        
        return samples;
    }

    public float[,] DiscretizeNormalizedNoise(float[,] samples, int elevations) {
        float[,] output = new float[samples.GetLength(0), samples.GetLength(1)];
        
        List<float> bounds = new List<float>();
        
        float unit = 1.0f / elevations;
        
        for(int i = 0; i <= elevations + 1; i++)
            bounds.Add(i * unit);

        for(int y = 0; y < samples.GetLength(1); y++) {
            for(int x = 0; x < samples.GetLength(0); x++) {
                //output[i] = Mathf.Floor(samples[i]);
                int boundIndex = bounds.FindIndex(b => b > samples[x, y]);
                output[x, y] = bounds[boundIndex - 1];
            }
        }
        
        return output;
    }

    // This will discretize noise to the nearest whole numbers, intended for use with
    // noise that has already been shifted to a non-unit range.
    public float[,] DiscretizeDenormalizedNoise(float[,] samples) {
        float[,] output = new float[samples.GetLength(0), samples.GetLength(1)];

        for(int y = 0; y < samples.GetLength(1); y++) {
            for(int x = 0; x < samples.GetLength(0); x++) {
                output[x, y] = Mathf.Floor(samples[x, y]);
            }
        }

        return output;
    }

    public float[,] ShiftNoise(float oldMin, float oldMax, float newMin, float newMax, float[,] samples) {
        float[,] output = new float[samples.GetLength(0), samples.GetLength(1)];

        for(int y = 0; y < samples.GetLength(1); y++) {
            for(int x = 0; x < samples.GetLength(0); x++) {
                output[x, y] = MathUtils.ConvertRange(oldMin, oldMax, newMin, newMax, samples[x, y]);
            }
        }

        return output;
    }

    public float[,] ApplyCubicWeight(float[,] samples) {
        float[,] output = new float[samples.GetLength(0), samples.GetLength(1)];
        
        for(int y = 0; y < samples.GetLength(1); y++) {
            //float scale = MathUtils.ConvertRange(0, 1, vMin, vMax, (float)y / samples.GetLength(1)); 

            // need to take a min of 1 and value here, the upper values are just getting scaled
            // back down to where they used to be.

            float heightRatio = (float)y / (samples.GetLength(1) - 1);

            // (2x - 1)^3 + 1
            float scale = Mathf.Pow(2 * heightRatio - 1, 3) + 1;

            //Debug.Log(y + ": " + scale);

            for(int x = 0; x < samples.GetLength(0); x++) {
                float weighted = samples[x, y] * scale;
                output[x, y] = Mathf.Min(1, weighted); //MathUtils.ConvertRange(0, 2, 0, 1, weighted);
            }
        }
        
        return output;
    }

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
