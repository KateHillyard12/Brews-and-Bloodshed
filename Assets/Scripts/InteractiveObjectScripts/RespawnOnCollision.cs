using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOnCollision : MonoBehaviour
{
    private Transform spawnPoint;         // The spawn point to return to
    private LayerMask floorLayer;         // Layer mask to identify the floor collider

    // Set the spawn point from the Spawner script
    public void SetSpawnPoint(Transform point)
    {
        spawnPoint = point;
    }

    // Set the floor layer from the Spawner script
    public void SetFloorLayer(LayerMask layer)
    {
        floorLayer = layer;
    }

    // Handle collisions with the floor
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is on the specified floor layer
        if ((floorLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            // Respawn the object at the original spawn point
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }
    }
}
