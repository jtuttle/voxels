using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {
	public WorldConfig Config { get; private set; }

	public GameObject ChunkPrototype;
	public Chunk[,,] Chunks;

    public TextureAtlas TextureAtlas { get; private set; }

    private float[,] _noise;

    private Dictionary<IntVector2, ChunkGroup> _screenChunkGroups;

    protected void Awake() {
        _screenChunkGroups = new Dictionary<IntVector2, ChunkGroup>();
    }

	public void Initialize(WorldConfig config, float[,] noise) {
		Config = config;

        _noise = noise;

        TextureAtlas = new TextureAtlas(4, 4);
	}

    public void CreateScreen(IntVector2 screenCoords) {
        float[,] screenNoise = new float[Config.ChunkGroupWidth, Config.ChunkGroupHeight];

        int startX = screenCoords.X * Config.ChunkGroupWidth;
        int startY = screenCoords.Y * Config.ChunkGroupHeight;

        for(int x = 0; x < Config.ChunkGroupWidth; x++) {
            for(int y = 0; y < Config.ChunkGroupHeight; y++) {
                float sample = _noise[startX + x, startY + y];
                Debug.Log(sample);

                screenNoise[x, y] = _noise[startX + x, startY + y];
            }
        }

        ChunkGroup chunkGroup = CreateChunkGroup(screenNoise, screenCoords);

        _screenChunkGroups[screenCoords] = chunkGroup;
    }

    public Rect GetScreenBounds(IntVector2 screenCoords) {
        int chunkSize = Config.ChunkSize;
        int chunkGroupWidth = Config.ChunkGroupWidth;
        int chunkGroupHeight = Config.ChunkGroupHeight;

        // This is pretty arbitrary and depends on camera angle/distance.
        // 13 works well for an angle/distance of 40/40.
        float screenEdge = 13.0f;
        
        return new Rect(screenCoords.X * chunkGroupWidth * chunkSize + screenEdge,
                        screenCoords.Y * chunkGroupHeight * chunkSize + screenEdge,
                        chunkGroupWidth * chunkSize - chunkSize - screenEdge * 2,
                        chunkGroupHeight * chunkSize - chunkSize - screenEdge * 2);
    }

    private ChunkGroup CreateChunkGroup(float[,] chunkGroupNoise, IntVector2 screenCoords) {
        GameObject go = new GameObject();
        go.name = "Screen " + screenCoords.X + "," + screenCoords.Y;

        ChunkGroup chunkGroup = go.AddComponent<ChunkGroup>();
        chunkGroup.Initialize(chunkGroupNoise, screenCoords, this);
        chunkGroup.CreateChunks();

        go.transform.position = new Vector3(screenCoords.X * Config.ChunkGroupWidth * Config.ChunkSize, 
                                            0, 
                                            screenCoords.Y * Config.ChunkGroupHeight * Config.ChunkSize);

        return chunkGroup;
    }
}
