using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjectsSpawner : MonoBehaviour
{
    [SerializeField] GameObject persistentObjectsPrefab;

    private void Awake()
    {
        var existingObjs = FindObjectsOfType<PersistentObjects>();
        if(existingObjs.Length == 0)
        {
            Instantiate(persistentObjectsPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}
