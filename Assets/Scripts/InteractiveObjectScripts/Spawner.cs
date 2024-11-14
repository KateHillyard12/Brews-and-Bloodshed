using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefabToSpawn;      // The prefab to be instantiated
    public Transform[] spawnPoints;       // Array of specific spawn points
    public LayerMask floorLayer;          // Layer mask to identify the floor collider

    void Awake()
    {
        SpawnPrefabs();                   // Call the function to spawn prefabs
    }

    void SpawnPrefabs()
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

        // Loop through each spawn point to instantiate the prefab
        foreach (Transform spawnPoint in spawnPoints)
        {
            // Instantiate the prefab and assign its spawn point
            GameObject spawnedPrefab = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
            RespawnOnCollision respawnComponent = spawnedPrefab.AddComponent<RespawnOnCollision>();
            respawnComponent.SetSpawnPoint(spawnPoint);
            respawnComponent.SetFloorLayer(floorLayer);
        }
    }
}
