using UnityEngine;
using System.Collections;

// The world is split up into chunk groupings that I will call screens, though
// they will often extend over an area that's larger than the player's screen.
public class WorldScreenComponent : MonoBehaviour {
    public Chunk[,,] Chunks { get; private set; }

    public XY Coords { get; private set; }
    public Rect NavBounds { get; private set; }

    private float[,] _samples;
    private WorldComponent _world;

    public int Width { get { return Chunks.GetLength(0); } }
    public int Height { get { return Chunks.GetLength(1); } }

    public void Initialize(float[,] samples, XY coords, WorldComponent world) {
        _samples = samples;
        Coords = coords;
        _world = world;

        float edgeSize = world.Config.ChunkSize / 2;

        // Set navigation bounds at half a chunk.
        NavBounds = world.GetScreenBounds(Coords, edgeSize, edgeSize);
    }

    public byte GetBlock(int x, int y, int z) {
        int chunkSize = _world.Config.ChunkSize;
        
        XYZ chunkCoords = new XYZ((int)Mathf.Floor(x / chunkSize), 
                                  (int)Mathf.Floor(y / chunkSize), 
                                  (int)Mathf.Floor(z / chunkSize));
        
        Chunk chunk = Chunks[chunkCoords.X, chunkCoords.Y, chunkCoords.Z];
        
        return chunk.GetBlock(x % chunkSize, y % chunkSize, z % chunkSize);
    }

    public void CreateChunks() {
        int chunkSize = _world.Config.ChunkSize;
        int screenHeight = _world.Config.ScreenChunks.Y;

        Chunks = new Chunk[_samples.GetLength(0), 
                           screenHeight, 
                           _samples.GetLength(1)];
        
        // coastline test
        /*
        for(int i = 0; i < samples.Length; i++) {
            if(i > 30)
                samples[i] = 1;
            else
                samples[i] = 0;
        }
        */
        
        for(int x = 0; x < _samples.GetLength(0); x++) {
            for(int z = 0; z < _samples.GetLength(1); z++) {
                for(int y = 0; y < screenHeight; y++) {
                    Vector3 chunkPos = new Vector3(x * chunkSize - 0.5f, 
                                                   y * chunkSize + 0.5f, 
                                                   z * chunkSize - 0.5f);

                    //GameObject newChunkGo = Instantiate(_world.ChunkPrototype, chunkPos,
                    //                                    new Quaternion(0, 0, 0, 0)) as GameObject;

                    GameObject newChunkGo = _world.ChunkPool.GetObject();
                    newChunkGo.transform.position = chunkPos;
                    newChunkGo.transform.parent = transform;

                    float sample = _samples[x, z];
                    bool solid = (y <= sample);
                    
                    // TODO - need to raise water by 1 and lower snow by 1
                    // not really sure what this was doing...
                    //if(y == 0)
                    //    solid = (y <= samples[z * Config.ChunkCountX + x] + 1);
                    
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
                    newChunk.Initialize(chunkSize, solid, _world.TextureAtlas, textureIndex);
                    //newChunk.transform.parent = transform;

                    newChunk.chunkGroup = this;
                    newChunk.chunkOffset = new XYZ(x * chunkSize, y * chunkSize, z * chunkSize);

                    Chunks[x, y, z] = newChunk;
                }
            }
        }

        for(int x = 0; x < _samples.GetLength(0); x++) {
            for(int z = 0; z < _samples.GetLength(1); z++) {
                for(int y = 0; y < screenHeight; y++) {
                    Chunks[x, y, z].GenerateMesh();
                }
            }
        }
    }
}
