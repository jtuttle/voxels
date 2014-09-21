using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Fractal;
using CoherentNoise.Interpolation;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NoiseTestController : MonoBehaviour {
    public GameObject Canvas;

    public int Width;
    public int Height;
    public float Scale;

    private float[] _currentSamples;
    private Texture2D _currentTexture;

    protected void Start() {
        GenerateNoiseTest();
    }

    protected void Update() {

    }

    protected void OnGUI() {
        if(GUI.Button(new Rect(20, 40, 80, 20), "Refresh"))
            OnRefreshClick();

        if(GUI.Button(new Rect(20, 80, 80, 20), "Discritize"))
            OnDiscritizeClick();

        if(GUI.Button(new Rect(20, 120, 80, 20), "Save"))
            OnSaveClick();
    }

    private void OnRefreshClick() {
        GenerateNoiseTest();
    }

    private void OnDiscritizeClick() {
        Discritize(_currentSamples, 5);

        _currentTexture = GenerateTexture(Width, Height, _currentSamples);
        Canvas.renderer.material.mainTexture = _currentTexture;
    }

    private void OnSaveClick() {
        SaveTextureToFile(_currentTexture, "Resources/Textures/test.png");
    }

    private void GenerateNoiseTest() {
        _currentSamples = GenerateCoherentNoise(Width, Height);

        _currentTexture = GenerateTexture(Width, Height, _currentSamples);
        Canvas.renderer.material.mainTexture = _currentTexture;
    }

    private float[] GenerateCoherentNoise(int width, int height) {
        float[] samples = new float[width * height];
        
        //Generator generator = new ValueNoise(Random.Range(1, 65536), SCurve.Cubic);
        Generator generator = new PinkNoise(Random.Range(1, 65536));
        
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                float xCoord = ((float)x / width) * Scale;
                float yCoord = ((float)y / height) * Scale;
                
                float sample = generator.GetValue(xCoord, yCoord, 0);

                // PinkNoise usually returns value in range[-1,1] but this is not guaranteed.
                sample = Mathf.Clamp(sample, -1, 1);

                // Convert values from range [-1,1] to range [0,1].
                sample = MathUtils.ConvertRange(-1, 1, 0, 1, sample);
                
                samples[y * width + x] = sample;
            }
        }
        
        return samples;
    }

    private Texture2D GenerateTexture(int width, int height, float[] samples) {
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

        Color[] pixels = new Color[width * height];

        for(int i = 0; i < samples.Length; i++) {
            float sample = samples[i];
            pixels[i] = new Color(sample, sample, sample);
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    private void SaveTextureToFile(Texture2D tex, string filepath) {
        string fullpath = Application.dataPath + "/" + filepath;
        FileStream file = File.Open(fullpath, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        
        byte[] bytes = tex.EncodeToPNG();
        writer.Write(bytes);
        writer.Close();

        file.Close();
    }

    // Split the range [0,1] into evenly-spaced intervals and then 
    // round each sample to the closest interval boundary.
    private void Discritize(float[] samples, int intervals) {
        List<float> bounds = new List<float>();
        float unit = 1.0f / intervals;

        for(int i = 0; i <= intervals + 1; i++)
            bounds.Add(i * unit);

        for(int i = 0; i < samples.Length; i++) {
            int boundIndex = bounds.FindIndex(x => x > samples[i]);
            samples[i] = bounds[boundIndex - 1];
        }
    }
}
