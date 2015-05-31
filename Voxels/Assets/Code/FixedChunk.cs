using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class is meant to represent a chunk that will not change and therefore 
// does not need to be updated dynamically.

public class FixedChunk : IChunk {
    public byte[,,] Blocks;

    public FixedChunk(byte[,,] blocks) {
        Blocks = blocks;
    }

    public byte GetBlock(int x, int y, int z) {
        return Blocks[x, y, z];
    }
}
