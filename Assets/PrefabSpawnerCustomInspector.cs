using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabSpawner))]
public class PrefabSpawnerCustomInspector : Editor {

    public override void OnInspectorGUI()
    {
        PrefabSpawner script = (PrefabSpawner)target;

        base.OnInspectorGUI();

        //script.prefab = (GameObject)EditorGUILayout.ObjectField(script.prefab, typeof(GameObject));
        //script.spawnCount = EditorGUILayout.IntField("Spawn Count", script.spawnCount);
        //script.relativeOffset = EditorGUILayout.Vector3Field("Relative Offset", script.relativeOffset);

        if (GUILayout.Button("Spawn"))
        {
            script.SpawnObjects(script.prefab, script.spawnCount, script.offset);
        }
    }

}
