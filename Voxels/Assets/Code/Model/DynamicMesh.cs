using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicMesh {
    protected List<Vector3> _verts;
    protected List<int> _tris;
    protected List<Vector2> _uvs;

    protected int _faceCount;

	public DynamicMesh() {
        _verts = new List<Vector3>();
        _tris = new List<int>();
        _uvs = new List<Vector2>();

        _faceCount = 0;
    }

    public virtual Mesh GetMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = _verts.ToArray();
        mesh.triangles = _tris.ToArray();
        mesh.uv = _uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
        
        return mesh;
    }

    public void AddVertex(int x, int y, int z) {
        _verts.Add(new Vector3(x, y, z));
    }
}
