using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Fractal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator {
    public int Seed { get; private set; }

    public WorldGenerator(int seed) {
        Seed = seed;
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

    public float[,] WeightNoise(float vMin, float vMax, float[,] samples) {
        float[,] output = new float[samples.GetLength(0), samples.GetLength(1)];
        
        for(int y = 0; y < samples.GetLength(1); y++) {
            float scale = MathUtils.ConvertRange(0, 1, vMin, vMax, (float)y / samples.GetLength(1)); 

            Debug.Log(y + ": " + scale);

            for(int x = 0; x < samples.GetLength(0); x++) {
                float weighted = samples[x, y] * scale;
                output[x, y] = MathUtils.ConvertRange(0, 2, 0, 1, weighted);
            }
        }
        
        return output;
    }

    /*
    private float[] GenerateWorldNoise(int width, int height, int elevations) {
        float[] samples = new float[width * height];
        
        //Generator generator = new ValueNoise(Random.Range(1, 65536), SCurve.Cubic);
        Generator generator = new PinkNoise(Seed);
        
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                float xCoord = (float)x / width;
                float yCoord = (float)y / height;
                
                float sample = generator.GetValue(xCoord, yCoord, 0);

                // PinkNoise usually returns value in range[-1,1] but this is not guaranteed.
                sample = Mathf.Clamp(sample, -1, 1);
                
                // Convert values from range [-1,1] to range [0,elevations].
                // This makes discretization and chunk generation much easier.
                sample = MathUtils.ConvertRange(-1, 1, 0, 1, sample);
                
                samples[y * width + x] = sample;

                //Debug.Log(x + "," + y + " => " + sample);
            }
        }
        
        return samples;
    }


    */
}
