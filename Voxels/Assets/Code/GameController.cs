﻿using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	// TEMP
	public CharacterController Player;

	protected void Start () {
		int worldX = 128;
		int worldY = 16;
		int worldZ = 128;

		WorldConfig worldConfig = new WorldConfig(worldX, worldY, worldZ, 8);

		World world = GameObject.Find("World").GetComponent<World>();
		world.Initialize(worldConfig);
		world.Generate();

		// center the world
		world.transform.position = new Vector3(-(worldX / 2), -(worldY / 2), -(worldZ / 2));
	}
}
