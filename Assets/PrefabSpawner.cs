using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour {

    public GameObject prefab;
    public int spawnCount = 1;
    public Vector3 rotation;
    Quaternion objectRotation;
    public Vector3 offset;
    public bool isOffsetRelative = false;
    public bool deleteObjectsFromBefore = false;

    public void SpawnObjects(GameObject prefab, int count, Vector3 offset, Transform parent)
    {
        objectRotation = Quaternion.Euler(rotation);
        if(deleteObjectsFromBefore)
        {
            for (int i = transform.childCount -1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
        if(!isOffsetRelative)
        {
            for (int i = 0; i < count; i++)
            {
                Instantiate(prefab, offset * i, objectRotation, parent);
            }
        }
        else
        {
            Transform lastTransform = transform;
            for (int i = 0; i < count; i++)
            {
                GameObject lastSpawned = Instantiate(prefab, lastTransform.position, objectRotation * lastTransform.rotation, parent);
                lastSpawned.transform.position  += new Vector3(offset.x * lastTransform.right.x, offset.y * lastTransform.right.y, offset.z * lastTransform.right.z)
                                                + new Vector3(offset.x * lastTransform.up.x, offset.y * lastTransform.up.y, offset.z * lastTransform.up.z) 
                                                + new Vector3(offset.x * lastTransform.forward.x, offset.y * lastTransform.forward.y, offset.z * lastTransform.forward.z);
                lastTransform = lastSpawned.transform;
            }
        }
    }
}
