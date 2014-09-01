using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	private int worldX = 4;
	private int worldY = 4;
	private int worldZ = 4;
	private byte[,,] data;

	public GameObject chunk;
	private int chunkSize = 2;

	private GameObject[,,] chunks;

	private void Awake() {
		data = new byte[worldX, worldY, worldZ];

		for(int x = 0; x < worldX; x++) {
			for(int y = 0; y < worldY; y++) {
				for(int z = 0; z < worldZ; z++) {
					//if(y <= 8) {
						data[x, y, z] = 1;
					//}
				}
			}
		}

		chunks = new GameObject[
			Mathf.FloorToInt(worldX / chunkSize),
			Mathf.FloorToInt(worldY / chunkSize),
			Mathf.FloorToInt(worldZ / chunkSize)
		];

		for (int x = 0; x < chunks.GetLength(0); x++) {
			for (int y = 0; y < chunks.GetLength(1); y++) {
				for (int z = 0; z < chunks.GetLength(2); z++) {
					chunks[x,y,z]= Instantiate(chunk, new Vector3(x * chunkSize, y * chunkSize, z * chunkSize), new Quaternion(0, 0, 0, 0)) as GameObject;
 
					Chunk newChunkScript = chunks[x,y,z].GetComponent("Chunk") as Chunk;
 
					newChunkScript.worldGO = gameObject;
					newChunkScript.chunkSize = chunkSize;
					newChunkScript.chunkX = x * chunkSize;
					newChunkScript.chunkY = y * chunkSize;
					newChunkScript.chunkZ = z * chunkSize;
				}
			}
		}
	}

	public byte Block(int x, int y, int z) {
		if(x >= worldX || x < 0 || y >= worldY || y < 0 || z >= worldZ || z < 0)
			return (byte)1;

		return data[x, y, z];
	}
}
