using UnityEngine;
using System.Collections;

public class WorldConfig {
	public int WorldX { get; private set; }
	public int WorldY { get; private set; }
	public int WorldZ { get; private set; }
	public int ChunkSize { get; private set; }

	public WorldConfig(int worldX, int worldY, int worldZ, int chunkSize) {
		WorldX = worldX;
		WorldY = worldY;
		WorldZ = worldZ;
		ChunkSize = chunkSize;
	}
}
