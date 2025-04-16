using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Material defaultMaterial;
    public Material hoverMaterial;
    public Material selectedMaterial;

    private Renderer[] npcRenderers;

    
    // Reference to ResolutionManager
    public ResolutionManager resolutionManager;

    private void Start()
    {
        npcRenderers = GetComponentsInChildren<Renderer>(); // Add this line

        if (npcRenderers.Length == 0)
        {
            Debug.LogError("No renderers found on NPC or its children.");
        }


        // Optionally, you can find ResolutionManager dynamically if it's not assigned
        if (resolutionManager == null)
        {
            resolutionManager = FindObjectOfType<ResolutionManager>();
        }
    }

    public void HoverMaterial()
    {
        foreach (Renderer renderer in npcRenderers)
        {
            renderer.material = hoverMaterial;
        }
    }

    public void SelectNPC()
    {
        foreach (Renderer renderer in npcRenderers)
        {
            renderer.material = selectedMaterial;
        }

        Debug.Log($"{gameObject.name} selected!");

        if (resolutionManager != null)
        {
            resolutionManager.EndResolution(this);
        }
    }

    public void ResetMaterial()
    {
        foreach (Renderer renderer in npcRenderers)
        {
            renderer.material = defaultMaterial;
        }
    }

}