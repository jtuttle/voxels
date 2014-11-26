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

        int chunkSize = 8;
        int worldChunksX = 256;
        int worldChunksY = 7;
        int worldChunksZ = 192;
        int screenChunksX = 16;
        int screenChunksZ = 12;
        
        WorldConfig worldConfig = new WorldConfig(chunkSize,
                                                  worldChunksX, worldChunksY, worldChunksZ,
                                                  screenChunksX, screenChunksZ);
        
        WorldNoiseGenerator worldNoiseGen = new WorldNoiseGenerator(Random.Range(1, 65536));
        float[,] worldNoise = worldNoiseGen.GenerateWorldNoise(worldChunksX, worldChunksZ, worldChunksY);

        // TEMP - just to see full noise map
        NoiseCanvas.renderer.material.mainTexture = GenerateTexture(worldChunksX, 
                                                                    worldChunksZ, 
                                                                    worldNoiseGen.DiscretizeNormalizedNoise(worldNoise, worldChunksY));

        // Shift noise to ints for easier processing
        worldNoise = worldNoiseGen.ShiftNoise(0, 1, 0, worldChunksY, worldNoise);
        worldNoise = worldNoiseGen.DiscretizeDenormalizedNoise(worldNoise);

        World world = GameObject.Find("World").GetComponent<World>();
        world.Initialize(worldConfig, worldNoise);

        // TODO: This is always coming up ocean, something's wrong!
        IntVector2 startCoords = new IntVector2(0, 0);
        CreateInitialChunkGroups(world, startCoords);

        GameData.World = world;
        GameData.CurrentScreenCoords = startCoords;

        // TEMP - place the player
        Vector2 screenCenter = world.GetScreenCenter(startCoords);
        Vector3 playerStartPos = new Vector3(screenCenter.x, 50, screenCenter.y);

        GameObject playerGo = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Player"));
        Player player = playerGo.GetComponent<Player>();
        player.transform.position = playerStartPos;

        ScreenCamera screenCam = Camera.main.GetComponent<ScreenCamera>();
        screenCam.Player = player;
        screenCam.Bounds = world.GetScreenBounds(startCoords, 13.0f);

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

    // Creates the initial set of six chunk groups around the player's starting point.
    // TODO: make this draw from a pool of reusable chunk groups
    private void CreateInitialChunkGroups(World world, IntVector2 startCoords) {
        WorldConfig config = world.Config;
        int hGroupCount = config.WorldChunksX / config.ScreenChunksX;
        int vGroupCount = config.WorldChunksZ / config.ScreenChunksZ;

        for(int x = startCoords.X - 1; x <= startCoords.X + 1; x++) {
            for(int y = startCoords.Y; y <= startCoords.Y + 1; y++) {
                if(x >= 0 && x < hGroupCount && y >= 0 && y < vGroupCount)
                    world.CreateScreen(new IntVector2(x, y));
            }
        }
    }
}
