using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: This could be a singleton...

// The WorldScreenManager is responsible for managing the Unity representation
// of WorldScreens. It uses the chunk pool to build and destroy them on demand 
// and provides information about screen dimensions and boundaries.

public class WorldScreenManager : MonoBehaviour {
    private GameObjectPool _chunkPool;

    private Dictionary<XY, ChunkGroup> _screenChunks;

    protected void Awake() {
        _chunkPool = GameObject.Find("ChunkPool").
            GetComponent<GameObjectPool>();

        _screenChunks = new Dictionary<XY, ChunkGroup>();
    }

	public void CreateScreen(XY screenCoord) {
        World world = GameData.World;

        WorldScreen screen = world.GetScreen(screenCoord);
        float[,] screenNoise = world.GetScreenNoise(screen.Coord);

        ChunkGroup screenChunks = CreateScreenChunks(screen.Coord, screenNoise);

        _screenChunks[screen.Coord] = screenChunks;
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
        float top = 40;
        float bottom = 26;
        float left, right;
        left = right = 25;

        Rect blah = new Rect(screenBounds.xMin + left,
                        screenBounds.yMin + bottom,
                        screenBounds.width - left - right,
                        screenBounds.height - top - bottom);

        return blah;
    }

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
}
