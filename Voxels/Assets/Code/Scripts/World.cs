using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	public WorldConfig Config { get; private set; }

	public byte[,,] data;

	public GameObject chunk;
	public Chunk[,,] chunks;

	public void Initialize(WorldConfig config) {
		Config = config;
	}

	public void Generate() {
		CreateBlocks();
		CreateChunks();
	}

	public byte Block(int x, int y, int z) {
		// avoid hiding tops of blocks at top of map
		if(y >= Config.WorldY) return (byte)0;

		if(x >= Config.WorldX || x < 0 || y < 0 || z >= Config.WorldZ || z < 0)
			return (byte)1;

		return data[x, y, z];
	}

	private void CreateBlocks() {
		data = new byte[Config.WorldX, Config.WorldY, Config.WorldZ];

		for(int x = 0; x < Config.WorldX; x++) {
			for(int z = 0; z < Config.WorldZ; z++) {
				int stone = PerlinNoise(x, 0, z, 10, 3, 1.2f);
				stone += PerlinNoise(x, 300, z, 20, 4, 0) + 10;

				int dirt = PerlinNoise(x, 100, z, 50, 2, 0) + 1;

				for(int y = 0; y < Config.WorldY; y++) {
					if(y <= stone)
						data[x, y, z] = 1;
					else if(y <= stone + dirt)
						data[x, y, z] = 2;
				}
			}
		}
	}

	private void CreateChunks() {
		chunks = new Chunk[
			Mathf.FloorToInt(Config.WorldX / Config.ChunkSize),
			Mathf.FloorToInt(Config.WorldY / Config.ChunkSize),
			Mathf.FloorToInt(Config.WorldZ / Config.ChunkSize)
		];

		for(int x = 0; x < chunks.GetLength(0); x++) {
			for(int y = 0; y < chunks.GetLength(1); y++) {
				for(int z = 0; z < chunks.GetLength(2); z++) {
					GameObject newChunkGo = Instantiate(chunk,
						new Vector3(x * Config.ChunkSize - 0.5f, y * Config.ChunkSize + 0.5f, z * Config.ChunkSize - 0.5f),
						new Quaternion(0, 0, 0, 0)) as GameObject;

					Chunk newChunk = newChunkGo.GetComponent("Chunk") as Chunk;
					newChunk.transform.parent = transform;

					newChunk.worldGO = gameObject;
					newChunk.chunkSize = Config.ChunkSize;
					newChunk.chunkX = x * Config.ChunkSize;
					newChunk.chunkY = y * Config.ChunkSize;
					newChunk.chunkZ = z * Config.ChunkSize;

					chunks[x, y, z] = newChunk;
				}
			}
		}
	}

	private int PerlinNoise(int x, int y, int z, float scale, float height, float power) {
		float rValue;
		rValue = Noise.GetNoise(((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
		rValue *= height;

		if(power != 0)
			rValue = Mathf.Pow(rValue, power);

		return (int)rValue;
	}
}
