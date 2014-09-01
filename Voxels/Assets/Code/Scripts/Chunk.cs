using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour {
	public GameObject worldGO;
	private World world;

	public int chunkSize;
	public int chunkX;
	public int chunkY;
	public int chunkZ;

	private List<Vector3> newVertices = new List<Vector3>();
	private List<int> newTriangles = new List<int>();
	private List<Vector2> newUV = new List<Vector2>();
	private int faceCount;

	private float tUnit = 0.25f;
	private Vector2 tStone = new Vector2(1, 0);
	private Vector2 tGrass = new Vector2(0, 1);

	private Mesh mesh;
	private MeshCollider col;

	void Start() {
		world = worldGO.GetComponent<World>();

		mesh = GetComponent<MeshFilter>().mesh;
		col = GetComponent<MeshCollider>();

		GenerateMesh();
	}

	public byte Block(int x, int y, int z) {
		return world.Block(x + chunkX, y + chunkY, z + chunkZ);
	}

	private void GenerateMesh() {
		for(int x = 0; x < chunkSize; x++) {
			for(int y = 0; y < chunkSize; y++) {
				for(int z = 0; z < chunkSize; z++) {
					CubeTop(x, y, z, Block(x, y, z));
					CubeBot(x, y, z, Block(x, y, z));
					CubeEast(x, y, z, Block(x, y, z));
					CubeWest(x, y, z, Block(x, y, z));
					CubeNorth(x, y, z, Block(x, y, z));
					CubeSouth(x, y, z, Block(x, y, z));

					/*
					// block is solid
					if(Block(x, y, z) != 0) {
						// block above is air
						if(Block(x, y + 1, z) == 0)
							CubeTop(x, y, z, Block(x, y, z));

						// block below is air
						if(Block(x, y - 1, z) == 0)
							CubeBot(x, y, z, Block(x, y, z));

						// block east is air
						if(Block(x + 1, y, z) == 0)
							CubeEast(x, y, z, Block(x, y, z));

						// block west is air
						if(Block(x - 1, y, z) == 0)
							CubeWest(x, y, z, Block(x, y, z));

						// block north is air
						if(Block(x, y, z + 1) == 0)
							CubeNorth(x, y, z, Block(x, y, z));

						// block south is air
						if(Block(x, y, z - 1) == 0)
							CubeSouth(x, y, z, Block(x, y, z));
					}
				    */
				}
			}
		}
		
		UpdateMesh();
	}

	private void UpdateMesh() {
		mesh.Clear();
		mesh.vertices = newVertices.ToArray();
		mesh.uv = newUV.ToArray();
		mesh.triangles = newTriangles.ToArray();
		mesh.Optimize();
		mesh.RecalculateNormals();

		col.sharedMesh = null;
		col.sharedMesh = mesh;

		newVertices.Clear();
		newUV.Clear();
		newTriangles.Clear();

		faceCount = 0;
	}

	private void CubeTop(int x, int y, int z, byte GetBlock) {
		newVertices.Add(new Vector3(x, y, z + 1));
		newVertices.Add(new Vector3(x + 1, y, z + 1));
		newVertices.Add(new Vector3(x + 1, y, z));
		newVertices.Add(new Vector3(x, y, z));
		Cube(tStone);
	}

	private void CubeNorth(int x, int y, int z, byte GetBlock) {
		newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
		newVertices.Add(new Vector3(x + 1, y, z + 1));
		newVertices.Add(new Vector3(x, y, z + 1));
		newVertices.Add(new Vector3(x, y - 1, z + 1));
		Cube(tStone);
	}

	private void CubeEast(int x, int y, int z, byte GetBlock) {
		newVertices.Add(new Vector3(x + 1, y - 1, z));
		newVertices.Add(new Vector3(x + 1, y, z));
		newVertices.Add(new Vector3(x + 1, y, z + 1));
		newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
		Cube(tStone);
	}

	private void CubeSouth(int x, int y, int z, byte GetBlock) {
		newVertices.Add(new Vector3(x, y - 1, z));
		newVertices.Add(new Vector3(x, y, z));
		newVertices.Add(new Vector3(x + 1, y, z));
		newVertices.Add(new Vector3(x + 1, y - 1, z));
		Cube(tStone);
	}

	private void CubeWest(int x, int y, int z, byte GetBlock) {
		newVertices.Add(new Vector3(x, y - 1, z + 1));
		newVertices.Add(new Vector3(x, y, z + 1));
		newVertices.Add(new Vector3(x, y, z));
		newVertices.Add(new Vector3(x, y - 1, z));
		Cube(tStone);
	}

	private void CubeBot(int x, int y, int z, byte GetBlock) {
		newVertices.Add(new Vector3(x, y - 1, z));
		newVertices.Add(new Vector3(x + 1, y - 1, z));
		newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
		newVertices.Add(new Vector3(x, y - 1, z + 1));
		Cube(tStone);
	}

	private void Cube(Vector2 texturePos) {
		int offset = faceCount * 4;

		newTriangles.Add(offset + 0); //1
		newTriangles.Add(offset + 1); //2
		newTriangles.Add(offset + 2); //3
		newTriangles.Add(offset + 0); //1
		newTriangles.Add(offset + 2); //3
		newTriangles.Add(offset + 3); //4

		newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
		newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
		newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
		newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y));

		faceCount++;
	}
}
