using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: This could be a singleton...

// The WorldScreenManager is responsible for managing the Unity representation
// of WorldScreens. It uses the chunk pool to build and destroy them on demand 
// and provides information about screen dimensions and boundaries.

public class WorldScreenManager : MonoBehaviour {
    // TODO: Pool this.
    public GameObject DynamicMeshPrototype;

    private GameObjectPool _chunkPool;

    private Dictionary<XY, ChunkGroup> _screenChunks;

    private Dictionary<XY, GameObject> _screenMeshes;

    protected void Awake() {
        _chunkPool = GameObject.Find("ChunkPool").
            GetComponent<GameObjectPool>();

        _screenChunks = new Dictionary<XY, ChunkGroup>();

        _screenMeshes = new Dictionary<XY, GameObject>();
    }

	public void CreateScreen(XY screenCoord) {
        World world = GameData.World;

        WorldScreen screen = world.GetScreen(screenCoord);
        float[,] screenNoise = world.GetScreenNoise(screen.Coord);

        //ChunkGroup screenChunks = CreateScreenChunks(screen.Coord, screenNoise);
        //_screenChunks[screen.Coord] = screenChunks;

        GenerateMesh(screenCoord, screenNoise);
    }

    private void GenerateMesh(XY screenCoord, float[,] screenNoise) {
        WorldConfig worldConfig = GameData.World.Config;

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        // Offset of vertices from which to make a triangle in verts array.
        int offset = 0;

        int chunkSize = worldConfig.ChunkSize;
        float voxelSize = 1.0f / chunkSize;

        for(int sx = 0; sx < screenNoise.GetLength(0); sx++) {
            for(int sz = 0; sz < screenNoise.GetLength(1); sz++) {
                float sample = (int)screenNoise[sx, sz];
                int textureIndex = 12;

                // top face
                for(int vx = 0; vx < chunkSize; vx++) {
                    for(int vz = 0; vz < chunkSize; vz++) {
                        float x = sx + vx * voxelSize;
                        float y = sample;
                        float z = sz + vz * voxelSize;

                        CubeTop(verts, x, y, z, voxelSize);
                        CubeFaceTris(tris, offset);
                        
                        offset += 4;

                        TextureAtlas atlas = GameData.TextureAtlas;
                        uvs.AddRange(atlas.getUVCoords(textureIndex));
                    }
                }

                // west face
                if(sx > 0 && screenNoise[sx - 1, sz] < sample) {
                    for(int vz = 0; vz < chunkSize; vz++) {
                        for(int vy = 0; vy < chunkSize; vy++) {
                            float x = sx;
                            float y = sample - vy * voxelSize;
                            float z = sz + vz * voxelSize;
                            
                            CubeWestVerts(verts, x, y, z, voxelSize);
                            CubeFaceTris(tris, offset);
                            
                            offset += 4;

                            TextureAtlas atlas = GameData.TextureAtlas;
                            uvs.AddRange(atlas.getUVCoords(textureIndex));
                        }
                    }
                }

                // east face 
                if(sx < screenNoise.GetLength(0) - 1 && screenNoise[sx + 1, sz] < sample) {
                    float x = sx + voxelSize * (chunkSize - 1);

                    for(int vz = 0; vz < chunkSize; vz++) {
                        for(int vy = 0; vy < chunkSize; vy++) {
                            float y = sample - vy * voxelSize;
                            float z = sz + vz * voxelSize;
                            
                            CubeEastVerts(verts, x, y, z, voxelSize);
                            CubeFaceTris(tris, offset);
                            
                            offset += 4;

                            TextureAtlas atlas = GameData.TextureAtlas;
                            uvs.AddRange(atlas.getUVCoords(textureIndex));
                        }
                    }
                }

                // south face 
                if(sz > 0 && screenNoise[sx, sz - 1] < sample) {
                    for(int vx = 0; vx < chunkSize; vx++) {
                        for(int vy = 0; vy < chunkSize; vy++) {
                            float x = sx + vx * voxelSize;
                            float y = sample - vy * voxelSize;
                            float z = sz;
                            
                            CubeSouthVerts(verts, x, y, z, voxelSize);
                            CubeFaceTris(tris, offset);
                            
                            offset += 4;

                            TextureAtlas atlas = GameData.TextureAtlas;
                            uvs.AddRange(atlas.getUVCoords(textureIndex));
                        }
                    }
                }

                // north face 
                if(sz < screenNoise.GetLength(1) - 1 && screenNoise[sx, sz + 1] < sample) {
                    float z = sz + voxelSize * (chunkSize - 1);

                    for(int vx = 0; vx < chunkSize; vx++) {
                        for(int vy = 0; vy < chunkSize; vy++) {
                            float x = sx + vx * voxelSize;
                            float y = sample - vy * voxelSize;
                            
                            CubeNorthVerts(verts, x, y, z, voxelSize);
                            CubeFaceTris(tris, offset);
                            
                            offset += 4;

                            TextureAtlas atlas = GameData.TextureAtlas;
                            uvs.AddRange(atlas.getUVCoords(textureIndex));
                        }
                    }
                }
            }
        }

        XYZ screenChunks = worldConfig.ScreenChunks;

        Vector3 screenPos = new Vector3(screenCoord.X * screenChunks.X,
                                        0,
                                        screenCoord.Y * screenChunks.Z);

        GameObject dynamicMesh = (GameObject)Instantiate(DynamicMeshPrototype, 
                                                         screenPos, 
                                                         Quaternion.identity);
        dynamicMesh.transform.parent = transform;

        // Generate the mesh object.
        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        // Don't think this is necessary
        //mesh.RecalculateBounds();

        mesh.Optimize();
        mesh.RecalculateNormals();

        dynamicMesh.GetComponent<MeshFilter>().mesh = mesh;

        MeshCollider collider = dynamicMesh.GetComponent<MeshCollider>();
        collider.sharedMesh = null;
        collider.sharedMesh = mesh;

        _screenMeshes[screenCoord] = dynamicMesh;
    }

    // TODO: Abstract all this stuff to a helper class.
    private void CubeTop(List<Vector3> verts, float x, float y, float z, float size) {
        verts.Add(new Vector3(x, y, z + size));
        verts.Add(new Vector3(x + size, y, z + size));
        verts.Add(new Vector3(x + size, y, z));
        verts.Add(new Vector3(x, y, z));
    }
    
    private void CubeNorthVerts(List<Vector3> verts, float x, float y, float z, float size) {
        verts.Add(new Vector3(x + size, y - size, z + size));
        verts.Add(new Vector3(x + size, y, z + size));
        verts.Add(new Vector3(x, y, z + size));
        verts.Add(new Vector3(x, y - size, z + size));
    }
    
    private void CubeEastVerts(List<Vector3> verts, float x, float y, float z, float size) {
        verts.Add(new Vector3(x + size, y - size, z));
        verts.Add(new Vector3(x + size, y, z));
        verts.Add(new Vector3(x + size, y, z + size));
        verts.Add(new Vector3(x + size, y - size, z + size));
    }
    
    private void CubeSouthVerts(List<Vector3> verts, float x, float y, float z, float size) {
        verts.Add(new Vector3(x, y - size, z));
        verts.Add(new Vector3(x, y, z));
        verts.Add(new Vector3(x + size, y, z));
        verts.Add(new Vector3(x + size, y - size, z));
    }
    
    private void CubeWestVerts(List<Vector3> verts, float x, float y, float z, float size) {
        verts.Add(new Vector3(x, y - size, z + size));
        verts.Add(new Vector3(x, y, z + size));
        verts.Add(new Vector3(x, y, z));
        verts.Add(new Vector3(x, y - size, z));
    }
    
    private void CubeBotVerts(List<Vector3> verts, float x, float y, float z, float size) {
        verts.Add(new Vector3(x, y - size, z));
        verts.Add(new Vector3(x + size, y - size, z));
        verts.Add(new Vector3(x + size, y - size, z + size));
        verts.Add(new Vector3(x, y - size, z + size));
    }

    private void CubeFaceTris(List<int> tris, int offset) {
        tris.Add(offset + 0); //1
        tris.Add(offset + 1); //2
        tris.Add(offset + 2); //3
        tris.Add(offset + 0); //1
        tris.Add(offset + 2); //3
        tris.Add(offset + 3); //4
    }

    public void DestroyScreen(XY coord) {

    }

    public Vector2 GetScreenDimensions() {
        XYZ screenSize = GameData.World.Config.ScreenSize;
        return new Vector2(screenSize.X, screenSize.Z);
    }
    
    public Vector2 GetScreenCorner(XY screenCoord) {
        XYZ screenSize = GameData.World.Config.ScreenSize;
        
        return new Vector2((float)screenCoord.X * screenSize.X,
                           (float)screenCoord.Y * screenSize.Z);
    }
    
    public Vector2 GetScreenCenter(XY screenCoord) {
        XYZ screenSize = GameData.World.Config.ScreenSize;
        
        Vector2 corner = GetScreenCorner(screenCoord);
        
        return corner + new Vector2(screenSize.X / 2, screenSize.Z / 2);
    }

    public Rect GetScreenBounds(XY screenCoord) {
        Vector2 corner = GetScreenCorner(screenCoord);
        Vector2 dimensions = GetScreenDimensions();
        
        int chunkSize = GameData.World.Config.ChunkSize;
        
        return new Rect(corner.x, corner.y, dimensions.x, dimensions.y);
    }

    public Rect GetScreenCameraBounds(XY screenCoord) {
        Rect screenBounds = GetScreenBounds(screenCoord);

        // TODO: un-hardcode these?
        float top = 0;
        float bottom = 0;
        float left, right;
        left = right = 0;

        Rect camBounds = new Rect(screenBounds.xMin + left,
                                  screenBounds.yMin + bottom,
                                  screenBounds.width - left - right,
                                  screenBounds.height - top - bottom);

        return camBounds;
    }

    /*
    private ChunkGroup CreateScreenChunks(XY screenCoord, float[,] screenNoise) {
        WorldConfig worldConfig = GameData.World.Config;

        int chunkSize = worldConfig.ChunkSize;
        XYZ screenSize = worldConfig.ScreenSize;
        int screenHeight = worldConfig.ScreenChunks.Y;
        
        Chunk[,,] chunks = new Chunk[screenNoise.GetLength(0), 
                                     screenHeight, 
                                     screenNoise.GetLength(1)];

        Vector2 screenOffset = new Vector2(screenCoord.X * screenSize.X,
                                           screenCoord.Y * screenSize.Z);

        for(int x = 0; x < screenNoise.GetLength(0); x++) {
            for(int z = 0; z < screenNoise.GetLength(1); z++) {
                int y = (int)screenNoise[x, z];

                Vector3 chunkPos = new Vector3(screenOffset.x + x * chunkSize, 
                                               y * chunkSize, 
                                               screenOffset.y + z * chunkSize);
                    
                //GameObject newChunkGo = Instantiate(_world.ChunkPrototype, chunkPos,
                //                                    new Quaternion(0, 0, 0, 0)) as GameObject;
                
                GameObject chunkGo = _chunkPool.GetObject();
                chunkGo.transform.position = chunkPos;
                chunkGo.transform.parent = transform;
                    
                int textureIndex = 12;
                    
                if(y == 0)
                    textureIndex = 14;
                else if(y == 1)
                    textureIndex = 13;
                else if(y == 5)
                    textureIndex = 15;
                else if(y == 6)
                    textureIndex = 8;
                    
                Chunk chunk = chunkGo.GetComponent("Chunk") as Chunk;
                chunk.Initialize(chunkSize, true, GameData.TextureAtlas, textureIndex);

                chunks[x, y, z] = chunk;
            }
        }

        ChunkGroup chunkGroup = new ChunkGroup(chunks);
        
        for(int x = 0; x < screenNoise.GetLength(0); x++) {
            for(int z = 0; z < screenNoise.GetLength(1); z++) {
                int y = (int)screenNoise[x, z];

                Chunk chunk = chunks[x, y, z];

                // Set the variables necessary for mesh generation optimization
                // TODO: This can probably be handled more gracefully
                chunk.chunkGroup = chunkGroup;
                chunk.chunkOffset = new XYZ(x * chunkSize, y * chunkSize, z * chunkSize);

                chunk.GenerateMesh();
            }
        }

        return chunkGroup;
    }
    */
}
