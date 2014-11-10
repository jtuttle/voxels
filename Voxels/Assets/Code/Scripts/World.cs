using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	public WorldConfig Config { get; private set; }

	public GameObject ChunkPrototype;
	public Chunk[,,] Chunks;

    public TextureAtlas TextureAtlas { get; private set; }

	public void Initialize(WorldConfig config) {
		Config = config;

        TextureAtlas = new TextureAtlas(4, 4);
	}

    public void CreateChunkGroup(float[,] samples, Vector2 offset) {
        GameObject go = new GameObject();
        go.name = "ChunkGroup";

        ChunkGroup chunkGroup = go.AddComponent<ChunkGroup>();
        chunkGroup.Initialize(samples, offset, this);
        chunkGroup.CreateChunks();

        go.transform.position = new Vector3(offset.x, 0, offset.y);
    }
}
