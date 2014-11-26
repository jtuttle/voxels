using UnityEngine;
using System.Collections;

public class WorldConfig {
    public int ChunkSize { get; private set; }

	public int WorldChunksX { get; private set; }
	public int WorldChunksY { get; private set; }
	public int WorldChunksZ { get; private set; }

    public int ScreenChunksX { get; private set; }
    public int ScreenChunksZ { get; private set; }

    // TODO: figure out consistent naming scheme and dimensions.
    public WorldConfig(int chunkSize, int worldChunksX, int worldChunksY, int worldChunksZ, int screenChunksX, int screenChunksZ) {
        ChunkSize = chunkSize;
		WorldChunksX = worldChunksX;
		WorldChunksY = worldChunksY;
		WorldChunksZ = worldChunksZ;
        ScreenChunksX = screenChunksX;
        ScreenChunksZ = screenChunksZ;
	}
}
