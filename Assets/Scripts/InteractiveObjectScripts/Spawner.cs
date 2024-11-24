using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoints;       // Array of specific spawn points
    public LayerMask floorLayer;          // Layer mask to identify the floor collider
    private GameObject[] existingMugs;    // Array to store the existing mugs

    void Awake()
    {
        InitializeMugs();
    }

    void InitializeMugs()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points defined!");
            return;
        }

        // Find all existing mugs in the scene by their tag
        existingMugs = GameObject.FindGameObjectsWithTag("Mug");

        // Check if we have the same number of mugs and spawn points
        if (existingMugs.Length != spawnPoints.Length)
        {
            Debug.LogWarning("The number of spawn points does not match the number of mugs!");
            return;
        }

        for (int i = 0; i < existingMugs.Length; i++)
        {
            GameObject mug = existingMugs[i];
            Transform spawnPoint = spawnPoints[i];

            // Set initial position and rotation
            mug.transform.position = spawnPoint.position;
            mug.transform.rotation = spawnPoint.rotation;

            // Add or configure respawn component
            RespawnOnCollision respawnComponent = mug.GetComponent<RespawnOnCollision>();
            if (respawnComponent == null)
            {
                respawnComponent = mug.AddComponent<RespawnOnCollision>();
            }

            respawnComponent.SetSpawnPoint(spawnPoint);
            respawnComponent.SetFloorLayer(floorLayer);
        }
    }
}