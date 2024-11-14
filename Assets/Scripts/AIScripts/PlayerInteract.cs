using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            float interactRange = 5.5f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out NPCInteractable npcInteractable))
                {
                    npcInteractable.Interact();
                }
            }
        }
    }

    // Optional: You can keep this method if you want to get the closest interactable
    public NPCInteractable GetInteractableObject()
    {
        List<NPCInteractable> npcInteractableList = new List<NPCInteractable>();
        float interactRange = 10f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out NPCInteractable npcInteractable))
            {
                npcInteractableList.Add(npcInteractable);
            }
        }

        NPCInteractable closestInteractable = null;
        foreach (NPCInteractable npcInteractable in npcInteractableList)
        {
            if (closestInteractable == null)
            {
                closestInteractable = npcInteractable;
            }
            else
            {
                if (Vector3.Distance(transform.position, npcInteractable.transform.position) < Vector3.Distance(transform.position, closestInteractable.transform.position))
                {
                    closestInteractable = npcInteractable;
                }
            }
        }

        return closestInteractable;
    }
}

