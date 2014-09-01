using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManualMesh : MonoBehaviour {

	public List<Vector3> newVertices = new List<Vector3>();
	public List<int> newTriangles = new List<int>();
	public List<Vector2> newUV = new List<Vector2>();

	private Mesh _mesh;

	private float tUnit = 0.25f;
	private Vector2 tStone = new Vector2(0, 0);
	private Vector2 tGrass = new Vector2(0, 1);

	private int _squareCount = 0;

	private byte[,] _blocks;

	protected void Awake() {
		_mesh = GetComponent<MeshFilter>().mesh;
	}

	protected void Start () {
		GenerateTerrain();
		BuildMesh();
		UpdateMesh();
	}

	private void GenerateTerrain() {
		_blocks = new byte[10, 10];

		for(int x = 0; x < _blocks.GetLength(0); x++) {
			for(int y = 0; y < _blocks.GetLength(1); y++) {
				if(y == 5) {
					_blocks[x, y] = 2;
				} else {
					_blocks[x, y] = 1;
				}
			}
		}
	}

	private void BuildMesh() {
		for(int x = 0; x < _blocks.GetLength(0); x++) {
			for(int y = 0; y < _blocks.GetLength(1); y++) {
				if(_blocks[x, y] == 1) {
					GenerateSquare(x, y, 0, tStone);
				} else if(_blocks[x, y] == 2) {
					GenerateSquare(x, y, 0, tGrass);
				}
			}
		}
	}

	private void GenerateSquare(int x, int y, int z, Vector2 texture) {
		newVertices.Add(new Vector3(x, y, z));
		newVertices.Add(new Vector3(x + 1, y, z));
		newVertices.Add(new Vector3(x + 1, y - 1, z));
		newVertices.Add(new Vector3(x, y - 1, z));

		int offset = _squareCount * 4;

		newTriangles.Add(offset + 0);
		newTriangles.Add(offset + 1);
		newTriangles.Add(offset + 3);
		newTriangles.Add(offset + 1);
		newTriangles.Add(offset + 2);
		newTriangles.Add(offset + 3);

		newUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y + tUnit));
		newUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y + tUnit));
		newUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y));
		newUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y));

		_squareCount++;
	}

	private void UpdateMesh() {
		_mesh.Clear();
		_mesh.vertices = newVertices.ToArray();
		_mesh.triangles = newTriangles.ToArray();
		_mesh.uv = newUV.ToArray();
		_mesh.Optimize();
		_mesh.RecalculateNormals();

		_squareCount = 0;
		newVertices.Clear();
		newTriangles.Clear();
		newUV.Clear();
	}
}
