using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GameObjectPool : MonoBehaviour {
    public GameObject Prefab;
    public int Size = 10;

    private Queue<GameObject> _objects;

    protected void Awake() {
        // Rebuild the child queue when the scene opens.
        if(_objects == null) {
            _objects = new Queue<GameObject>();

            foreach(Transform child in transform)
                _objects.Enqueue(child.gameObject);
        }
    }

    public GameObject GetObject() {
        return _objects.Dequeue();
    }

    public void PutObject(GameObject obj) {
        _objects.Enqueue(obj);
        obj.transform.parent = transform;
    }

    public void CreatePool() {
        if(Prefab == null)
            throw new Exception("Object pool prefab has not been set.");

        if(_objects != null)
            ClearPool();

        _objects = new Queue<GameObject>();

        for(int i = 0; i < Size; i++) {
            GameObject obj = (GameObject)Instantiate(Prefab);
            obj.transform.parent = transform;
            _objects.Enqueue(obj);
        }
    }

    public void ClearPool() {
        while(_objects.Count > 0) {
            GameObject obj = _objects.Dequeue();
            DestroyImmediate(obj);
        }

        _objects.Clear();
    }
}
