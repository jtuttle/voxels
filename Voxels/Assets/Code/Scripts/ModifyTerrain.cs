using UnityEngine;
using System.Collections;

public class ModifyTerrain : MonoBehaviour {
	public World world;
	public GameObject cameraGO;

	protected void Start() {
		world = gameObject.GetComponent("World") as World;
		cameraGO = GameObject.FindGameObjectWithTag("MainCamera");
	}

	protected void Update() {
		if(Input.GetMouseButtonDown(0)) {
			ReplaceBlockCursor(0);
		}

		if(Input.GetMouseButtonDown(1)) {
			AddBlockCursor(1);
		}
	}

	public void ReplaceBlockCenter(float range, byte block) {
		Ray ray = new Ray(cameraGO.transform.position, cameraGO.transform.forward);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit)) {
			if(hit.distance < range)
				ReplaceBlockAt(hit, block);
		}
	}

	public void AddBlockCenter(float range, byte block) {
		Ray ray = new Ray(cameraGO.transform.position, cameraGO.transform.forward);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit)) {
			if(hit.distance < range)
				ReplaceBlockAt(hit, block);
		}
	}

	public void ReplaceBlockCursor(byte block) {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit)) {
			ReplaceBlockAt(hit, block);
			Debug.DrawLine(ray.origin, ray.origin + (ray.direction * hit.distance), Color.green, 2);
		}
	}

	public void AddBlockCursor(byte block) {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit)) {
			AddBlockAt(hit, block);
			Debug.DrawLine(ray.origin, ray.origin + (ray.direction * hit.distance), Color.green, 2);
		}
	}

	public void ReplaceBlockAt(RaycastHit hit, byte block) {
		Vector3 position = hit.point;
		position += (hit.normal * -0.5f);

		SetBlockAt(position, block);
	}

	public void AddBlockAt(RaycastHit hit, byte block) {
		Vector3 position = hit.point;
		position += (hit.normal * 0.5f);

		SetBlockAt(position, block);
	}

	public void SetBlockAt(Vector3 position, byte block) {
		int x = Mathf.RoundToInt(position.x);
		int y = Mathf.RoundToInt(position.y);
		int z = Mathf.RoundToInt(position.z);

		SetBlockAt(x, y, z, block);
	}

	public void SetBlockAt(int x, int y, int z, byte block) {
		WorldConfig config = world.Config;

		if(x < 0 || x >= config.WorldX || y < 0 || y >= config.WorldY || z < 0 || z >= config.WorldZ)
			return;

		world.data[x, y, z] = block;
		UpdateChunkAt(x, y, z);
	}

	public void UpdateChunkAt(int x, int y, int z) {
		int chunkSize = world.Config.ChunkSize;

		int updateX = Mathf.FloorToInt(x / chunkSize);
		int updateY = Mathf.FloorToInt(y / chunkSize);
		int updateZ = Mathf.FloorToInt(z / chunkSize);

		world.chunks[updateX, updateY, updateZ].dirty = true;

		if(x - (chunkSize * updateX) == 0 && updateX != 0)
			world.chunks[updateX - 1, updateY, updateZ].dirty = true;

		if(x - (chunkSize * updateX) == chunkSize - 1 && updateX != world.chunks.GetLength(0) - 1)
			world.chunks[updateX + 1, updateY, updateZ].dirty = true;

		if(y - (chunkSize * updateY) == 0 && updateY != 0)
			world.chunks[updateX, updateY - 1, updateZ].dirty = true;

		if(y - (chunkSize * updateY) == chunkSize - 1 && updateY != world.chunks.GetLength(1) - 1)
			world.chunks[updateX, updateY + 1, updateZ].dirty = true;

		if(z - (chunkSize * updateZ) == 0 && updateZ != 0)
			world.chunks[updateX, updateY, updateZ - 1].dirty = true;

		if(z - (chunkSize * updateZ) == chunkSize - 1 && updateZ != world.chunks.GetLength(2) - 1)
			world.chunks[updateX, updateY, updateZ + 1].dirty = true;
	}
}
