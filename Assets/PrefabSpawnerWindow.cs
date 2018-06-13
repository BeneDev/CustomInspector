using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class PrefabSpawnerWindow : EditorWindow {

    #region Private Fields

    GameObject prefab;
    Transform parentTransform;

    bool keepPrefabLink = true;

    int amount;
    Vector3 offset;

    #endregion

    [MenuItem("Tools/PrefabSpawner %#Ö")]
	static void Init()
    {
        PrefabSpawnerWindow window = GetWindow<PrefabSpawnerWindow>();
        window.Show();
    }

    // For packing in fields and 
    private void OnGUI()
    {
        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false) as GameObject;
        parentTransform = EditorGUILayout.ObjectField("Parent", parentTransform, typeof(Transform), true) as Transform;

        keepPrefabLink = EditorGUILayout.Toggle("Keep Prefab Link", keepPrefabLink);
        
        amount = EditorGUILayout.IntField("SpawnAmount", amount);
        offset = EditorGUILayout.Vector3Field("Offset", offset);

        // Geschweifte Klammern haben keine syntaktische wirkung, falls nicht vorgegeben. Hier rücken sie nur ein
        EditorGUI.BeginDisabledGroup(!prefab || !parentTransform);
        {
            if (GUILayout.Button("Spawn"))
            {
                InstantiateObjects(prefab, amount, offset, parentTransform, keepPrefabLink);
            }
        }
        EditorGUI.EndDisabledGroup();
        EditorGUI.BeginDisabledGroup(!parentTransform);
        {
            if (GUILayout.Button("Delete Objects"))
            {
                DestroyObjects(parentTransform);
            }
        }
        EditorGUI.EndDisabledGroup();

    }

    private static void DestroyObjects(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);
        }
    }

    private static void InstantiateObjects(GameObject prefab, int amount, Vector3 offset, Transform parent = null, bool keepPrefabLink = true)
    {
        GameObject obj;
        if (keepPrefabLink)
        {
            for (int i = 0; i < amount; i++)
            {
                obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                obj.transform.parent = parent;
                obj.transform.localPosition = offset * i;
                Undo.RegisterCreatedObjectUndo(obj, "InstantiatePrefabs");
            }
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                obj = Instantiate(prefab, offset * i, Quaternion.identity, parent);
                Undo.RegisterCreatedObjectUndo(obj, "InstantiatePrefabs");
            }
        }
    }
}
