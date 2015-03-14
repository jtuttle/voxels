using UnityEngine;
using System.Collections;

// This class describes the world from the same perspective that it is rendered,
// so X and Z represent width and depth, while Y represents elevation.
public class WorldConfig {
    public int ChunkSize { get; private set; }

    public XYZ ScreenChunks { get; private set; }
    public XY ScreenCount { get; private set; }
    public XYZ ScreenSize { get; private set; }

    public XYZ WorldChunks { get; private set; }

    public int MinRoomSize { get; private set; }

    public int KeyLevels { get; private set; }

    public WorldConfig(int chunkSize, XYZ screenChunks, XY screenCount) {
        ChunkSize = chunkSize;
        ScreenChunks = screenChunks;
        ScreenCount = screenCount;

        // Now that 1 chunk = 1 unit, these are the same thing. This will 
        // allow altering the number of voxels per chunk without changing
        // the size of everything.
        ScreenSize = ScreenChunks; // * chunkSize;

        WorldChunks = new XYZ(screenChunks.X * screenCount.X,
                              screenChunks.Y,
                              screenChunks.Z * screenCount.Y);

        // TODO: parameterize these
        MinRoomSize = 16;
        KeyLevels = 2;
    }
}
