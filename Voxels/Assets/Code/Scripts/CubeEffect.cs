using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeEffect : MonoBehaviour {
    public Player Player { get; set; }

    private List<GameObject> _cubes;

    private float _time;
    private float _elapsedTime;

    private bool _spawned = false;

    public void Spawn(Color color, float time) {
        _time = time;

        _cubes = new List<GameObject>();
        
        GameObject cube = (GameObject)Resources.Load("Prefabs/PhysicsCube");
        
        for(int i = 0; i < 20; i++) {
            GameObject newCube = (GameObject)Instantiate(cube);
            newCube.transform.parent = transform;
            newCube.transform.localPosition = 
                Vector3.zero + new Vector3(Random.Range(-1, 1), 
                                           Random.Range(-1, 1), 
                                           Random.Range(-1, 1));
            newCube.renderer.material.color = color;

            _cubes.Add(newCube);
        }

        _spawned = true;
    }

    protected void Update() {
        if(_spawned) {
            _elapsedTime += Time.deltaTime;

            if(_elapsedTime > _time) {
                foreach(GameObject cube in _cubes)
                    Destroy(cube);
                Destroy(gameObject);
            }
        }
    }
}
