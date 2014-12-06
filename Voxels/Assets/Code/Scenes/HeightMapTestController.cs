using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Fractal;
using CoherentNoise.Interpolation;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HeightMapTestController : MonoBehaviour {
    public int Width;
    public int Height;
    public float Scale;

    public float HeightScale;

    private int _currentSeed;
    private float[] _currentSamples;

    private GameObject[,] _cubes;

    protected void Start() {
        _cubes = new GameObject[256, 256];

        for(int x = 0; x < Width; x++) {
            for(int y = 0; y < Width; y++) {
                _cubes[x,y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
        }

        RefreshScene();
    }

    protected void Update() {

    }

    protected void OnGUI() {
        if(GUI.Button(new Rect(20, 40, 80, 20), "Refresh"))
            OnRefreshClick();

        if(GUI.Button(new Rect(20, 80, 80, 20), "Save"))
            OnSaveClick();
    }

    private void OnRefreshClick() {
        RefreshScene();
    }

    private void RefreshScene() {
        CreateScene();
    }

    private void CreateScene() {
        _currentSamples = GenerateCoherentNoise(Width, Height);

        Debug.Log(_currentSeed);

        for(int z = 0; z < Height; z++) {
            for(int x = 0; x < Width; x++) {
                GameObject cube = _cubes[x,z];

                float sample = _currentSamples[z * Width + x];
                //Debug.Log(sample);
                float height = sample * HeightScale;

                cube.transform.localScale = new Vector3(0.8f, height, 0.8f);
                cube.transform.position = new Vector3(x, 0 + (height / 2), z);
            }
        }
    }

    private void OnSaveClick() {
        //SaveTextureToFile(_currentTexture, "Resources/Textures/test.png");
    }

    private float[] GenerateCoherentNoise(int width, int height) {
        float[] samples = new float[width * height];

        //_currentSeed = 43831;
        _currentSeed = Random.Range(1, 65536);

        //Generator generator = new ValueNoise(Random.Range(1, 65536), SCurve.Cubic);
        Generator generator = new PinkNoise(_currentSeed);

        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                float xCoord = (((float)x / width) + 0) * Scale;
                float yCoord = (((float)y / height) + 0) * Scale;

                float sample = generator.GetValue(xCoord, yCoord, 0);
                //Debug.Log("initial sample: " + sample);

                // Pink noise will mostly fall in [-1,1] but this is not guaranteed
                sample = Mathf.Clamp(sample, -1, 1);
                //Debug.Log("clamped: " + sample);

                // Shift sample value to the [0,1] range
                sample = MathUtils.ConvertRange(-1, 1, 0, 1, sample);
                //Debug.Log("normalized: " + sample);

                samples[y * width + x] = sample;
            }
        }

        return samples;
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
