using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour {
	public World world;
	public IntVector3 chunkOffset;

	private bool _dirty = false;

	private Mesh mesh;
	private MeshCollider col;

	// TODO - abstract cube building
	private List<Vector3> newVertices = new List<Vector3>();
	private List<int> newTriangles = new List<int>();
	private List<Vector2> newUV = new List<Vector2>();
	private int faceCount;

	// TODO - abstract texture handling
	private float tUnit = 0.0625f;
	private Vector2 tStone = new Vector2(1, 15);
	private Vector2 tDirt = new Vector2(2, 15);
	private Vector2 tGrassTop = new Vector2(1, 6);
	private Vector2 tGrassSide = new Vector2(3, 15);

	protected void Start() {
		mesh = GetComponent<MeshFilter>().mesh;
		col = GetComponent<MeshCollider>();

		GenerateMesh();
	}

	protected void LateUpdate() {
		if(_dirty) {
			GenerateMesh();
			_dirty = false;
		}
	}

	public byte Block(int x, int y, int z) {
		return world.Block(x + chunkOffset.X, y + chunkOffset.Y, z + chunkOffset.Z);
	}

	public void GenerateMesh() {
		int chunkSize = world.Config.ChunkSize;

		for(int x = 0; x < chunkSize; x++) {
			for(int y = 0; y < chunkSize; y++) {
				for(int z = 0; z < chunkSize; z++) {
					byte block = Block(x, y, z);

					// block is solid
					if(block != 0) {
						// block above is air
						if(Block(x, y + 1, z) == 0)
							CubeTop(x, y, z, block);

						// block below is air
						if(Block(x, y - 1, z) == 0)
							CubeBot(x, y, z, block);

						// block east is air
						if(Block(x + 1, y, z) == 0)
							CubeEast(x, y, z, block);

						// block west is air
						if(Block(x - 1, y, z) == 0)
							CubeWest(x, y, z, block);

						// block north is air
						if(Block(x, y, z + 1) == 0)
							CubeNorth(x, y, z, block);

						// block south is air
						if(Block(x, y, z - 1) == 0)
							CubeSouth(x, y, z, block);
					}
				}
			}
		}
		
		UpdateMesh();
	}

	public void MarkDirty() {
		_dirty = true;
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

	private void CubeTop(int x, int y, int z, byte block) {
		newVertices.Add(new Vector3(x, y, z + 1));
		newVertices.Add(new Vector3(x + 1, y, z + 1));
		newVertices.Add(new Vector3(x + 1, y, z));
		newVertices.Add(new Vector3(x, y, z));

		Vector2 texturePos = new Vector2(0, 0);

		texturePos = tDirt;

		
		if(block == 1)
			texturePos = tStone;
		else if(block == 2)
			texturePos = tGrassTop;
		

		Cube(texturePos);
	}

	private void CubeNorth(int x, int y, int z, byte block) {
		newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
		newVertices.Add(new Vector3(x + 1, y, z + 1));
		newVertices.Add(new Vector3(x, y, z + 1));
		newVertices.Add(new Vector3(x, y - 1, z + 1));
		CubeSide(x, y, z, block);
	}

	private void CubeEast(int x, int y, int z, byte block) {
		newVertices.Add(new Vector3(x + 1, y - 1, z));
		newVertices.Add(new Vector3(x + 1, y, z));
		newVertices.Add(new Vector3(x + 1, y, z + 1));
		newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
		CubeSide(x, y, z, block);
	}

	private void CubeSouth(int x, int y, int z, byte block) {
		newVertices.Add(new Vector3(x, y - 1, z));
		newVertices.Add(new Vector3(x, y, z));
		newVertices.Add(new Vector3(x + 1, y, z));
		newVertices.Add(new Vector3(x + 1, y - 1, z));
		CubeSide(x, y, z, block);
	}

	private void CubeWest(int x, int y, int z, byte block) {
		newVertices.Add(new Vector3(x, y - 1, z + 1));
		newVertices.Add(new Vector3(x, y, z + 1));
		newVertices.Add(new Vector3(x, y, z));
		newVertices.Add(new Vector3(x, y - 1, z));
		CubeSide(x, y, z, block);
	}

	private void CubeBot(int x, int y, int z, byte block) {
		newVertices.Add(new Vector3(x, y - 1, z));
		newVertices.Add(new Vector3(x + 1, y - 1, z));
		newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
		newVertices.Add(new Vector3(x, y - 1, z + 1));

		Vector2 texturePos = new Vector2(0, 0);

		texturePos = tDirt;

		
		if(block == 1)
			texturePos = tStone;
		else if(block == 2)
			texturePos = tDirt;
		

		Cube(texturePos);
	}

	private void CubeSide(int x, int y, int z, byte block) {
		Vector2 texturePos = new Vector2(0, 0);

		texturePos = tDirt;

		
		if(block == 1)
			texturePos = tStone;
		else if(block == 2) {
			if(Block(x, y + 1, z) == 0)
				texturePos = tGrassSide;
			else
				texturePos = tDirt;
		}


		Cube(texturePos);
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
