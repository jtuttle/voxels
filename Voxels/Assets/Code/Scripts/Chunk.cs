using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour {
    public WorldScreenComponent chunkGroup;

    public XYZ chunkOffset;

    private byte[,,] _blocks;
    private bool _dirty = false;

    private Mesh mesh;
    private MeshCollider col;

    // TODO - abstract cube building
    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();
    private int faceCount;

    private TextureAtlas _textureAtlas;
    private int _textureIndex;

    protected void Start() {
        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider>();
    }

    protected void LateUpdate() {
        if(_dirty) {
            GenerateMesh();
            _dirty = false;
        }
    }

    public void Initialize(int chunkSize, bool solid, TextureAtlas textureAtlas, int textureIndex) {
        _blocks = new byte[chunkSize, chunkSize, chunkSize];

        // for now this is just going to be hard-coded to be
        // completely filled or completely empty
        byte blockData = (byte)(solid ? 1 : 0);

        for(int x = 0; x < chunkSize; x++) {
            for(int y = 0; y < chunkSize; y++) {
                for(int z = 0; z < chunkSize; z++) {
                    _blocks[x, y, z] = blockData;
                }   
            }
        }

        _textureAtlas = textureAtlas;
        _textureIndex = textureIndex;
    }

    public byte GetBlock(int x, int y, int z) {
        return _blocks[x, y, z];
    }

    public void GenerateMesh() {
        int chunkSize = _blocks.GetLength(0);

        for(int x = 0; x < chunkSize; x++) {
            for(int y = 0; y < chunkSize; y++) {
                for(int z = 0; z < chunkSize; z++) {
                    byte block = GetBlock(x, y, z);

                    // Skip render if block is empty.
                    if(block != 0) {
                        // The below logic is an optimization to only render block faces that
                        // are next to an empty block (and therefore potentially visible).
                        int worldCoordX = chunkOffset.X + x;
                        int worldCoordY = chunkOffset.Y + y;
                        int worldCoordZ = chunkOffset.Z + z;

                        int maxBlockX = chunkGroup.Chunks.GetLength(0) * chunkSize - 1;
                        int maxBlockY = chunkGroup.Chunks.GetLength(1) * chunkSize - 1;
                        int maxBlockZ = chunkGroup.Chunks.GetLength(2) * chunkSize - 1;

                        // block above is empty
                        if(worldCoordY + 1 > maxBlockY || chunkGroup.GetBlock(worldCoordX, worldCoordY + 1, worldCoordZ) == 0)
                            CubeTop(x, y, z, block);

                        // block below is empty
                        if(worldCoordY - 1 < 0 || chunkGroup.GetBlock(worldCoordX, worldCoordY - 1, worldCoordZ) == 0)
                            CubeBot(x, y, z, block);

                        // block east is empty
                        if(worldCoordX + 1 > maxBlockX || chunkGroup.GetBlock(worldCoordX + 1, worldCoordY, worldCoordZ) == 0)
                            CubeEast(x, y, z, block);

                        // block west is empty
                        if(worldCoordX - 1 < 0 || chunkGroup.GetBlock(worldCoordX - 1, worldCoordY, worldCoordZ) == 0)
                            CubeWest(x, y, z, block);

                        // block north is empty
                        if(worldCoordZ + 1 > maxBlockZ || chunkGroup.GetBlock(worldCoordX, worldCoordY, worldCoordZ + 1) == 0)
                            CubeNorth(x, y, z, block);

                        // block south is empty
                        if(worldCoordZ - 1 < 0 || chunkGroup.GetBlock(worldCoordX, worldCoordY, worldCoordZ - 1) == 0)
                            CubeSouth(x, y, z, block);
                    }
                }
            }
        }
        
        UpdateMesh();
    }

    public void MarkDirty() {
        _dirty = true;
    }

    private void UpdateMesh() {
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        col.sharedMesh = null;
        col.sharedMesh = mesh;

        newVertices.Clear();
        newUV.Clear();
        newTriangles.Clear();

        faceCount = 0;
    }

    private void CubeTop(int x, int y, int z, byte block) {
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x, y, z));

        Cube();
    }

    private void CubeNorth(int x, int y, int z, byte block) {
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));
        CubeSide(x, y, z, block);
    }

    private void CubeEast(int x, int y, int z, byte block) {
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        CubeSide(x, y, z, block);
    }

    private void CubeSouth(int x, int y, int z, byte block) {
        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        CubeSide(x, y, z, block);
    }

    private void CubeWest(int x, int y, int z, byte block) {
        newVertices.Add(new Vector3(x, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x, y - 1, z));
        CubeSide(x, y, z, block);
    }

    private void CubeBot(int x, int y, int z, byte block) {
        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        Cube();
    }

    private void CubeSide(int x, int y, int z, byte block) {
        Cube();
    }

    private void Cube() {
        int offset = faceCount * 4;

        newTriangles.Add(offset + 0); //1
        newTriangles.Add(offset + 1); //2
        newTriangles.Add(offset + 2); //3
        newTriangles.Add(offset + 0); //1
        newTriangles.Add(offset + 2); //3
        newTriangles.Add(offset + 3); //4

        newUV.AddRange(_textureAtlas.getUVCoords(_textureIndex));

        faceCount++;
    }
}
