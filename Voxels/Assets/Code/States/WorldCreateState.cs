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

        // TEMP - just to see full noise map
        NoiseCanvas.renderer.material.mainTexture = GenerateTexture(chunkCountX, chunkCountZ, worldNoise);

        IntVector2 startCoords = new IntVector2(0, 0);
        world.CreateScreen(startCoords);

        GameData.World = world;
        GameData.CurrentScreenCoords = startCoords;

        // TEMP - place the player
        GameObject playerGo = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Player"));
        Player player = playerGo.GetComponent<Player>();
        player.transform.position = new Vector3(50, 50, 50);
        Camera.main.GetComponent<PlayerCamera>().Player = player;

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
