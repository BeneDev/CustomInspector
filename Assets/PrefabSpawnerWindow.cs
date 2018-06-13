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

    string saveFileName;
    string loadFileName;

    Vector2 scrollViewPos;

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
        scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos);
        {
            if (!prefab)
            {
                // "Sieht halt schon ein bisschen räudig aus." - Utz Stauder, 2018
                // GUILayout.Label("Please assign a prefab reference.", GUI.tooltip);
                EditorGUILayout.HelpBox("Please assign a prefab reference.", MessageType.Info);
            }
            prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false) as GameObject;
            parentTransform = EditorGUILayout.ObjectField(new GUIContent("Parent", "All prefabs will be parented to this transform"), parentTransform, typeof(Transform), true) as Transform;

            keepPrefabLink = EditorGUILayout.Toggle("Keep Prefab Link", keepPrefabLink);

            amount = EditorGUILayout.IntField("SpawnAmount", amount);
            if (amount < 1)
            {
                amount = 1;
            }

            offset = EditorGUILayout.Vector3Field("Offset", offset);

            rotation = EditorGUILayout.Vector3Field("Rotation", rotation);
            relativeRotation = EditorGUILayout.Toggle(new GUIContent("Relative Rotation", "Determines if the rotation will be relative to the last object's rotation."), relativeRotation);

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
                    if (GUILayout.Button("Spawn Objects"))
                    {
                        InstantiateObjects(prefab, amount, offset, rotation, relativeRotation, parentTransform, keepPrefabLink);
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                saveFileName = EditorGUILayout.TextField("Save As", saveFileName);
                if (GUILayout.Button("Save Settings"))
                {
                    SaveSettings();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                loadFileName = EditorGUILayout.TextField("Load", loadFileName);
                if (GUILayout.Button("Load Settings"))
                {
                    LoadSettings();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    private static void SaveSettings()
    {

    }

    private static void LoadSettings()
    {

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
