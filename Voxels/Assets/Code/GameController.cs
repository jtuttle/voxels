using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	// TEMP
	public CharacterController Player;

    public float Scale;
    public float Depth;
    public float Power;

	protected void Start () {
        new PerlinNoiseTester().CreateTest();

		int worldX = 128;
		int worldY = 16;
		int worldZ = 128;

		WorldConfig worldConfig = new WorldConfig(worldX, worldY, worldZ, 16);

		World world = GameObject.Find("World").GetComponent<World>();
		world.Initialize(worldConfig);
		world.Generate();

		// center the world
		// TODO: this breaks...add the appropriate coordinate translations to fix it?
		// maybe that would be too slow...
		//world.transform.position = new Vector3(-(worldX / 2), -(worldY / 2), -(worldZ / 2));

		GameObject playerGO = GameObject.Find("Player");
		playerGO.transform.position = new Vector3(worldX / 2, worldY / 2 + 5, worldZ / 2);
	}

    private float[] GenerateNoise(int width, int height) {
        float[] samples = new float[width * height];
        
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                samples[y * width + x] = Noise.GetNoise(x, y, 10, Scale, Depth, Power);
            }
        }
        
        return samples;
    }
}
