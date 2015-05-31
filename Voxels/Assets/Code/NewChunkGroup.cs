using System.Collections;
using UnityEngine;

public class NewChunkGroup {
    private IChunk[,,] _chunks;
    public IChunk[,,] Chunks {
        get { return _chunks; }
        set { 
            _chunks = value;
            Size = new XYZ(_chunks.GetLength(0) * ChunkSize.X,
                           _chunks.GetLength(1) * ChunkSize.Y,
                           _chunks.GetLength(2) * ChunkSize.Z);
        }
    }

    public XYZ ChunkSize { get; private set; }
    public XYZ Size { get; private set; }

    public NewChunkGroup(XYZ chunkSize) {
        ChunkSize = chunkSize;
    }
    
    public byte GetBlock(int x, int y, int z) {
        // This is probably horribly inefficient. It can be avoided by only
        // calling this method with appropriate values but this is definitely 
        // a bit more robust.
        if(x < 0 || y < 0 || z < 0 || x > Size.X - 1 || y > Size.Y - 1 || z > Size.Z - 1)
            return 0;

        XYZ chunkCoords = new XYZ((int)Mathf.Floor(x / ChunkSize.X), 
                                  (int)Mathf.Floor(y / ChunkSize.Y), 
                                  (int)Mathf.Floor(z / ChunkSize.Z));

        IChunk chunk = _chunks[chunkCoords.X, chunkCoords.Y, chunkCoords.Z];
        
        if(chunk == null) return 0;
        
        return chunk.GetBlock(x % ChunkSize.X, y % ChunkSize.Y, z % ChunkSize.Z);
    }
}