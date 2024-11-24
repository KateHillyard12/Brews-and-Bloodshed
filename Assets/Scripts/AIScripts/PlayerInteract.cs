using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private NPC selectedNPC; 
    public Camera playerCamera;
    public bool isResolutionActive;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            float interactRange = 15f;
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
            {
                if (hit.collider.TryGetComponent(out NPCInteractable npcInteractable))
                {
                    Debug.Log("Interacting with: " + npcInteractable.name);
                    npcInteractable.Interact();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && isResolutionActive)
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, 10f))
            {
                NPC npc = hit.collider.GetComponent<NPC>();
                if (npc != null)
                {
                    if (selectedNPC != null)
                    {
                        selectedNPC.ResetMaterial(); // Reset previously selected NPC
                    }

                    selectedNPC = npc;
                    selectedNPC.SelectNPC();
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