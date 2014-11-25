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

        // TEMP - just to see full noise map
        NoiseCanvas.renderer.material.mainTexture = GenerateTexture(chunkCountX, 
                                                                    chunkCountZ, 
                                                                    worldNoiseGen.DiscretizeNormalizedNoise(worldNoise, chunkCountY));

        // Shift noise to ints for easier processing
        worldNoise = worldNoiseGen.ShiftNoise(0, 1, 0, chunkCountY, worldNoise);
        worldNoise = worldNoiseGen.DiscretizeDenormalizedNoise(worldNoise);

        World world = GameObject.Find("World").GetComponent<World>();
        world.Initialize(worldConfig, worldNoise);

        // TODO: This is always coming up ocean, something's wrong!
        IntVector2 startCoords = new IntVector2(0, 0);
        world.CreateScreen(startCoords);

        // Looks like it's not crazy to draw 6 screens so we could
        // have some nice depth to our vision.
        /*
        world.CreateScreen(startCoords + new IntVector2(1, 0));
        world.CreateScreen(startCoords + new IntVector2(2, 0));
        world.CreateScreen(startCoords + new IntVector2(0, 1));
        world.CreateScreen(startCoords + new IntVector2(1, 1));
        world.CreateScreen(startCoords + new IntVector2(2, 1));
        */

        GameData.World = world;
        GameData.CurrentScreenCoords = startCoords;

        // TEMP - place the player
        Vector3 playerStartPos = new Vector3(startCoords.X * worldConfig.ChunkGroupWidth * worldConfig.ChunkSize,
                                             50,
                                             startCoords.Y * worldConfig.ChunkGroupHeight * worldConfig.ChunkSize);

        GameObject playerGo = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Player"));
        Player player = playerGo.GetComponent<Player>();
        player.transform.position = playerStartPos;

        ScreenCamera screenCam = Camera.main.GetComponent<ScreenCamera>();
        screenCam.Player = player;
        screenCam.Bounds = world.GetScreenBounds(startCoords);

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
