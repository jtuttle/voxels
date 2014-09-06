using UnityEngine;
using System.Collections;
using System.IO;

public class NoiseTestController : MonoBehaviour {
    public GameObject Canvas;

    public int Width;
    public int Height;
    public float Scale;

    private Texture2D _currentTexture;

    protected void Start() {
        GenerateTexture();
    }

    protected void Update() {

    }

    protected void OnGUI() {
        if(GUI.Button(new Rect(20, 40, 80, 20), "Refresh"))
            OnRefreshClick();
    }

    private void OnRefreshClick() {
        GenerateTexture();
    }

    private void OnWriteClick() {

    }

    private void GenerateTexture() {
        _currentTexture = new Texture2D(Width, Height, TextureFormat.ARGB32, false);
        _currentTexture.SetPixels(GenerateNoiseUnity());
        _currentTexture.Apply();

        //SaveTextureToFile(_currentTexture, "Resources/Textures/test.png");
        //_currentTexture = (Texture2D)Resources.Load("Textures/test.png");

        Canvas.renderer.material.mainTexture = _currentTexture;
    }

    private Color[] GenerateNoise() {
        return null;
    }

    private Color[] GenerateNoiseUnity() {
        Color[] colors = new Color[Width * Height];

        Perlin perlin = new Perlin();

        for(int y = 0; y < Height; y++) {
            for(int x = 0; x < Width; x++) {
                float xCoord = (float)x / Width * Scale;
                float yCoord = (float)y / Height * Scale;

                float sample = perlin.Noise(xCoord, yCoord); //Mathf.PerlinNoise(xCoord, yCoord);
                colors[y * Width + x] = new Color(sample, sample, sample);
            }
        }

        return colors;
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
