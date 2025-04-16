using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public GameObject textPrefab; // UI Text prefab for chat bubbles
    public Transform canvasTransform; // Canvas where chat bubbles are displayed
    public Camera mainCamera; // Reference to the camera
    [SerializeField] private AudioSource audioSource; // Reference to AudioSource

    [SerializeField] private List<string> desiredIngredients; // Ingredients NPC expects
    [SerializeField] private string correctResponse; // Response for correct order
    [SerializeField] private string incorrectResponse; // Response for incorrect order
    [SerializeField] private string[] interactTexts; // Interaction texts before receiving a mug

    public bool HasReceivedMug { get; private set; } = false;
    public int InteractionCount { get; private set; } // Tracks number of interactions

    private string lastResponse; // Last response (correct/incorrect line)
    private bool isDisplayingText = false;

     private MovementScript playerMovementScript; // Reference to player movement script

     private NPCIdleController idleController;


    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        playerMovementScript = FindObjectOfType<MovementScript>();

        // Set default responses and orders based on the NPC's tag
        switch (gameObject.tag)
        {
            case "Stacy":
                desiredIngredients = new List<string> { "Coffee" };
                correctResponse = "Thanks! Just how I like it. I heard the murder likes milk.";
                incorrectResponse = "That's not what I ordered! Oh well...";
                interactTexts = new string[] { "It's a lovely day!", "Have you heard about the murder down the street?", "My bestie told me the murder attends this cafe often.", "Anyways, could I get a regular black coffee please?" };
                break;
            case "Mark":
                desiredIngredients = new List<string> { "Coffee", "Milk" };
                correctResponse = "Finally, my coffee with milk. I bet that murder is pretty happy to get away.";
                incorrectResponse = "Ugh, this isn't right.";
                interactTexts = new string[] { "Hey", "Could I get a coffee with milk?", "Make it quick, I don’t have a lot of patience." };
                break;
            case "Dave":
                desiredIngredients = new List<string> { "Coffee", "Milk", "CSyrup" }; // Caramel syrup
                correctResponse = "Perfect! Thank you. Im just so happy!";
                incorrectResponse = "I asked for caramel syrup!";
                interactTexts = new string[] { "This place is adorable!", "I'll have a coffee with milk and caramel syrup pretty please!" };
                break;
        }
        
        idleController = GetComponentInParent<NPCIdleController>();


    }

    public void Interact()
    {
        InteractionCount++; // Increment interaction count
        if (isDisplayingText) return; // Prevent interaction if text is already displaying
        
        if (playerMovementScript != null && playerMovementScript.isResolutionActive)
        {
            Debug.Log("Interaction skipped during resolution.");
            return;
        }

        if (HasReceivedMug)
        {
            ChatBubble.Create(canvasTransform, lastResponse, textPrefab, mainCamera, transform);
            Debug.Log($"NPC {name} says: {lastResponse}");
            PlayInteractionSound();
        }
        else
        {
            StartCoroutine(DisplayDialogue(interactTexts));
        }
    }

    public void ReceiveMug(GameObject mug)
    {
        Debug.Log($"[RECEIVE] {name} received a mug.");
        
        MugSnapper mugSnapper = mug.GetComponent<MugSnapper>();
        if (mugSnapper == null)
        {
            Debug.LogWarning("MugSnapper not found on mug.");
            return;
        }

        bool isOrderCorrect = CheckIngredients(mugSnapper);
        lastResponse = isOrderCorrect ? correctResponse : incorrectResponse;

        ChatBubble.Create(canvasTransform, lastResponse, textPrefab, mainCamera, transform);
        HasReceivedMug = true;

        Debug.Log("Calling SwitchToMugPose...");
        if (idleController != null)
        {
            idleController.SwitchToMugPose();
        }
        else
        {
            Debug.LogWarning("idleController is null!");
        }
    }


    private bool CheckIngredients(MugSnapper mugSnapper)
    {
        var mugIngredients = mugSnapper.GetIngredients();

        // Check if the number of ingredients matches
        if (mugIngredients.Count != desiredIngredients.Count)
        {
            return false;
        }

        // Create dictionaries to count occurrences of each ingredient
        var desiredCount = CountIngredients(desiredIngredients);
        var mugCount = CountIngredients(mugIngredients);

        // Compare the dictionaries
        foreach (var ingredient in desiredCount)
        {
            if (!mugCount.ContainsKey(ingredient.Key) || mugCount[ingredient.Key] != ingredient.Value)
            {
                return false;
            }
        }

        return true;
    }

    // Helper method to count ingredient occurrences
    private Dictionary<string, int> CountIngredients(List<string> ingredients)
    {
        var ingredientCount = new Dictionary<string, int>();
        foreach (var ingredient in ingredients)
        {
            if (ingredientCount.ContainsKey(ingredient))
            {
                ingredientCount[ingredient]++;
            }
            else
            {
                ingredientCount[ingredient] = 1;
            }
        }
        return ingredientCount;
    }
    
    private IEnumerator DisplayDialogue(string[] dialogueTexts)
    {
        isDisplayingText = true; // Mark text as active
        foreach (var text in dialogueTexts)
        {
            ChatBubble.Create(canvasTransform, text, textPrefab, mainCamera, transform);
            PlayInteractionSound();
            yield return new WaitForSeconds(3f); // Wait before showing the next text
        }
        isDisplayingText = false; // Reset flag after dialogue ends
    }

    private void PlayInteractionSound()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}