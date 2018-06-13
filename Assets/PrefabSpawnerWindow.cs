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
    bool relativeRotation;
    Vector3 offset;
    Vector3 rotation;

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
        if(amount < 1)
        {
            amount = 1;
        }

        offset = EditorGUILayout.Vector3Field("Offset", offset);

        rotation = EditorGUILayout.Vector3Field("Rotation", rotation);
        relativeRotation = EditorGUILayout.Toggle("Relative Rotation", relativeRotation);

        // Geschweifte Klammern haben keine syntaktische wirkung, falls nicht vorgegeben. Hier rücken sie nur ein
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUI.BeginDisabledGroup(!parentTransform);
            {
                if (GUILayout.Button("Delete Objects"))
                {
                    DestroyObjects(parentTransform);
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(!prefab || !parentTransform);
            {
                if (GUILayout.Button("Spawn"))
                {
                    InstantiateObjects(prefab, amount, offset, rotation, relativeRotation, parentTransform, keepPrefabLink);
                }
            }
            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.EndHorizontal();

    }

    private static void DestroyObjects(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);
        }
    }

    private static void InstantiateObjects(GameObject prefab, int amount, Vector3 offset, Vector3 rotation, bool relativeRotation = false, Transform parent = null, bool keepPrefabLink = true)
    {
        GameObject obj;
        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        if (keepPrefabLink)
        {
            for (int i = 0; i < amount; i++)
            {
                rot = Quaternion.Euler(rotation * i);
                if (relativeRotation)
                {
                    // Rotate the offset vector with the rot quaternion
                    pos = pos + (rot * offset);
                }
                else
                {
                    pos = offset * i;
                }
                obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                obj.transform.parent = parent;
                obj.transform.localPosition = pos;
                obj.transform.localRotation = rot;
                Undo.RegisterCreatedObjectUndo(obj, "InstantiatePrefabs");
            }
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                rot = Quaternion.Euler(rotation * i);
                if (relativeRotation)
                {
                    pos = pos + (rot * offset);
                }
                else
                {
                    pos = offset * i;
                }
                obj = Instantiate(prefab);
                obj.transform.parent = parent;
                obj.transform.localPosition = pos;
                obj.transform.localRotation = rot;
                Undo.RegisterCreatedObjectUndo(obj, "InstantiatePrefabs");
            }
        }
    }
}
