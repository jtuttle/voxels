using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NoiseTestController : MonoBehaviour {
    public GameObject Canvas;

    public int Width;
    public int Height;
    public int Elevations;
    public float Scale;

    private WorldGenerator _world;
    private float[] _currentNoise;
    private Texture2D _currentTexture;

    protected void Start() {
        GenerateNoiseTest();
    }

    protected void Update() {

    }

    protected void OnGUI() {
        if(GUI.Button(new Rect(20, 40, 80, 20), "Refresh"))
            OnRefreshClick();

        if(GUI.Button(new Rect(20, 80, 80, 20), "Discretize"))
            OnDiscretizeClick();

        if(GUI.Button(new Rect(20, 120, 80, 20), "Save"))
            OnSaveClick();
    }

    private void OnRefreshClick() {
        GenerateNoiseTest();
    }

    private void OnDiscretizeClick() {
        _currentNoise = _world.DiscretizeNormalizedNoise(_currentNoise, Elevations);
        _currentTexture = GenerateTexture(Width, Height, _currentNoise);
        Canvas.renderer.material.mainTexture = _currentTexture;
    }

    private void OnSaveClick() {
        SaveTextureToFile(_currentTexture, "Resources/Textures/test.png");
    }

    private void GenerateNoiseTest() {
        _world = new WorldGenerator(Random.Range(1, 65536));

        _currentNoise = _world.GenerateRawNoise(Width, Height);
        _currentTexture = GenerateTexture(Width, Height, _currentNoise);
        Canvas.renderer.material.mainTexture = _currentTexture;
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
}
