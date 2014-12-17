using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldTestController : MonoBehaviour {
    public GameObject Canvas;

    public Vector3 ScreenChunks;
    public Vector3 ScreenCount;

    private WorldGenerator _worldGenerator;
    //private float[,] _currentNoise;
    private World _currentWorld;

    private Texture2D _currentTexture;

    protected void Start() {
        RefreshWorld();
    }

    protected void Update() {

    }

    protected void OnGUI() {
        if(GUI.Button(new Rect(20, 40, 80, 20), "Refresh"))
            OnRefreshClick();

        //if(GUI.Button(new Rect(20, 80, 80, 20), "Find Rooms"))
        //    OnFindRoomsClick();

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
        RefreshWorld();
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

    private void RefreshWorld() {
        XYZ screenChunks = new XYZ((int)ScreenChunks.x, (int)ScreenChunks.y, (int)ScreenChunks.z);
        XYZ screenCount = new XYZ((int)ScreenCount.x, (int)ScreenCount.y, (int)ScreenCount.z);
        WorldConfig worldConfig = new WorldConfig(8, screenChunks, screenCount);

        _worldGenerator = new WorldGenerator();
        World world = _worldGenerator.GenerateWorld(worldConfig);

        _currentWorld = world;
        _currentTexture = GenerateTexture(_currentWorld);
        Canvas.renderer.material.mainTexture = _currentTexture;
    }

    private Texture2D GenerateTexture(World world) {
        XYZ worldChunks = world.Config.WorldChunks;
        int elevations = world.Config.ScreenChunks.Y;

        int width = worldChunks.X;
        int height = worldChunks.Z;
        float normalizeRatio = 1.0f / elevations;

        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

        Color[] pixels = new Color[width * height];

        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                float sample = world.Noise[x, y] * normalizeRatio;
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
