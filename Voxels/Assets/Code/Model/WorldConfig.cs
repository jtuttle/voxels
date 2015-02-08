using UnityEngine;
using System.Collections;

// This class describes the world from the same perspective that it is rendered,
// so X and Z represent width and depth, while Y represents elevation.
public class WorldConfig {
    public int ChunkSize { get; private set; }

    public XYZ ScreenChunks { get; private set; }
    public XYZ ScreenCount { get; private set; }
    public XYZ ScreenSize { get; private set; }

    public XYZ WorldChunks { get; private set; }

    public int MinRoomSize { get; private set; }

    public int KeyLevels { get; private set; }

    public WorldConfig(int chunkSize, XYZ screenChunks, XYZ screenCount) {
        ChunkSize = chunkSize;
        ScreenChunks = screenChunks;
        ScreenCount = screenCount;

        ScreenSize = ScreenChunks * chunkSize;

        WorldChunks = new XYZ(screenChunks.X * screenCount.X,
                              screenChunks.Y * screenCount.Y,
                              screenChunks.Z * screenCount.Z);

        // TODO: parameterize these
        MinRoomSize = 16;
        KeyLevels = 2;
	}
}
