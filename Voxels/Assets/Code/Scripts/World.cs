using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	public WorldConfig Config { get; private set; }

	public GameObject ChunkPrototype;
	public Chunk[,,] Chunks;

    public TextureAtlas TextureAtlas { get; private set; }

    private float[,] _noise;

	public void Initialize(WorldConfig config, float[,] noise) {
		Config = config;

        _noise = noise;

        TextureAtlas = new TextureAtlas(4, 4);
	}

    public void CreateScreen(IntVector2 screenCoords) {
        float[,] screenNoise = new float[Config.ChunkGroupWidth, Config.ChunkGroupHeight];

        int startX = screenCoords.X * Config.ChunkGroupWidth;
        int startY = screenCoords.X * Config.ChunkGroupWidth;

        for(int x = startX; x < startX + Config.ChunkGroupWidth; x++) {
            for(int y = startY; y < startY + Config.ChunkGroupHeight; y++) {
                screenNoise[x, y] = _noise[x, y];
            }
        }

        CreateChunkGroup(screenNoise, screenCoords);
    }

    private void CreateChunkGroup(float[,] noise, IntVector2 screenCoords) {
        GameObject go = new GameObject();
        go.name = "Screen " + screenCoords.X + "," + screenCoords.Y;

        ChunkGroup chunkGroup = go.AddComponent<ChunkGroup>();
        chunkGroup.Initialize(noise, screenCoords, this);
        chunkGroup.CreateChunks();

        go.transform.position = new Vector3(screenCoords.X, 0, screenCoords.Y);
    }
}
