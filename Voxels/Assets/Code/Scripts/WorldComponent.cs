using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldComponent : MonoBehaviour {
    public WorldConfig Config { get { return _world.Config; } }

    private World _world;
    private float[,] _noise;

    public GameObjectPool ChunkPool;
    public Chunk[,,] Chunks;

    public TextureAtlas TextureAtlas { get; private set; }

    private Dictionary<XY, WorldScreenComponent> _screens;

    protected void Awake() {
        _screens = new Dictionary<XY, WorldScreenComponent>();
    }

    public void Initialize(World world) {
        _world = world;

        /*
        Config = config;
        _noise = noise;
        */

        TextureAtlas = new TextureAtlas(4, 4);
    }

    public void CreateScreen(XY screenCoords) {
        XYZ screenChunks = Config.ScreenChunks;

        float[,] screenNoise = new float[screenChunks.X, screenChunks.Z];

        int startX = screenCoords.X * screenChunks.X;
        int startY = screenCoords.Y * screenChunks.Z;

        for(int x = 0; x < screenChunks.X; x++) {
            for(int y = 0; y < screenChunks.Z; y++) {
                screenNoise[x, y] = _world.Noise[startX + x, startY + y];
            }
        }

        WorldScreenComponent screen = CreateScreenChunks(screenNoise, screenCoords);

        _screens[screenCoords] = screen;
    }

    public WorldScreenComponent GetScreen(XY screenCoords) {
        return _screens.ContainsKey(screenCoords) ? _screens[screenCoords] : null;
    }

    public Vector2 GetScreenDimensions() {
        XYZ screenSize = Config.ScreenSize;
        return new Vector2(screenSize.X, screenSize.Z);
    }

    public Vector2 GetScreenCorner(XY screenCoords) {
        XYZ screenSize = Config.ScreenSize;

        return new Vector2((float)screenCoords.X * screenSize.X,
                           (float)screenCoords.Y * screenSize.Z);
    }

    public Vector2 GetScreenCenter(XY screenCoords) {
        XYZ screenSize = Config.ScreenSize;

        Vector2 corner = GetScreenCorner(screenCoords);

        return corner + new Vector2(screenSize.X / 2, screenSize.Z / 2);
    }

    // The edge size is pretty arbitrary and depends on camera angle/distance.
    // 13 works well for an angle/distance of 40/40.
    public Rect GetScreenBounds(XY screenCoords, float horizontalEdge, float verticalEdge) {
        Vector2 corner = GetScreenCorner(screenCoords);
        Vector2 dimensions = GetScreenDimensions();
        
        int chunkSize = Config.ChunkSize;
        
        return new Rect(corner.x + horizontalEdge,
                        corner.y + verticalEdge,
                        dimensions.x - horizontalEdge * 2,
                        dimensions.y - verticalEdge * 2);
    }

    // TODO: This is badly named.
    private WorldScreenComponent CreateScreenChunks(float[,] chunkGroupNoise, XY screenCoords) {
        XYZ screenSize = Config.ScreenSize;

        GameObject go = new GameObject();
        go.name = "Screen " + screenCoords.X + "," + screenCoords.Y;

        WorldScreenComponent worldScreen = go.AddComponent<WorldScreenComponent>();
        worldScreen.Initialize(chunkGroupNoise, screenCoords, this);
        worldScreen.CreateChunks();

        go.transform.position = new Vector3(screenCoords.X * screenSize.X, 
                                            0, 
                                            screenCoords.Y * screenSize.Z);

        return worldScreen;
    }
}
