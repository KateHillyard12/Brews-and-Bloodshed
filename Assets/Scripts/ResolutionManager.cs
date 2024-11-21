using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public List<SnapPoint> npcSnapPoints; // List of all NPC snap points
    public Camera playerCamera;
    public MovementScript playerMovement; // Reference to player movement script

    public bool isResolutionActive = false;

    void Update()
    {
        // Check if all NPC snap points are occupied
        if (!isResolutionActive && AllNPCsOccupied())
        {
            ActivateResolution();
        }
    }

    bool AllNPCsOccupied()
    {
        foreach (SnapPoint snapPoint in npcSnapPoints)
        {
            if (!snapPoint.isOccupied)
                return false;
        }
        return true;
    }

    void ActivateResolution()
    {
        isResolutionActive = true;
        
        // Stop player movement but allow horizontal camera rotation
        playerMovement.isResolutionActive = true;


        // Display text and initiate hover effect for NPCs
        foreach (var npc in npcSnapPoints)
        {
            NPCInteractable npcInteractable = npc.GetComponentInParent<NPCInteractable>();
            if (npcInteractable != null)
            {
                npcInteractable.EnableHoverEffect(); // Enable hover on NPCs
            }
        }
    }
}

