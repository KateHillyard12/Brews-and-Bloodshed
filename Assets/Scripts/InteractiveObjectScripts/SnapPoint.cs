using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public bool isOccupied = false;  // Flag to indicate if the snap point is occupied
    public MachineType machineType; // Type of machine for machine snap points

    public void SnapToPoint(Transform mugTransform)
    {
        mugTransform.position = transform.position;
        mugTransform.rotation = transform.rotation;
        isOccupied = true;

        if (CompareTag("Machine"))
        {
            Debug.Log("Snapped to machine. Mug is ready for processing.");
        }
        else if (CompareTag("NPC"))
        {
            Debug.Log("Snapped to NPC. Mug is now held.");
            mugTransform.parent = transform; // Parent the mug to the snap point

            // Notify the NPCInteractable script
            NPCInteractable npc = GetComponentInParent<NPCInteractable>();
            if (npc != null)
            {
                npc.ReceiveMug(mugTransform.gameObject);
                Debug.Log($"{npc.name} received a mug.");
            }
            else
            {
                Debug.LogWarning("No NPCInteractable script found on parent!");
            }
        }
    }

    public void Release()
    {
        if (CompareTag("NPC"))
        {
            Debug.Log("NPC snap points do not release mugs.");
            return;
        }

        isOccupied = false;
        Debug.Log("Released snap point.");
    }
}
