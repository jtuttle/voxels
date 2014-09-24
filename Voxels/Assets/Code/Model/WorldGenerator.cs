using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Fractal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator {
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Elevations { get; private set; }
    public int Seed { get; private set; }

    public float[] RawSamples;
    public float[] DiscreteSamples;

    public WorldGenerator(int width, int height, int elevations, int seed) {
        Width = width;
        Height = height;
        Elevations = elevations;
        Seed = seed;

        RawSamples = GenerateWorldNoise(Width, Height, elevations);
        DiscreteSamples = DiscretizeWorldNoise(RawSamples, elevations);
    }

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

    private float[] DiscretizeWorldNoise(float[] samples, int elevations) {
        float[] output = new float[samples.Length];

        List<float> bounds = new List<float>();

        float unit = 1.0f / elevations;

        for(int i = 0; i <= elevations + 1; i++)
            bounds.Add(i * unit);

        for(int i = 0; i < samples.Length; i++) {
            //output[i] = Mathf.Floor(samples[i]);
            int boundIndex = bounds.FindIndex(x => x > samples[i]);
            output[i] = bounds[boundIndex - 1];
        }

        return output;
    }
}
