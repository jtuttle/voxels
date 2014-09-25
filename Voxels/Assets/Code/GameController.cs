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
        int chunkCountX = 16;
		int chunkCountY = 7;
		int chunkCountZ = 16;

        WorldGenerator worldGen = new WorldGenerator(Random.Range(1, 65536));

		WorldConfig worldConfig = new WorldConfig(chunkCountX, chunkCountY, chunkCountZ, 8);

		World world = GameObject.Find("World").GetComponent<World>();
		world.Initialize(worldConfig);

        float[] rawNoise = worldGen.GenerateRawNoise(chunkCountX, chunkCountZ);
        float[] shiftedNoise = worldGen.ShiftNoise(0, 1, 0, chunkCountY, rawNoise);
        float[] discreteNoise = worldGen.DiscretizeDenormalizedNoise(shiftedNoise);

        world.Generate(discreteNoise);




		// center the world
		// TODO: this breaks...add the appropriate coordinate translations to fix it?
		// maybe that would be too slow...
		//world.transform.position = new Vector3(-(worldX / 2), -(worldY / 2), -(worldZ / 2));

		//GameObject playerGO = GameObject.Find("Player");
		//playerGO.transform.position = new Vector3(worldX / 2, worldY / 2 + 5, worldZ / 2);
	}
}
