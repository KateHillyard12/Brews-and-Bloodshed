using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class MugSnapper : MonoBehaviour
{
    private SnapPoint currentSnapPoint = null;
    private Color currentColor = Color.white; // Default mug color
    private Dictionary<MachineType, Color> ingredientColors;
    private List<string> ingredients = new List<string>();
    [SerializeField] private VisualEffect steamVFX;


    private void Awake()
    {
        ingredientColors = new Dictionary<MachineType, Color>
        {
            { MachineType.Coffee, new Color(0.3f, 0.15f, 0f) }, // Dark brown
            { MachineType.Milk, new Color(1f, 0.95f, 0.8f) },   // Cream
            { MachineType.VSyrup, new Color(1f, 1f, 0.85f) },   // Vanilla
            { MachineType.CSyrup, new Color(0.8f, 0.6f, 0.2f) }, // Caramel
            { MachineType.CHSyrup, new Color(0.4f, 0.2f, 0.1f) } // Chocolate
        };

        UpdateMugColor();
        
        if (steamVFX != null)
        {
            steamVFX.Stop(); // Prevent auto-play
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SnapPoint snapPoint = other.GetComponent<SnapPoint>();
        if (snapPoint != null && !snapPoint.isOccupied)
        {
            currentSnapPoint = snapPoint;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SnapPoint snapPoint = other.GetComponent<SnapPoint>();
        if (snapPoint != null && snapPoint == currentSnapPoint)
        {
            currentSnapPoint.Release();
            currentSnapPoint = null;
        }
    }

    public void DropMug()
    {
        if (currentSnapPoint != null)
        {
            currentSnapPoint.SnapToPoint(transform);

            if (currentSnapPoint.CompareTag("Machine"))
            {
                PourEffectController pourEffect = currentSnapPoint.GetComponent<PourEffectController>();
                if (pourEffect != null)
                {
                    pourEffect.StartPouring();
                }
                // Machine interaction: add ingredient and update color
                if (ingredientColors.TryGetValue(currentSnapPoint.machineType, out Color ingredientColor))
                {
                    AddIngredient(currentSnapPoint.machineType.ToString(), ingredientColor);

                }
            }
            else if (currentSnapPoint.CompareTag("NPC"))
            {
                // NPC interaction: Notify the NPC that they received the mug
                NPCInteractable npc = currentSnapPoint.GetComponentInParent<NPCInteractable>();
                if (npc != null)
                {
                    Debug.Log($"Mug given to NPC: {npc.name}");
                    npc.ReceiveMug(this.gameObject); // Notify the NPC
                    currentSnapPoint.isOccupied = true;
                    enabled = false; // Disable the MugSnapper script
                }
            }
        }
    }

    private void AddIngredient(string ingredientName, Color ingredientColor)
    {
        // Add the ingredient to the list
        ingredients.Add(ingredientName);

        if (ingredientName == "Coffee" && steamVFX != null)
        {
            steamVFX.Play();
        }


        // Update the mug's color (mix the color)
        currentColor = Color.Lerp(currentColor, ingredientColor, 0.5f);
        UpdateMugColor();

        // Output the ingredient list as a debug message
        string ingredientList = string.Join(", ", ingredients);
        Debug.Log($"The mug now contains: {ingredientList}");
    }

    private void UpdateMugColor()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.gameObject.name == "Liquid") 
            {
                renderer.material.color = currentColor;
            }
        }
    }



    public List<string> GetIngredients()
    {
        return new List<string>(ingredients);
    }

    public void ResetState()
    {
        ingredients.Clear();
        currentColor = Color.white;
        UpdateMugColor();
        Debug.Log("MugSnapper reset: ingredients cleared, color reset.");
        if (steamVFX != null)
        {
            steamVFX.Stop();
        }

    }
}
