using UnityEngine;
using System.Collections;

public class WorldConfig {
	public int ChunkCountX { get; private set; }
	public int ChunkCountY { get; private set; }
	public int ChunkCountZ { get; private set; }
	public int ChunkSize { get; private set; }

	public WorldConfig(int chunkCountX, int chunkCountY, int chunkCountZ, int chunkSize) {
		ChunkCountX = chunkCountX;
		ChunkCountY = chunkCountY;
		ChunkCountZ = chunkCountZ;
		ChunkSize = chunkSize;
	}
}
