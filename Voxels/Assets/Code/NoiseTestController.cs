using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Fractal;
using CoherentNoise.Interpolation;
using System.Collections;
using System.IO;
using UnityEngine;

public class NoiseTestController : MonoBehaviour {
    public GameObject Canvas;

    public int Width;
    public int Height;

    public float Scale;
    public float Depth;
    public float Power;

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
        //_currentTexture.SetPixels(GenerateNoise());
        //_currentTexture.SetPixels(GenerateNoiseUnity());
        _currentTexture.SetPixels(GenerateCoherentNoise());
        _currentTexture.Apply();

        //SaveTextureToFile(_currentTexture, "Resources/Textures/test.png");
        //_currentTexture = (Texture2D)Resources.Load("Textures/test.png");

        Canvas.renderer.material.mainTexture = _currentTexture;
    }

    private Color[] GenerateCoherentNoise() {
        Color[] colors = new Color[Width * Height];


        //Generator generator = new ValueNoise(3465478, SCurve.Cubic);

        Generator generator = new PinkNoise(Random.Range(1, 65536));

        float oldRange = 1 - (-1);
        float newRange = 1 - 0;

        for(int y = 0; y < Height; y++) {
            for(int x = 0; x < Width; x++) {
                float xCoord = (float)x / Width;
                float yCoord = (float)y / Height;

                float sample = generator.GetValue(xCoord, yCoord, 0);// + 1;
                sample = (((sample - (-1)) * newRange) / oldRange) + 0;

                colors[y * Width + x] = new Color(sample, sample, sample);
            }
        }

        return colors;
    }

    private Color[] GenerateNoise() {
        Color[] colors = new Color[Width * Height];

        for(int y = 0; y < Height; y++) {
            for(int x = 0; x < Width; x++) {
                float sample = Noise.GetNoise(x, y, 10, Scale, Depth, Power);
                colors[y * Width + x] = new Color(sample, sample, sample);
            }
        }

        return colors;
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
