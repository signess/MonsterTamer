using UnityEngine;

public class PersistentObjectsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject persistentObjectsPrefab;

    private void Awake()
    {
        var existingObjs = FindObjectsOfType<PersistentObjects>();
        if (existingObjs.Length == 0)
        {
            var spawnPos = Vector3.zero;
            var grid = FindObjectOfType<Grid>();
            if (grid != null)
                spawnPos = grid.transform.position;
            Instantiate(persistentObjectsPrefab, spawnPos, Quaternion.identity);
        }
    }
}