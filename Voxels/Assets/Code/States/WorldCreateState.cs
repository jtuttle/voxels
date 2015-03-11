using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldCreateState : FSMState {
    private WorldScreenManager _worldScreenManager;

    public WorldCreateState()
        : base(GameState.WorldCreate) {

    }

    public override void InitState(FSMTransition transition) {
        base.InitState(transition);

        _worldScreenManager = GameObject.Find("WorldScreenManager").
            GetComponent<WorldScreenManager>();
    }

    public override void EnterState(FSMTransition transition) {
        base.EnterState(transition);

        // TODO: currently all this does is perform calculations to get the 
        // correct square from the chunk texture, which is 4 x 4. It should  
        // probably store the texture as well at some point.
        GameData.TextureAtlas = new TextureAtlas(4, 4);

        string worldName = (transition as WorldCreateTransition).WorldName;
        WorldConfig worldConfig = CreateWorldConfig();

        World world = 
            new WorldGenerator().GenerateWorld(worldName, worldConfig);

        GameData.World = world;

        WorldScreen initialScreen = world.GetScreen(world.InitialRoom);

        CreateInitialScreens(initialScreen.Coord);
        GameData.CurrentScreenCoord = initialScreen.Coord;

        ExitState(new FSMTransition(GameState.PlayerCreate));
    }

    public override void ExitState(FSMTransition nextStateTransition) {
        
        base.ExitState(nextStateTransition);
    }

    public override void Dispose() {
    
        base.Dispose();
    }

    // TODO: This should probably be taken from some global config file or
    // GameObject so that it can be tweaked without touching the code.
    private WorldConfig CreateWorldConfig() {
        // each loaded screen could be 16 x 12 chunks
        // world can be 16 x 16 screens
        
        int chunkSize = 8;
        XYZ screenChunks = new XYZ(16, 8, 12);
        XYZ screenCount = new XYZ(16, 1, 16);
        
        return new WorldConfig(chunkSize, screenChunks, screenCount);
    }

    private void CreateInitialScreens(XY coord) {
        _worldScreenManager.CreateScreen(new XY(0, 0));

        /*
        for(int x = coord.X - 1; x <= coord.X + 1; x++) {
            for(int y = coord.Y; y <= coord.Y + 1; y++)
                _worldScreenManager.CreateScreen(new XY(x, y));
        }
        */
    }

    // TODO: This should be on a NoiseCanvas script or something
    /*
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
    */
}
