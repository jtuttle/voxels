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
        GenerateMesh(screenCoord);
    }

    private void GenerateMesh(XY screenCoord) {
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

        _screenMeshes[screenCoord] = dynamicMesh;
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
}
