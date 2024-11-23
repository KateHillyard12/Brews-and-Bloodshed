using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private NPC selectedNPC;
    public Camera playerCamera;
    public bool isResolutionActive;

    // Range for interactions
    public float interactRange = 15f;
    public float resolutionRange = 10f;

    private void Update()
    {
        // Handle regular NPC interaction (press Q)
        if (Input.GetKeyDown(KeyCode.Q) && !isResolutionActive)
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
            {
                if (hit.collider.TryGetComponent(out NPCInteractable npcInteractable))
                {
                    npcInteractable.Interact();
                }
            }
        }

        // Handle NPC selection when resolution mechanic is active
        if (Input.GetKeyDown(KeyCode.Q) && isResolutionActive)
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, resolutionRange))
            {
                NPC npc = hit.collider.GetComponent<NPC>();
                if (npc != null)
                {
                    // Reset previously selected NPC if there is one
                    if (selectedNPC != null)
                    {
                        selectedNPC.ResetMaterial(); // Reset material or any other state
                    }

                    // Select the new NPC
                    selectedNPC = npc;
                    selectedNPC.SelectNPC(); // Highlight the selected NPC
                }
            }
        }
    }

    // Optional: This method can be used if needed to get the closest interactable object
    public NPCInteractable GetInteractableObject()
    {
        List<NPCInteractable> npcInteractableList = new List<NPCInteractable>();
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
            if (closestInteractable == null || Vector3.Distance(transform.position, npcInteractable.transform.position) < Vector3.Distance(transform.position, closestInteractable.transform.position))
            {
                closestInteractable = npcInteractable;
            }
        }

        return closestInteractable;
    }
}
