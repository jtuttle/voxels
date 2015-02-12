using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldCreateState : FSMState {
    private GameObject NoiseCanvas;

    public WorldCreateState()
        : base(GameState.WorldCreate) {

    }

    public override void InitState(FSMTransition transition) {
        base.InitState(transition);

        NoiseCanvas = GameObject.Find("NoiseCanvas");
    }

    public override void EnterState(FSMTransition transition) {
        base.EnterState(transition);

        // each loaded screen could be 16 x 12 chunks
        // world can be 16 x 16 screens

        int chunkSize = 8;
        XYZ screenChunks = new XYZ(16, 8, 12);
        XYZ screenCount = new XYZ(16, 1, 16);

        WorldConfig worldConfig = new WorldConfig(chunkSize, screenChunks, screenCount);

        XYZ worldChunks = worldConfig.WorldChunks;

        WorldNoiseGenerator worldNoiseGen = new WorldNoiseGenerator(Random.Range(1, 65536));
        float[,] worldNoise = worldNoiseGen.GenerateWorldNoise(worldChunks.X, worldChunks.Z, worldChunks.Y);

        // TEMP - just to see full noise map
        // BUG - this seems to give a pretty different map than is generated in the world,
        // should really make these two sync up.
        NoiseCanvas.renderer.material.mainTexture = GenerateTexture(worldChunks.X, 
                                                                    worldChunks.Z, 
                                                                    worldNoise);

        // Shift noise to ints for easier processing
        worldNoise = worldNoiseGen.ShiftNoise(0, 1, 0, worldChunks.Y, worldNoise);
        worldNoise = worldNoiseGen.DiscretizeDenormalizedNoise(worldNoise);

        WorldComponent world = GameObject.Find("World").GetComponent<WorldComponent>();
        world.Initialize(worldConfig, worldNoise);

        // TODO: This is always coming up ocean, something's wrong!
        XY startCoords = new XY(0, 0);
        CreateInitialScreens(world, startCoords);

        GameData.World = world;
        GameData.CurrentScreen = world.GetScreen(startCoords);

        // TEMP - place the player
        Vector2 screenCenter = world.GetScreenCenter(startCoords);
        Vector3 playerStartPos = new Vector3(screenCenter.x, 50, screenCenter.y);

        GameObject playerGo = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Player"));
        Player player = playerGo.GetComponent<Player>();
        player.transform.position = playerStartPos;

        GameData.Player = player;

        ScreenCamera screenCam = Camera.main.GetComponent<ScreenCamera>();
        screenCam.Player = player;
        screenCam.UpdateBounds(startCoords);

        ExitState(new FSMTransition(GameState.WorldNavigate ));
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
    private void CreateInitialScreens(WorldComponent world, XY startCoords) {
        world.CreateScreen(startCoords);

        /*
        XYZ screenCount = world.Config.ScreenCount;

        WorldConfig config = world.Config;
        int hGroupCount = screenCount.X;
        int vGroupCount = screenCount.Z;

        for(int x = startCoords.X - 1; x <= startCoords.X + 1; x++) {
            for(int y = startCoords.Y; y <= startCoords.Y + 1; y++) {
                if(x >= 0 && x < hGroupCount && y >= 0 && y < vGroupCount)
                    world.CreateScreen(new XY(x, y));
            }
        }
        */
    }
}
