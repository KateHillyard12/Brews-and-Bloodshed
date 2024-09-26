using UnityEngine;

public class Spawner : MonoBehaviour
{
public GameObject prefabToSpawn;
    public Transform[] spawnPoints;

    void Awake()
    {
        SpawnPrefab();
    }

    void SpawnPrefab()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("Prefab to spawn is not assigned!");
            return;
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points defined!");
            return;
        }

        Transform chosenSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(prefabToSpawn, chosenSpawnPoint.position, chosenSpawnPoint.rotation);
    }
}
