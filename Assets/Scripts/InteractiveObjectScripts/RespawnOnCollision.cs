using UnityEngine;

public class RespawnOnCollision : MonoBehaviour
{
    private Transform spawnPoint;
    private LayerMask floorLayer;
    private MugState mugState;

    public void SetSpawnPoint(Transform point)
    {
        spawnPoint = point;
    }

    public void SetFloorLayer(LayerMask layer)
    {
        floorLayer = layer;
    }

    public void SetMugState(MugState state)
    {
        mugState = state;
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

        // Reset state and color
        if (mugState != null)
        {
            mugState.ResetState();
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white; // Reset to default color
        }
    }
}
