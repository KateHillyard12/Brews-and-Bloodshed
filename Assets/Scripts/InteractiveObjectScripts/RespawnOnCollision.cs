using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOnCollision : MonoBehaviour
{
    private Transform spawnPoint;
    private LayerMask floorLayer;

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
        if (((1 << collision.gameObject.layer) & floorLayer) != 0)
        {
            Debug.Log("Mug hit the floor, respawning...");
            Respawn();
        }
    }

    private void Respawn()
    {
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

        Debug.Log("Mug fully reset and respawned.");
    }


    private IEnumerator EnableInteractionAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // Adjust delay as necessary
        Debug.Log("Mug ready for interaction.");
    }

}