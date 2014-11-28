using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameObjectPool))]
public class GameObjectPoolEditor : Editor {
    public override void OnInspectorGUI () {
        if(GUILayout.Button("Create Pool"))
            (target as GameObjectPool).CreatePool();

        if(GUILayout.Button("Clear Pool"))
            (target as GameObjectPool).ClearPool();

        base.OnInspectorGUI();
    }
}
