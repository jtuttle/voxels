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

    private Dictionary<XY, GameObject> _screenMeshes;

    protected void Awake() {
        _chunkPool = GameObject.Find("ChunkPool").
            GetComponent<GameObjectPool>();

        _screenMeshes = new Dictionary<XY, GameObject>();
    }

	public void CreateScreen(XY screenCoord) {
        if(!IsValidScreenCoord(screenCoord)) return;

        GameObject screenMesh = GenerateScreenMesh(screenCoord);

        _screenMeshes[screenCoord] = screenMesh;

        PlaceScreenObjects(screenCoord);
    }

    public void DestroyScreen(XY screenCoord) {
        if(!IsValidScreenCoord(screenCoord)) return;

        Destroy(_screenMeshes[screenCoord]);

        _screenMeshes.Remove(screenCoord);
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

        // TODO: Need to come up with a better way to handle this. It's way
        // too arbitrary and probably won't work for all player elevations.
        float top = 2.5f;
        float bottom = 2.5f;
        float left, right;
        left = right = 2.8f;

        Rect camBounds = new Rect(screenBounds.xMin + left,
                                  screenBounds.yMin + bottom,
                                  screenBounds.width - left - right,
                                  screenBounds.height - top - bottom);

        return camBounds;
    }

    private bool IsValidScreenCoord(XY coord) {
        XY screenCount = GameData.World.Config.ScreenCount;
        return coord.X > 0 && coord.X < screenCount.X
            && coord.Y > 0 && coord.Y < screenCount.Y;
    }

    private GameObject GenerateScreenMesh(XY screenCoord) {
        WorldConfig worldConfig = GameData.World.Config;
        int chunkSize = worldConfig.ChunkSize;
        XYZ screenChunks = worldConfig.ScreenChunks;
        
        TextureAtlas atlas = GameData.TextureAtlas;
        int texIndex = 12; // TEMP
        
        float[,] worldNoise = GameData.World.Noise;
        
        CubeMesh mesh = new CubeMesh(chunkSize, atlas, texIndex);
        CubeMesh cMesh = new CubeMesh(1, null, 0);
        
        for(int sx = 0; sx < screenChunks.X; sx++) {
            for(int sz = 0; sz < screenChunks.Z; sz++) {
                // Calculate world coords. Since we have to check neighbors
                // across screen boundaries, screen coords will not suffice.
                int wx = screenCoord.X * screenChunks.X + sx;
                int wz = screenCoord.Y * screenChunks.Z + sz;
                
                float sample = (int)worldNoise[wx, wz];
                
                mesh.AddTopFace(sx, sample, sz);
                cMesh.AddTopFace(sx, sample, sz);
                
                // north face 
                if(wz < worldNoise.GetLength(1) - 1 && worldNoise[wx, wz + 1] < sample) {
                    mesh.AddNorthFace(sx, sample, sz);
                    cMesh.AddNorthFace(sx, sample, sz);
                }
                
                // east face 
                if(wx < worldNoise.GetLength(0) - 1 && worldNoise[wx + 1, wz] < sample) {
                    mesh.AddEastFace(sx, sample, sz);
                    cMesh.AddEastFace(sx, sample, sz);
                }
                
                // south face 
                if(wz > 0 && worldNoise[wx, wz - 1] < sample) {
                    mesh.AddSouthFace(sx, sample, sz);
                    cMesh.AddSouthFace(sx, sample, sz);
                }
                
                // west face
                if(wx > 0 && worldNoise[wx - 1, wz] < sample) {
                    mesh.AddWestFace(sx, sample, sz);
                    cMesh.AddWestFace(sx, sample, sz);
                }
            }
        }
        
        Vector3 screenPos = new Vector3(screenCoord.X * screenChunks.X,
                                        0,
                                        screenCoord.Y * screenChunks.Z);
        
        GameObject dynamicMesh = (GameObject)Instantiate(DynamicMeshPrototype, 
                                                         screenPos, 
                                                         Quaternion.identity);
        dynamicMesh.transform.parent = transform;
        
        dynamicMesh.GetComponent<MeshFilter>().mesh = mesh.GetMesh();
        dynamicMesh.GetComponent<MeshCollider>().sharedMesh = cMesh.GetMesh();
        
        return dynamicMesh;
    }

    private void PlaceScreenObjects(XY screenCoord) {
        WorldScreen screen = GameData.World.GetScreen(screenCoord);

        GameObject treePrototype = Resources.Load("Prefabs/Tree") as GameObject;

        Vector2 screenCorner = GetScreenCorner(screenCoord);
        int chunkSize = 1;

        foreach(Room room in screen.Rooms) {
            foreach(XY coord in room.Perimeter) {
                float x = screenCorner.x + coord.X * chunkSize;
                float y = room.Elevation;
                float z = screenCorner.y + coord.Y * chunkSize;

                GameObject tree = (GameObject)Instantiate(treePrototype);
                tree.transform.position = new Vector3(x, y, z);
            }
        }
    }
}
