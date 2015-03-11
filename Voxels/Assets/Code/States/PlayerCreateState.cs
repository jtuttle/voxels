using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCreateState : FSMState {
    private WorldScreenManager _worldScreenManager;

    public PlayerCreateState()
        : base(GameState.PlayerCreate) {

    }

    public override void InitState(FSMTransition transition) {
        base.InitState(transition);

        _worldScreenManager = GameObject.Find("WorldScreenManager").
            GetComponent<WorldScreenManager>();
    }

    public override void EnterState(FSMTransition transition) {
        base.EnterState(transition);

        World world = GameData.World;
        WorldScreen initialScreen = world.GetScreen(world.InitialRoom);

        Vector2 screenCenter = 
            _worldScreenManager.GetScreenCenter(initialScreen.Coord);

        Vector3 playerStartPos = 
            new Vector3(screenCenter.x, 10, screenCenter.y);

        GameObject playerGo = (GameObject)GameObject.
            Instantiate(Resources.Load("Prefabs/Player"));

        Player player = playerGo.GetComponent<Player>();
        player.transform.name = "Player";
        player.transform.position = playerStartPos;

        Camera.main.GetComponent<BoundedTargetCamera>().Target = playerGo;

        ExitState(new FSMTransition(GameState.WorldNavigate));
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
        for(int x = coord.X - 1; x <= coord.X + 1; x++) {
            for(int y = coord.Y; y <= coord.Y + 1; y++)
                _worldScreenManager.CreateScreen(new XY(x, y));
        }
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
