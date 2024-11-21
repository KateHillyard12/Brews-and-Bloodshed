using System.Collections.Generic;
using UnityEngine;

public class MugState : MonoBehaviour
{
    private Dictionary<string, int> ingredients = new Dictionary<string, int>();

    public void AddIngredient(string ingredient)
    {
        if (ingredients.ContainsKey(ingredient))
        {
            ingredients[ingredient]++;
        }
        else
        {
            ingredients[ingredient] = 1;
        }

        DebugIngredients();
    }
    
    public void ResetState()
    {
        ingredients.Clear(); // Clear all ingredients
        Debug.Log("Ingredients after reset: " + string.Join(", ", ingredients));
    }

    private void DebugIngredients()
    {
        string ingredientList = "This mug contains: ";
        foreach (var item in ingredients)
        {
            ingredientList += $"{item.Value} {item.Key}(s), ";
        }

        Debug.Log(ingredientList.TrimEnd(',', ' '));
    }
}
