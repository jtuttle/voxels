using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMesh {
    private int _faceSections;
    private TextureAtlas _atlas;
    private int _texIndex;

    private float _faceSectionSize;

    private List<Vector3> _verts;
    private List<int> _tris;
    private List<Vector2> _uvs;

    private int _offset;

    public CubeMesh(int faceSections, TextureAtlas atlas, int texIndex) {
        _faceSections = faceSections;
        _atlas = atlas;
        _texIndex = texIndex;

        _faceSectionSize = 1.0f / _faceSections;

        _verts = new List<Vector3>();
        _tris = new List<int>();
        _uvs = new List<Vector2>();

        _offset = 0;
    }

    public void AddTopFace(float cx, float cy, float cz) {
        for(int fx = 0; fx < _faceSections; fx++) {
            for(int fz = 0; fz < _faceSections; fz++) {
                float x = cx + fx * _faceSectionSize;
                float y = cy;
                float z = cz + fz * _faceSectionSize;
                
                CubeTop(x, y, z, _faceSectionSize);
                CubeFaceTris();

                if(_atlas != null)
                    _uvs.AddRange(_atlas.getUVCoords(_texIndex));
            }
        }
    }

    public void AddNorthFace(float cx, float cy, float cz) {
        float z = cz + _faceSectionSize * (_faceSections - 1);
        
        for(int fx = 0; fx < _faceSections; fx++) {
            for(int fy = 0; fy < _faceSections; fy++) {
                float x = cx + fx * _faceSectionSize;
                float y = cy - fy * _faceSectionSize;
                
                CubeNorth(x, y, z, _faceSectionSize);
                CubeFaceTris();

                if(_atlas != null)
                    _uvs.AddRange(_atlas.getUVCoords(_texIndex));
            }
        }
    }

    public void AddEastFace(float cx, float cy, float cz) {
        float x = cx + _faceSectionSize * (_faceSections - 1);
        
        for(int fz = 0; fz < _faceSections; fz++) {
            for(int fy = 0; fy < _faceSections; fy++) {
                float y = cy - fy * _faceSectionSize;
                float z = cz + fz * _faceSectionSize;
                
                CubeEast(x, y, z, _faceSectionSize);
                CubeFaceTris();

                if(_atlas != null)
                    _uvs.AddRange(_atlas.getUVCoords(_texIndex));
            }
        }
    }

    public void AddSouthFace(float cx, float cy, float cz) {
        for(int fx = 0; fx < _faceSections; fx++) {
            for(int fy = 0; fy < _faceSections; fy++) {
                float x = cx + fx * _faceSectionSize;
                float y = cy - fy * _faceSectionSize;
                float z = cz;
                
                CubeSouth(x, y, z, _faceSectionSize);
                CubeFaceTris();

                if(_atlas != null)
                    _uvs.AddRange(_atlas.getUVCoords(_texIndex));
            }
        }
    }

    public void AddWestFace(float cx, float cy, float cz) {
        for(int fz = 0; fz < _faceSections; fz++) {
            for(int fy = 0; fy < _faceSections; fy++) {
                float x = cx;
                float y = cy - fy * _faceSectionSize;
                float z = cz + fz * _faceSectionSize;
                
                CubeWest(x, y, z, _faceSectionSize);
                CubeFaceTris();

                if(_atlas != null)
                    _uvs.AddRange(_atlas.getUVCoords(_texIndex));
            }
        }
    }

    public Mesh GetMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = _verts.ToArray();
        mesh.triangles = _tris.ToArray();
        mesh.uv = _uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        return mesh;
    }

    private void CubeTop(float x, float y, float z, float size) {
        _verts.Add(new Vector3(x, y, z + size));
        _verts.Add(new Vector3(x + size, y, z + size));
        _verts.Add(new Vector3(x + size, y, z));
        _verts.Add(new Vector3(x, y, z));
    }
    
    private void CubeNorth(float x, float y, float z, float size) {
        _verts.Add(new Vector3(x + size, y - size, z + size));
        _verts.Add(new Vector3(x + size, y, z + size));
        _verts.Add(new Vector3(x, y, z + size));
        _verts.Add(new Vector3(x, y - size, z + size));
    }
    
    private void CubeEast(float x, float y, float z, float size) {
        _verts.Add(new Vector3(x + size, y - size, z));
        _verts.Add(new Vector3(x + size, y, z));
        _verts.Add(new Vector3(x + size, y, z + size));
        _verts.Add(new Vector3(x + size, y - size, z + size));
    }
    
    private void CubeSouth(float x, float y, float z, float size) {
        _verts.Add(new Vector3(x, y - size, z));
        _verts.Add(new Vector3(x, y, z));
        _verts.Add(new Vector3(x + size, y, z));
        _verts.Add(new Vector3(x + size, y - size, z));
    }
    
    private void CubeWest(float x, float y, float z, float size) {
        _verts.Add(new Vector3(x, y - size, z + size));
        _verts.Add(new Vector3(x, y, z + size));
        _verts.Add(new Vector3(x, y, z));
        _verts.Add(new Vector3(x, y - size, z));
    }
    
    private void CubeBot(float x, float y, float z, float size) {
        _verts.Add(new Vector3(x, y - size, z));
        _verts.Add(new Vector3(x + size, y - size, z));
        _verts.Add(new Vector3(x + size, y - size, z + size));
        _verts.Add(new Vector3(x, y - size, z + size));
    }
    
    private void CubeFaceTris() {
        _tris.Add(_offset + 0); //1
        _tris.Add(_offset + 1); //2
        _tris.Add(_offset + 2); //3
        _tris.Add(_offset + 0); //1
        _tris.Add(_offset + 2); //3
        _tris.Add(_offset + 3); //4

        _offset += 4;
    }
}
