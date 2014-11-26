using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {
	public WorldConfig Config { get; private set; }

	public GameObject ChunkPrototype;
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

        WorldScreen chunkGroup = CreateChunkGroup(screenNoise, screenCoords);

        _screens[screenCoords] = chunkGroup;
    }

    // The screen edge size is pretty arbitrary and depends on camera angle/distance.
    // 13 works well for an angle/distance of 40/40.
    public Rect GetScreenBounds(IntVector2 screenCoords, float edgeSize) {
        int chunkSize = Config.ChunkSize;
        int chunkGroupWidth = Config.ScreenChunksX;
        int chunkGroupHeight = Config.ScreenChunksZ;

        return new Rect(screenCoords.X * chunkGroupWidth * chunkSize + edgeSize,
                        screenCoords.Y * chunkGroupHeight * chunkSize + edgeSize,
                        chunkGroupWidth * chunkSize - chunkSize - edgeSize * 2,
                        chunkGroupHeight * chunkSize - chunkSize - edgeSize * 2);
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

    private WorldScreen CreateChunkGroup(float[,] chunkGroupNoise, IntVector2 screenCoords) {
        GameObject go = new GameObject();
        go.name = "Screen " + screenCoords.X + "," + screenCoords.Y;

        WorldScreen chunkGroup = go.AddComponent<WorldScreen>();
        chunkGroup.Initialize(chunkGroupNoise, screenCoords, this);
        chunkGroup.CreateChunks();

        go.transform.position = new Vector3(screenCoords.X * Config.ScreenChunksX * Config.ChunkSize, 
                                            0, 
                                            screenCoords.Y * Config.ScreenChunksZ * Config.ChunkSize);

        return chunkGroup;
    }
}
