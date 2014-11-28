using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {
	public WorldConfig Config { get; private set; }

	public GameObjectPool ChunkPool;
	public Chunk[,,] Chunks;

    public TextureAtlas TextureAtlas { get; private set; }

    private float[,] _noise;

    private Dictionary<IntVector2, WorldScreen> _screens;

    protected void Awake() {
        _screens = new Dictionary<IntVector2, WorldScreen>();
    }

	public void Initialize(WorldConfig config, float[,] noise) {
		Config = config;
        _noise = noise;

        TextureAtlas = new TextureAtlas(4, 4);
	}

    public void CreateScreen(IntVector2 screenCoords) {
        float[,] screenNoise = new float[Config.ScreenChunksX, Config.ScreenChunksZ];

        int startX = screenCoords.X * Config.ScreenChunksX;
        int startY = screenCoords.Y * Config.ScreenChunksZ;

        for(int x = 0; x < Config.ScreenChunksX; x++) {
            for(int y = 0; y < Config.ScreenChunksZ; y++) {
                screenNoise[x, y] = _noise[startX + x, startY + y];
            }
        }

        WorldScreen screen = CreateScreenChunks(screenNoise, screenCoords);

        _screens[screenCoords] = screen;
    }

    // The edge size is pretty arbitrary and depends on camera angle/distance.
    // 13 works well for an angle/distance of 40/40.
    public Rect GetScreenBounds(IntVector2 screenCoords, float edgeSize) {
        Vector2 corner = GetScreenCorner(screenCoords);
        Vector2 dimensions = GetScreenDimensions();

        int chunkSize = Config.ChunkSize;

        return new Rect(corner.x + edgeSize,
                        corner.y + edgeSize,
                        dimensions.x - edgeSize * 2,
                        dimensions.y - edgeSize * 2);
    }

    public WorldScreen GetScreen(IntVector2 screenCoords) {
        return _screens.ContainsKey(screenCoords) ? _screens[screenCoords] : null;
    }

    public Vector2 GetScreenDimensions() {
        int chunkSize = Config.ChunkSize;
        return new Vector2(Config.ScreenChunksX * chunkSize,
                           Config.ScreenChunksZ * chunkSize);
    }

    public Vector2 GetScreenCorner(IntVector2 screenCoords) {
        int chunkSize = Config.ChunkSize;
        return new Vector2((float)screenCoords.X * Config.ScreenChunksX * chunkSize,
                           (float)screenCoords.Y * Config.ScreenChunksZ * chunkSize);
    }

    public Vector2 GetScreenCenter(IntVector2 screenCoords) {
        int chunkSize = Config.ChunkSize;
        Vector2 corner = GetScreenCorner(screenCoords);

        return corner + new Vector2((Config.ScreenChunksX * chunkSize) / 2,
                                    (Config.ScreenChunksZ * chunkSize) / 2);
    }

    // TODO: This is badly named.
    private WorldScreen CreateScreenChunks(float[,] chunkGroupNoise, IntVector2 screenCoords) {
        GameObject go = new GameObject();
        go.name = "Screen " + screenCoords.X + "," + screenCoords.Y;

        WorldScreen worldScreen = go.AddComponent<WorldScreen>();
        worldScreen.Initialize(chunkGroupNoise, screenCoords, this);
        worldScreen.CreateChunks();

        go.transform.position = new Vector3(screenCoords.X * Config.ScreenChunksX * Config.ChunkSize, 
                                            0, 
                                            screenCoords.Y * Config.ScreenChunksZ * Config.ChunkSize);

        return worldScreen;
    }
}
