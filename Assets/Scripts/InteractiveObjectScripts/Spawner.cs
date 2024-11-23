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

        // Handle mismatched spawn points and mugs count
        if (existingMugs.Length != spawnPoints.Length)
        {
            Debug.LogWarning("The number of spawn points does not match the number of mugs!");

            // Adjusting spawn points and mugs if needed (either add or remove objects)
            if (existingMugs.Length < spawnPoints.Length)
            {
                for (int i = existingMugs.Length; i < spawnPoints.Length; i++)
                {
                    // Instantiate new mugs at spawn points if needed
                    GameObject newMug = Instantiate(existingMugs[0], spawnPoints[i].position, spawnPoints[i].rotation);
                    newMug.tag = "Mug"; // Ensure the tag is set
                    existingMugs = GameObject.FindGameObjectsWithTag("Mug"); // Update the array after instantiating
                }
            }
            else if (existingMugs.Length > spawnPoints.Length)
            {
                for (int i = spawnPoints.Length; i < existingMugs.Length; i++)
                {
                    // Remove extra mugs if needed (optional logic to clean up)
                    Destroy(existingMugs[i]);
                }
                existingMugs = GameObject.FindGameObjectsWithTag("Mug"); // Update the array after destruction
            }
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

            // Optionally reset the mug state (e.g., color, ingredients) here if required
            MugSnapper mugSnapper = mug.GetComponent<MugSnapper>();
            if (mugSnapper != null)
            {
                mugSnapper.ResetState(); // Reset mug state
            }
        }
    }
}
