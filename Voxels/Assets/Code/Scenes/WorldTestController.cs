using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldTestController : MonoBehaviour {
    public GameObject Canvas;

    public int Width;
    public int Height;
    public int Elevation;

    private WorldNoiseGenerator _worldNoise;
    private float[,] _currentNoise;
    private Texture2D _currentTexture;

    protected void Start() {
        GenerateWorldNoise();
    }

    protected void Update() {

    }

    protected void OnGUI() {
        if(GUI.Button(new Rect(20, 40, 80, 20), "Refresh"))
            OnRefreshClick();

        if(GUI.Button(new Rect(20, 80, 80, 20), "Find Rooms"))
            OnFindRoomsClick();

        /*
        if(GUI.Button(new Rect(20, 120, 80, 20), "Weight"))
            OnWeightClick();

        if(GUI.Button(new Rect(20, 160, 80, 20), "Blockify"))
            OnBlockifyClick();

        if(GUI.Button(new Rect(20, 200, 80, 20), "Discretize"))
            OnDiscretizeClick();
        */

        if(GUI.Button(new Rect(20, 240, 80, 20), "Save"))
            OnSaveClick();
    }

    private void OnRefreshClick() {
        GenerateWorldNoise();
    }

    private void OnFindRoomsClick() {

        /*
        _currentNoise = _worldNoise.NormalizeAverage(_currentNoise);
        _currentTexture = GenerateTexture(Width, Height, _currentNoise);
        Canvas.renderer.material.mainTexture = _currentTexture;
        */
    }

    private void OnSaveClick() {
        SaveTextureToFile(_currentTexture, "Resources/Textures/test.png");
    }

    private void GenerateWorldNoise() {
        _worldNoise = new WorldNoiseGenerator(Random.Range(1, 65536));

        _currentNoise = _worldNoise.GenerateWorldNoise(Width, Height, Elevation);
        _currentTexture = GenerateTexture(Width, Height, _currentNoise);
        Canvas.renderer.material.mainTexture = _currentTexture;
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
