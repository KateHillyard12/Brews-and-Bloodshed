using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // New Input System

public class PlayerInteract : MonoBehaviour
{
    private NPC selectedNPC;
    public Camera playerCamera;
    public bool isResolutionActive;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) // Ensures action triggers only once per press
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

            if (isResolutionActive)
            {
                if (Physics.Raycast(ray, out hit, 10f))
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
    }
}
