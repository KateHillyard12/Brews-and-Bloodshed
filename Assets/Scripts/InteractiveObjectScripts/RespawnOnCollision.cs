using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOnCollision : MonoBehaviour
{
    private Transform spawnPoint;
    private LayerMask floorLayer;
    private bool isRespawning = false; // Prevents multiple respawns at once

    public void SetSpawnPoint(Transform point)
    {
        spawnPoint = point;
    }

    public void SetFloorLayer(LayerMask layer)
    {
        floorLayer = layer;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & floorLayer) != 0 && !isRespawning)
        {
            StartCoroutine(RespawnCoroutine());
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        isRespawning = true;

        // Wait for a brief moment before resetting
        yield return new WaitForSeconds(0.1f);

        // Reset position and rotation
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        // Reset MugSnapper state
        MugSnapper mugSnapper = GetComponent<MugSnapper>();
        if (mugSnapper != null)
        {
            mugSnapper.ResetState();
        }

        // Reset ObjectGrabbable state
        ObjectGrabbable grabbable = GetComponent<ObjectGrabbable>();
        if (grabbable != null)
        {
            grabbable.ResetGrabbable();
        }

        // Reset Renderer (color)
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white; // Reset color
        }

        // Reset other potential components (optional)
        // Add additional resets here if necessary

        // Optionally, reset the mug's ingredients or other states
        MugState mugState = GetComponent<MugState>();
        if (mugState != null)
        {
            mugState.ResetState();
        }

        // Give the mug a brief moment to reset before interaction
        yield return new WaitForSeconds(0.1f);

        isRespawning = false;
    }
}
