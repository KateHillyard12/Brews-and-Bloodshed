using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Material defaultMaterial;
    public Material hoverMaterial;
    public Material selectedMaterial;

    private Renderer npcRenderer;
    
    // Reference to ResolutionManager
    public ResolutionManager resolutionManager;

    private void Start()
    {
        npcRenderer = GetComponent<Renderer>();
        if (npcRenderer == null)
        {
            Debug.LogError("No Renderer found on NPC");
        }

        // Optionally, you can find ResolutionManager dynamically if it's not assigned
        if (resolutionManager == null)
        {
            resolutionManager = FindObjectOfType<ResolutionManager>();
        }
    }

    public void HoverMaterial()
    {
        npcRenderer.material = hoverMaterial; // Apply hover material
    }

    public void SelectNPC()
    {
        if (npcRenderer != null)
        {
            npcRenderer.material = selectedMaterial; // Set to selected material
            Debug.Log($"{gameObject.name} selected!");

            // Call EndResolution from ResolutionManager when the NPC is selected
            if (resolutionManager != null)
            {
                resolutionManager.EndResolution(this);  // Pass this NPC as the selected NPC
            }
        }
    }

    public void ResetMaterial()
    {
        npcRenderer.material = defaultMaterial; // Reset to default material
    }
}