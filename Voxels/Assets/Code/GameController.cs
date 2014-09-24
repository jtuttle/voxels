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
        int worldX = 2;
		int worldY = 7;
		int worldZ = 2;

        WorldGenerator worldGen = new WorldGenerator(worldX, worldZ, worldY, Random.Range(1, 65536));

		WorldConfig worldConfig = new WorldConfig(worldX, worldY, worldZ, 8);

		World world = GameObject.Find("World").GetComponent<World>();
		world.Initialize(worldConfig);
        world.Generate(worldGen.DiscreteSamples);




		// center the world
		// TODO: this breaks...add the appropriate coordinate translations to fix it?
		// maybe that would be too slow...
		//world.transform.position = new Vector3(-(worldX / 2), -(worldY / 2), -(worldZ / 2));

		//GameObject playerGO = GameObject.Find("Player");
		//playerGO.transform.position = new Vector3(worldX / 2, worldY / 2 + 5, worldZ / 2);
	}

    /*
    private float[] GenerateNoise(int width, int height) {
        float[] samples = new float[width * height];

        //Generator generator = new ValueNoise(Random.Range(1, 65536), SCurve.Cubic);
        Generator generator = new PinkNoise(Random.Range(1, 65536));

        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                //samples[y * width + x] = Noise.GetNoise(x, y, 10, Scale, Depth, Power);

                float xCoord = (((float)x / width) + 0) * Scale;
                float yCoord = (((float)y / height) + 0) * Scale;
                
                float sample = generator.GetValue(xCoord, yCoord, 0);
                //Debug.Log("initial sample: " + sample);
                
                // Pink noise will mostly fall in [-1,1] but this is not guaranteed
                sample = Mathf.Clamp(sample, -1, 1);
                //Debug.Log("clamped: " + sample);
                
                // Shift sample value to the [0,1] range
                sample = MathUtils.ConvertRange(-1, 1, 0, 1, sample);
                //Debug.Log("normalized: " + sample);
                
                samples[y * width + x] = sample;
            }
        }
        
        return samples;
    }
    */
}
