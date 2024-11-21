using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public bool HasMug { get; private set; }
    public Material defaultMaterial;
    public Material hoverMaterial;
    public Material selectedMaterial;
    private Renderer npcRenderer;


    private void Start()
    {
        npcRenderer = GetComponent<Renderer>();
        if (npcRenderer == null)
        {
            Debug.LogError("No Renderer found on NPC");
        }
    }

    public void SetHoverMaterial(bool isHovering)
    {
        if (npcRenderer != null)
        {
            if (isHovering)
            {
                npcRenderer.material = hoverMaterial; // Apply hover material
            }
            else
            {
                npcRenderer.material = defaultMaterial; // Revert to normal material
            }
        }
    }

    public void SelectNPC()
    {
        npcRenderer.material = selectedMaterial;
        Debug.Log($"{gameObject.name} selected!");
    }
}
