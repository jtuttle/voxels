using UnityEngine;
using System.Collections;

public class ModifyTerrain : MonoBehaviour {
    public WorldComponent world;
    public GameObject cameraGO;

    protected void Start() {
        world = gameObject.GetComponent("World") as WorldComponent;
        cameraGO = GameObject.FindGameObjectWithTag("MainCamera");
    }

    protected void Update() {
        /*
        if(Input.GetMouseButtonDown(0)) {
            ReplaceBlockCursor(0);
        }

        if(Input.GetMouseButtonDown(1)) {
            AddBlockCursor(1);
        }
        */
    }

    public void ReplaceRectangularBox(Vector3 center, Vector3 edgeDistances, byte block) {
        Vector3 min = center - edgeDistances;
        Vector3 max = center + edgeDistances;

        for(int x = (int)min.x; x < max.x; x++) {
            for(int y = (int)min.y; y < max.y; y++) {
                for(int z = (int)min.z; z < max.z; z++) {
                    SetBlockAt(new Vector3(x, y, z), block);
                }
            }
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
            //ReplaceBlockAt(hit, block);
            ReplaceRectangularBox(hit.point, new Vector3(3, 3, 3), block);
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
        XYZ worldChunks = world.Config.WorldChunks;

        if(x < 0 || x >= worldChunks.X || y < 0 || y >= worldChunks.Y || z < 0 || z >= worldChunks.Z)
            return;

        // TODO - replace this (might have been done?)
//        world.data[x, y, z] = block;

        //UpdateChunkAt(x, y, z);
    }

    /* This became broken when I started using chunkgroups. Needs fixing.
    public void UpdateChunkAt(int x, int y, int z) {
        int chunkSize = world.Config.ChunkSize;

        int updateX = Mathf.FloorToInt(x / chunkSize);
        int updateY = Mathf.FloorToInt(y / chunkSize);
        int updateZ = Mathf.FloorToInt(z / chunkSize);

        world.Chunks[updateX, updateY, updateZ].MarkDirty();

        // Update neighboring chunks when necessary
        if(x - (chunkSize * updateX) == 0 && updateX != 0)
            world.Chunks[updateX - 1, updateY, updateZ].MarkDirty();

        if(x - (chunkSize * updateX) == chunkSize - 1 && updateX != world.Chunks.GetLength(0) - 1)
            world.Chunks[updateX + 1, updateY, updateZ].MarkDirty();

        if(y - (chunkSize * updateY) == 0 && updateY != 0)
            world.Chunks[updateX, updateY - 1, updateZ].MarkDirty();

        if(y - (chunkSize * updateY) == chunkSize - 1 && updateY != world.Chunks.GetLength(1) - 1)
            world.Chunks[updateX, updateY + 1, updateZ].MarkDirty();

        if(z - (chunkSize * updateZ) == 0 && updateZ != 0)
            world.Chunks[updateX, updateY, updateZ - 1].MarkDirty();

        if(z - (chunkSize * updateZ) == chunkSize - 1 && updateZ != world.Chunks.GetLength(2) - 1)
            world.Chunks[updateX, updateY, updateZ + 1].MarkDirty();
    }
    */   
}
