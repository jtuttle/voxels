using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	protected void Start () {
		WorldConfig worldConfig = new WorldConfig(64, 32, 64, 8);
		World world = GameObject.Find("World").GetComponent<World>();

		world.Initialize(worldConfig);
		world.Generate();
	}
}
