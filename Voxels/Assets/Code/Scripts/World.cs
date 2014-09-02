using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	public int worldX { get; private set; }
	public int worldY { get; private set; }
	public int worldZ { get; private set; }
	public byte[,,] data;

	public GameObject chunk;
	public int chunkSize { get; private set; }
	public Chunk[,,] chunks;

	private void Start() {
		worldX = 256;
		worldY = 128;
		worldZ = 256;
		chunkSize = 16;

		data = new byte[worldX, worldY, worldZ];

		for(int x = 0; x < worldX; x++) {
			for(int z = 0; z < worldZ; z++) {
				int stone = PerlinNoise(x, 0, z, 10, 3, 1.2f);
				stone += PerlinNoise(x, 300, z, 20, 4, 0) + 10;

				int dirt = PerlinNoise(x, 100, z, 50, 2, 0) + 1;

				for(int y = 0; y < worldY; y++) {
					if(y <= stone)
						data[x, y, z] = 1;
					else if(y <= stone + dirt)
						data[x, y, z] = 2;
				}
			}
		}

		chunks = new Chunk[
			Mathf.FloorToInt(worldX / chunkSize),
			Mathf.FloorToInt(worldY / chunkSize),
			Mathf.FloorToInt(worldZ / chunkSize)
		];

		Debug.Log(worldX + " " + chunkSize);

		for (int x = 0; x < chunks.GetLength(0); x++) {
			for (int y = 0; y < chunks.GetLength(1); y++) {
				for (int z = 0; z < chunks.GetLength(2); z++) {
					GameObject newChunkGo = Instantiate(chunk,
						new Vector3(x * chunkSize - 0.5f, y * chunkSize + 0.5f, z * chunkSize - 0.5f), 
						new Quaternion(0, 0, 0, 0)) as GameObject;

					Chunk newChunk = newChunkGo.GetComponent("Chunk") as Chunk;
					newChunk.transform.parent = transform;

					newChunk.worldGO = gameObject;
					newChunk.chunkSize = chunkSize;
					newChunk.chunkX = x * chunkSize;
					newChunk.chunkY = y * chunkSize;
					newChunk.chunkZ = z * chunkSize;

					chunks[x,y,z] = newChunk;
				}
			}
		}
	}

	public byte Block(int x, int y, int z) {
		// avoid hiding tops of blocks at top of map
		if(y >= worldY) return (byte)0;

		if(x >= worldX || x < 0 || y < 0 || z >= worldZ || z < 0)
			return (byte)1;

		return data[x, y, z];
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
