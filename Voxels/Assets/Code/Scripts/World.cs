using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	public WorldConfig Config { get; private set; }

	//public byte[,,] data;

	public GameObject chunk;
	public Chunk[,,] chunks;

	public void Initialize(WorldConfig config) {
		Config = config;
	}

    public void Generate(float[] samples) {
		//CreateBlocks();
        CreateChunks(samples);
	}

    /*
	public byte Block(int x, int y, int z) {
		// avoid hiding tops of blocks at top of map
		if(y >= Config.WorldY) return (byte)0;

		if(x >= Config.WorldX || x < 0 || y < 0 || z >= Config.WorldZ || z < 0)
			return (byte)1;

		return data[x, y, z];
	}
    */   

	private void CreateBlocks() {
		int worldX = Config.WorldX;
		int worldY = Config.WorldY;
		int worldZ = Config.WorldZ;

		//data = new byte[worldX, worldY, worldZ];

        /*
        for(int x = 0; x < Config.WorldX; x++) {
            for(int z = 0; z < Config.WorldZ; z++) {
                float sample = (float)Noise.GetNoise(x, 100, z, 100.0f, 1.4f, 8.0f);

                for(int y = 0; y < Config.WorldY; y++) {
                    if(sample < 0.5f)
                        data[x, y, z] = 0;
                    else
                        data[x, y, z] = 1;
                }
            }
        }
        */

        /*
		for(int x = 0; x < worldX; x++) {
			for(int z = 0; z < worldZ; z++) {
				for(int y = 0; y < worldY; y++) {
					if(x < worldX / 3 || x > worldX - (worldX / 3) || z < worldZ / 3 || z > worldZ - (worldZ / 3) || y < 1)
						data[x, y, z] = 1;
				}
			}
		}
        */      
		
		// Nice-looking Minecraft-y map
        /*
		for(int x = 0; x < Config.WorldX; x++) {
		    for(int z = 0; z < Config.WorldZ; z++) {
		        int stone = (int)Noise.GetNoise(x, 0, z, 10, 3, 1.2f);
                stone += (int)Noise.GetNoise(x, 300, z, 20, 4, 0) + 10;

                int dirt = (int)Noise.GetNoise(x, 100, z, 50, 2, 0) + 1;

		        for(int y = 0; y < Config.WorldY; y++) {
		            if(y <= stone)
		                data[x, y, z] = 1;
		            else if(y <= stone + dirt)
		                data[x, y, z] = 2;
		        }
		    }
		}
        */      
	}

	private void CreateChunks(float[] samples) {
		int chunkSize = Config.ChunkSize;

		chunks = new Chunk[
			Mathf.FloorToInt(Config.WorldX),
			Mathf.FloorToInt(Config.WorldY),
			Mathf.FloorToInt(Config.WorldZ)
		];

		for(int x = 0; x < chunks.GetLength(0); x++) {
            for(int z = 0; z < chunks.GetLength(2); z++) {
			    for(int y = 0; y < chunks.GetLength(1); y++) {
					GameObject newChunkGo = Instantiate(chunk,
						new Vector3(x * chunkSize - 0.5f, y * chunkSize + 0.5f, z * chunkSize - 0.5f),
						new Quaternion(0, 0, 0, 0)) as GameObject;

                    bool solid = (y <= samples[z * Config.WorldX + x]);

					Chunk newChunk = newChunkGo.GetComponent("Chunk") as Chunk;
                    newChunk.Initialize(Config.ChunkSize, solid);
					newChunk.transform.parent = transform;

					newChunk.world = this;
					newChunk.chunkOffset = new IntVector3(x * chunkSize, y * chunkSize, z * chunkSize);

					chunks[x, y, z] = newChunk;
				}
			}
		}
	}
}
