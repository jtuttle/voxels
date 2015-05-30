using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NoiseTestController : MonoBehaviour {
    public GameObject Canvas;

    public int Width;
    public int Height;
    public int Elevation;

    private WorldNoiseGenerator _worldNoise;
    private float[,] _currentNoise;
    private Texture2D _currentTexture;

    protected void Start() {
        GenerateRawNoise();
    }

    protected void Update() {

    }

    protected void OnGUI() {
        if(GUI.Button(new Rect(20, 20, 80, 20), "Refresh"))
            OnRefreshClick();

        if(GUI.Button(new Rect(20, 60, 80, 20), "Average"))
            OnAverageClick();

        if(GUI.Button(new Rect(20, 100, 80, 20), "Weight"))
            OnWeightClick();

        if(GUI.Button(new Rect(20, 140, 80, 20), "Blockify"))
            OnBlockifyClick();

        if(GUI.Button(new Rect(20, 180, 80, 20), "Discretize"))
            OnDiscretizeClick();

        if(GUI.Button(new Rect(20, 220, 80, 20), "Save"))
            OnSaveClick();
    }

    private void OnRefreshClick() {
        GenerateRawNoise();
    }

    private void OnAverageClick() {
        _currentNoise = _worldNoise.NormalizeAverage(_currentNoise);
        _currentTexture = GenerateTexture(Width, Height, _currentNoise);
        Canvas.GetComponent<Renderer>().material.mainTexture = _currentTexture;
    }

    private void OnWeightClick() {
        _currentNoise = _worldNoise.ApplyCubicWeight(_currentNoise);
        _currentTexture = GenerateTexture(Width, Height, _currentNoise);
        Canvas.GetComponent<Renderer>().material.mainTexture = _currentTexture;
    }

    private void OnBlockifyClick() {
        _currentNoise = _worldNoise.Blockify(_currentNoise);
        _currentTexture = GenerateTexture(Width, Height, _currentNoise);
        Canvas.GetComponent<Renderer>().material.mainTexture = _currentTexture;
    }

    private void OnDiscretizeClick() {
        _currentNoise = _worldNoise.DiscretizeNormalizedNoise(_currentNoise, Elevation);
        _currentTexture = GenerateTexture(Width, Height, _currentNoise);
        Canvas.GetComponent<Renderer>().material.mainTexture = _currentTexture;
    }

    private void OnSaveClick() {
        SaveTextureToFile(_currentTexture, "Resources/Textures/test.png");
    }

    private void GenerateRawNoise() {
        _worldNoise = new WorldNoiseGenerator(Random.Range(1, 65536));

        _currentNoise = _worldNoise.GenerateRawNoise(Width, Height);
        _currentTexture = GenerateTexture(Width, Height, _currentNoise);
        Canvas.GetComponent<Renderer>().material.mainTexture = _currentTexture;
    }

    private Texture2D GenerateTexture(int width, int height, float[,] samples) {
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

        Color[] pixels = new Color[width * height];

        for(int y = 0; y < samples.GetLength(1); y++) {
            for(int x = 0; x < samples.GetLength(0); x++) {
                float sample = samples[x, y];
                pixels[y * width + x] = new Color(sample, sample, sample);
            }
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
}
