using System.Collections.Generic;
using UnityEngine;

public class MugSnapper : MonoBehaviour
{
    private SnapPoint currentSnapPoint = null;
    private Color currentColor = Color.white; // Default mug color
    private Dictionary<MachineType, Color> ingredientColors;
    private List<string> ingredients = new List<string>(); // List to store added ingredients

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

            if (ingredientColors.TryGetValue(currentSnapPoint.machineType, out Color ingredientColor))
            {
                // Add the ingredient and color
                AddIngredient(currentSnapPoint.machineType.ToString(), ingredientColor);
            }
        }
    }

    private void AddIngredient(string ingredientName, Color ingredientColor)
    {
        // Add the ingredient to the list
        ingredients.Add(ingredientName);

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
            renderer.material.color = currentColor;
        }
    }
}
