using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: This can be a singleton

// The WorldScreenManager component handles WorldScreens as they appear in Unity. It
// is responsible for using the chunk pool to build and destroy them on demand and 
// providing information about screen dimensions and boundaries.

public class WorldScreenManager : MonoBehaviour {
    private GameObjectPool _chunkPool;

    private Dictionary<XY, ChunkGroup> _screenChunks;

    protected void Awake() {
        _chunkPool = GameObject.Find("ChunkPool").GetComponent<GameObjectPool>();

        _screenChunks = new Dictionary<XY, ChunkGroup>();
    }

	public void CreateScreen(WorldScreen screen) {
        float[,] screenNoise = GameData.World.GetScreenNoise(screen.Coord);

        ChunkGroup screenChunks = CreateScreenChunks(screen.Coord, screenNoise);

        _screenChunks[screen.Coord] = screenChunks;
    }

    public void DestroyScreen(XY coord) {

    }

    public Vector2 GetScreenDimensions() {
        XYZ screenSize = GameData.World.Config.ScreenSize;
        return new Vector2(screenSize.X, screenSize.Z);
    }
    
    public Vector2 GetScreenCorner(XY screenCoords) {
        XYZ screenSize = GameData.World.Config.ScreenSize;
        
        return new Vector2((float)screenCoords.X * screenSize.X,
                           (float)screenCoords.Y * screenSize.Z);
    }
    
    public Vector2 GetScreenCenter(XY screenCoords) {
        XYZ screenSize = GameData.World.Config.ScreenSize;
        
        Vector2 corner = GetScreenCorner(screenCoords);
        
        return corner + new Vector2(screenSize.X / 2, screenSize.Z / 2);
    }
    
    // The edge size is pretty arbitrary and depends on camera angle/distance.
    // 13 works well for an angle/distance of 40/40.
    public Rect GetScreenBounds(XY screenCoords, float horizontalEdge, float verticalEdge) {
        Vector2 corner = GetScreenCorner(screenCoords);
        Vector2 dimensions = GetScreenDimensions();
        
        int chunkSize = GameData.World.Config.ChunkSize;
        
        return new Rect(corner.x + horizontalEdge,
                        corner.y + verticalEdge,
                        dimensions.x - horizontalEdge * 2,
                        dimensions.y - verticalEdge * 2);
    }

    private ChunkGroup CreateScreenChunks(XY screenCoord, float[,] screenNoise) {
        WorldConfig worldConfig = GameData.World.Config;

        int chunkSize = worldConfig.ChunkSize;
        int screenHeight = worldConfig.ScreenChunks.Y;
        
        Chunk[,,] chunks = new Chunk[screenNoise.GetLength(0), 
                                     screenHeight, 
                                     screenNoise.GetLength(1)];

        Vector2 screenOffset = new Vector2(screenCoord.X * worldConfig.ScreenSize.X,
                                           screenCoord.Y * worldConfig.ScreenSize.Z);

        for(int x = 0; x < screenNoise.GetLength(0); x++) {
            for(int z = 0; z < screenNoise.GetLength(1); z++) {
                int y = (int)screenNoise[x, z];

                Vector3 chunkPos = new Vector3(screenOffset.x + (x * chunkSize - 0.5f), 
                                               y * chunkSize + 0.5f, 
                                               screenOffset.y + (z * chunkSize - 0.5f));
                    
                //GameObject newChunkGo = Instantiate(_world.ChunkPrototype, chunkPos,
                //                                    new Quaternion(0, 0, 0, 0)) as GameObject;
                
                GameObject newChunkGo = _chunkPool.GetObject();
                newChunkGo.transform.position = chunkPos;
                newChunkGo.transform.parent = transform;
                    
                int textureIndex = 12;
                    
                if(y == 0)
                    textureIndex = 14;
                else if(y == 1)
                    textureIndex = 13;
                else if(y == 5)
                    textureIndex = 15;
                else if(y == 6)
                    textureIndex = 8;
                    
                Chunk newChunk = newChunkGo.GetComponent("Chunk") as Chunk;
                newChunk.Initialize(chunkSize, true, GameData.TextureAtlas, textureIndex);

                chunks[x, y, z] = newChunk;
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
}
