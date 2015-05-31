using UnityEngine;
using System.Collections;

public class ChunkGroupMesh : DynamicMesh {
    private NewChunkGroup _chunkGroup;

    public ChunkGroupMesh(NewChunkGroup chunkGroup) {
        _chunkGroup = chunkGroup;
    }

    public override Mesh GetMesh() {
        IChunk[,,] chunks = _chunkGroup.Chunks;

        for(int x = 0; x < chunks.GetLength(0); x++) {
            for(int z = 0; z < chunks.GetLength(2); z++) {
                for(int y = 0; y < chunks.GetLength(1); y++) {
                    FixedChunk chunk = (FixedChunk)chunks[x, y, z];
                    AppendChunkMesh(chunk, new XYZ(x, y, z));
                }
            }
        }

        return base.GetMesh();
    }

    private void AppendChunkMesh(FixedChunk chunk, XYZ chunkIndex) {
        byte[,,] blocks = chunk.Blocks;
        IChunk[,,] chunks = _chunkGroup.Chunks;

        XYZ chunkOffset = chunkIndex * _chunkGroup.ChunkSize;

        // This is the index of the first block in the current chunk relative 
        // to the chunk group. It will be added to the current block's offset 
        // to find block neighbors when determining if faces should be drawn.
        XYZ firstBlockChunkGroupIndex = 
            new XYZ(chunkIndex.X * blocks.GetLength(0),
                    chunkIndex.Y * blocks.GetLength(1),
                    chunkIndex.Z * blocks.GetLength(2));

        // This is the maximum index possible in the chunk group. It can be used
        // to guarantee that we never try to call ChunkGroup#GetBlock with an
        // inappropriate value, which allows us to remove the bounds checking in
        // GetBlock and increase our efficiency a bit. Removing for now.
        //XYZ maxBlockIndex = _chunkGroup.Size - new XYZ(1, 1, 1);
        
        for(int x = 0; x < chunk.Blocks.GetLength(0); x++) {
            for(int z = 0; z < chunk.Blocks.GetLength(2); z++) {
                for(int y = 0; y < chunk.Blocks.GetLength(1); y++) {
                    byte block = chunk.GetBlock(x, y, z);
                    
                    if(block == 0) continue;
                    
                    // The below logic is an optimization to only render block faces that
                    // are next to an empty block (and therefore potentially visible).
                    int worldCoordX = firstBlockChunkGroupIndex.X + x;
                    int worldCoordY = firstBlockChunkGroupIndex.Y + y;
                    int worldCoordZ = firstBlockChunkGroupIndex.Z + z;

                    XYZ corner = chunkOffset + new XYZ(x, y, z);

                    // block above is empty
                    if(/*worldCoordY + 1 > maxBlockIndices.Y || */_chunkGroup.GetBlock(worldCoordX, worldCoordY + 1, worldCoordZ) == 0)
                        CubeTop(corner, block);
                    
                    // block below is empty
                    if(/*worldCoordY - 1 < 0 || */_chunkGroup.GetBlock(worldCoordX, worldCoordY - 1, worldCoordZ) == 0)
                        CubeBot(corner, block);
                    
                    // block east is empty
                    if(/*worldCoordX + 1 > maxBlockIndices.X || */_chunkGroup.GetBlock(worldCoordX + 1, worldCoordY, worldCoordZ) == 0)
                        CubeEast(corner, block);
                    
                    // block west is empty
                    if(/*worldCoordX - 1 < 0 || */_chunkGroup.GetBlock(worldCoordX - 1, worldCoordY, worldCoordZ) == 0)
                        CubeWest(corner, block);
                    
                    // block north is empty
                    if(/*worldCoordZ + 1 > maxBlockIndices.Z || */_chunkGroup.GetBlock(worldCoordX, worldCoordY, worldCoordZ + 1) == 0)
                        CubeNorth(corner, block);
                    
                    // block south is empty
                    if(/*worldCoordZ - 1 < 0 || */_chunkGroup.GetBlock(worldCoordX, worldCoordY, worldCoordZ - 1) == 0)
                        CubeSouth(corner, block);
                }
            }
        }
    }

    private void CubeTop(XYZ corner, byte block) {
        AddVertex(corner.X, corner.Y, corner.Z + 1);
        AddVertex(corner.X + 1, corner.Y, corner.Z + 1);
        AddVertex(corner.X + 1, corner.Y, corner.Z);
        AddVertex(corner.X, corner.Y, corner.Z);
        
        AddCubeFace();
    }
    
    private void CubeNorth(XYZ corner, byte block) {
        AddVertex(corner.X + 1, corner.Y - 1, corner.Z + 1);
        AddVertex(corner.X + 1, corner.Y, corner.Z + 1);
        AddVertex(corner.X, corner.Y, corner.Z + 1);
        AddVertex(corner.X, corner.Y - 1, corner.Z + 1);
        
        AddCubeFace();
    }
    
    private void CubeEast(XYZ corner, byte block) {
        AddVertex(corner.X + 1, corner.Y - 1, corner.Z);
        AddVertex(corner.X + 1, corner.Y, corner.Z);
        AddVertex(corner.X + 1, corner.Y, corner.Z + 1);
        AddVertex(corner.X + 1, corner.Y - 1, corner.Z + 1);
        
        AddCubeFace();
    }
    
    private void CubeSouth(XYZ corner, byte block) {
        AddVertex(corner.X, corner.Y - 1, corner.Z);
        AddVertex(corner.X, corner.Y, corner.Z);
        AddVertex(corner.X + 1, corner.Y, corner.Z);
        AddVertex(corner.X + 1, corner.Y - 1, corner.Z);
        
        AddCubeFace();
    }
    
    private void CubeWest(XYZ corner, byte block) {
        AddVertex(corner.X, corner.Y - 1, corner.Z + 1);
        AddVertex(corner.X, corner.Y, corner.Z + 1);
        AddVertex(corner.X, corner.Y, corner.Z);
        AddVertex(corner.X, corner.Y - 1, corner.Z);
        
        AddCubeFace();
    }
    
    private void CubeBot(XYZ corner, byte block) {
        AddVertex(corner.X, corner.Y - 1, corner.Z);
        AddVertex(corner.X + 1, corner.Y - 1, corner.Z);
        AddVertex(corner.X + 1, corner.Y - 1, corner.Z + 1);
        AddVertex(corner.X, corner.Y - 1, corner.Z + 1);
        
        AddCubeFace();
    }

    private void AddCubeFace() {
        int offset = _faceCount * 4;
        
        _tris.Add(offset + 0); //1
        _tris.Add(offset + 1); //2
        _tris.Add(offset + 2); //3
        _tris.Add(offset + 0); //1
        _tris.Add(offset + 2); //3
        _tris.Add(offset + 3); //4
        
        //newUV.AddRange(_textureAtlas.getUVCoords(_textureIndex);
        
        _faceCount++;
    }
}
