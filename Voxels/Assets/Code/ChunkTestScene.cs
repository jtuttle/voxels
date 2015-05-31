using UnityEngine;
using System.Collections;

public class ChunkTestScene : MonoBehaviour {
    public GameObject DynamicMeshPrototype;

    private XYZ chunkSize = new XYZ(8, 8, 8);

    void Awake() {
        byte[,,] template = GetStairsTemplate();

        NewChunkGroup group = new NewChunkGroup(chunkSize);

        IChunk[,,] chunks = new IChunk[4,1,4];

        for(int x = 0; x < chunks.GetLength(0); x++) {
            for(int z = 0; z < chunks.GetLength(2); z++) {
                for(int y = 0; y < chunks.GetLength(1); y++) {
                    chunks[x,y,z] = new FixedChunk(template);
                }
            }
        }

        group.Chunks = chunks;

        ChunkGroupMesh chunkGroupMesh = new ChunkGroupMesh(group);

        GameObject dynamicMeshGo = (GameObject)Instantiate(DynamicMeshPrototype, 
                                                         new Vector3(), 
                                                         Quaternion.identity);
        dynamicMeshGo.transform.parent = transform;
        
        dynamicMeshGo.GetComponent<MeshFilter>().mesh = chunkGroupMesh.GetMesh();
        dynamicMeshGo.GetComponent<MeshCollider>().sharedMesh = chunkGroupMesh.GetMesh();
    }

    private byte[,,] GetStairsTemplate() {
        byte[,,] template = new byte[chunkSize.X, chunkSize.Y, chunkSize.Z];

        for(int x = 0; x < chunkSize.X; x++) {
            for(int z = 0; z < chunkSize.Z; z++) {
                for(int y = 0; y < chunkSize.Y; y++) {
                    template[x, y, z] = 
                        (byte)(z + y < chunkSize.Z ? 1 : 0);
                }
            }
        }

        return template;
    }
}
