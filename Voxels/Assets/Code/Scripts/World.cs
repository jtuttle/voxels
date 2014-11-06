using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	public WorldConfig Config { get; private set; }

	//public byte[,,] data;

	public GameObject ChunkPrototype;
	public Chunk[,,] Chunks;

    public TextureAtlas TextureAtlas { get; private set; }

	public void Initialize(WorldConfig config) {
		Config = config;

        TextureAtlas = new TextureAtlas(4, 4);
	}
}
