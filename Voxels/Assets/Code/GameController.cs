using CoherentNoise;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Fractal;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour {
	// TEMP
	public CharacterController Player;

    public float Scale;
    public float Depth;
    public float Power;

	protected void Start () {
        // each loaded screen could be 16 x 12 chunks
        // world can be 16 x 16 screens

        int chunkCountX = 256;
		int chunkCountY = 7;
		int chunkCountZ = 192;
        int chunkSize = 8;
        IntVector2 chunkGroupSize = new IntVector2(16, 12);

        WorldConfig worldConfig = new WorldConfig(chunkCountX, chunkCountY, chunkCountZ, 
                                                  chunkSize, chunkGroupSize);

        WorldGenerator worldGen = new WorldGenerator(Random.Range(1, 65536));

        float[] rawNoise = worldGen.GenerateRawNoise(chunkCountX, chunkCountZ);
        float[] shiftedNoise = worldGen.ShiftNoise(0, 1, 0, chunkCountY, rawNoise);
        float[] discreteNoise = worldGen.DiscretizeDenormalizedNoise(shiftedNoise);

        World world = GameObject.Find("World").GetComponent<World>();
        world.Initialize(worldConfig);


        // quick test of chunk groups
        float[,] samples = new float[16, 12];
        Vector2 offset = new Vector2(4, 4);

        for(int x = 0; x < samples.GetLength(0); x++) {
            for(int y = 0; y < samples.GetLength(1); y++) {
                samples[x, y] = discreteNoise[y * world.Config.ChunkCountX + x];
            }
        }

        world.CreateChunkGroup(samples, new Vector2(0, 0));
        world.CreateChunkGroup(samples, new Vector2(200, 0));
	}
}
