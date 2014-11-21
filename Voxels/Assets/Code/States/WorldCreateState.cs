using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldCreateState : FSMState {
    private GameObject NoiseCanvas;

    public WorldCreateState()
        : base(GameState.WorldCreate) {

    }

    public override void InitState(FSMState prevState) {
        base.InitState(prevState);

        NoiseCanvas = GameObject.Find("NoiseCanvas");
    }

    public override void EnterState(FSMState prevState) {
        base.EnterState(prevState);

        // each loaded screen could be 16 x 12 chunks
        // world can be 16 x 16 screens
        
        int chunkCountX = 256;
        int chunkCountY = 7;
        int chunkCountZ = 192;
        int chunkSize = 8;
        int chunkGroupWidth = 16;
        int chunkGroupHeight = 12;
        
        WorldConfig worldConfig = new WorldConfig(chunkCountX, chunkCountY, chunkCountZ, 
                                                  chunkSize, chunkGroupWidth, chunkGroupHeight);
        
        WorldNoiseGenerator worldNoiseGen = new WorldNoiseGenerator(Random.Range(1, 65536));
        float[,] worldNoise = worldNoiseGen.GenerateWorldNoise(chunkCountX, chunkCountZ, chunkCountY);
        
        World world = GameObject.Find("World").GetComponent<World>();
        world.Initialize(worldConfig, worldNoise);

        // quick test of chunk groups
        /*
        float[,] samples = new float[16, 12];
        Vector2 offset = new Vector2(4, 4);
        
        for(int x = 0; x < samples.GetLength(0); x++) {
            for(int y = 0; y < samples.GetLength(1); y++) {
                samples[x, y] = worldNoise[x, y];
            }
        }
        
        world.CreateChunkGroup(samples, new Vector2(0, 0));
        world.CreateChunkGroup(samples, new Vector2(200, 0));
        */
        
        NoiseCanvas.renderer.material.mainTexture = GenerateTexture(chunkCountX, chunkCountZ, worldNoise);

        IntVector2 startCoords = new IntVector2(0, 0);
        world.CreateScreen(startCoords);

        GameData.World = world;
        GameData.CurrentScreenCoords = startCoords;

        ExitState(new FSMTransition(GameState.WorldNavigate));
    }

    public override void ExitState(FSMTransition nextStateTransition) {
        
        base.ExitState(nextStateTransition);
    }

    public override void Dispose() {
    
        base.Dispose();
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
}
