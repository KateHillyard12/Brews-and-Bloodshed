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
        {
            var possibleOrders = new List<List<string>> {
                new List<string> { "Coffee" },
                new List<string> { "Coffee", "Milk" },
                new List<string> { "Coffee", "VSyrup" },
                new List<string> { "Coffee", "CHSyrup"},
                new List<string> { "Coffee", "Milk", "CSyrup" }
            };
            desiredIngredients = possibleOrders[Random.Range(0, possibleOrders.Count)];
            string orderText = string.Join(", ", desiredIngredients.ConvertAll(FormatIngredientName));

            incorrectResponse = new string[] {
                "That's not what I ordered! Oh well...",
                "Ew, this isn't right.",
                "Hmm... did you mix up the drinks?",
                "I don’t think this is mine, but okay."
            }[Random.Range(0, 4)];

            if (MurderManager.Instance != null)
            {
                string murderer = MurderManager.Instance.murdererName;
                if (murderer == "Dave")
                    correctResponse = $"Thanks! Just how I like it. That guy is looking over his shoulder a lot. Weird.";
                else if (murderer == "Mark")
                    correctResponse = $"Thank you, this is perfect. I heard the murderer is kinda rough looking.";
                else
                    correctResponse = $"Hmm... you're sweet, I bet the murder wuld like you for this coffee!";

                interactTexts = new string[] {
                    "It's a lovely day!",
                    "Have you heard about the murder down the street?",
                    $"My bestie told me the murderer attends this cafe",
                    $"Anyways, could I get a {orderText} please?"
                };
            }
            else
            {
                correctResponse = $"Thanks! Just how I like it — {orderText}.";
                interactTexts = new string[] {
                    "It's a lovely day!",
                    "Have you heard about the murder down the street?",
                    "My bestie told me the murderer attends this cafe...",
                    $"Anyways, could I get a {orderText} please?"
                };
            }
            break;
        }

        case "Mark":
        {
            var possibleOrders = new List<List<string>> {
                new List<string> { "Coffee", "Milk" },
                new List<string> { "Coffee", "Milk", "VSyrup" },
                new List<string> { "Coffee", "CHSyrup" },
                new List<string> { "Coffee" }
            };
            desiredIngredients = possibleOrders[Random.Range(0, possibleOrders.Count)];
            string orderText = string.Join(", ", desiredIngredients.ConvertAll(FormatIngredientName));

            incorrectResponse = new string[] {
                "What is this? Definitely not what I asked for.",
                "This isn't my order. Whatever...",
                "Wrong again. Ugh.",
                "I didn’t ask for this..."
            }[Random.Range(0, 4)];

            if (MurderManager.Instance != null)
            {
                string murderer = MurderManager.Instance.murdererName;
                if (murderer == "Stacy")
                    correctResponse = $"This hits the spot. This chic next to me seems kinda nervous.";
                else if (murderer == "Dave")
                    correctResponse = $"Nice. I wouldn't trust anyone who seems too happy, ya know.";
                else
                    correctResponse = $"Yeah, this’ll do.";

                interactTexts = new string[] {
                    "Hey.",
                    $"Let me get a {orderText}.",
                    "You know, I saw someone shady outside.",
                    $"Make it quick, I don’t have a lot of patience."
                };
            }
            else
            {
                correctResponse = $"Nice, this {orderText} will do.";
                interactTexts = new string[] {
                    "Hey.",
                    $"Let me get a {orderText}.",
                    "Make it quick, I don’t have a lot of patience."
                };
            }
            break;
        }

        case "Dave":
        {
            var possibleOrders = new List<List<string>> {
                new List<string> { "Coffee", "Milk", "CSyrup" },
                new List<string> { "Coffee", "CSyrup" },
                new List<string> { "Coffee", "Milk" },
                new List<string> { "Coffee" }
            };
            desiredIngredients = possibleOrders[Random.Range(0, possibleOrders.Count)];
            string orderText = string.Join(", ", desiredIngredients.ConvertAll(FormatIngredientName));

            incorrectResponse = new string[] {
                "I asked for something else!",
                "This isn't what I expected...",
                "Aw man, wrong drink.",
                "Hmm, not quite right."
            }[Random.Range(0, 4)];

            if (MurderManager.Instance != null)
            {
                string murderer = MurderManager.Instance.murdererName;
                if (murderer == "Stacy")
                    correctResponse = $"Perfect! Did you hear that lady's best friend got killed?";
                else if (murderer == "Mark")
                    correctResponse = $"Mmm. If I was a murder I would be in rush to flee the country.";
                else
                    correctResponse = $"You made this? Well done. Watch your back though...";

                interactTexts = new string[] {
                    "This place is adorable!",
                    $"I'd like a {orderText}, pretty please!",
                    $"Hope the murderer doesn’t like {orderText} too…"
                };
            }
            else
            {
                correctResponse = $"Perfect! This {orderText} is just what I wanted.";
                interactTexts = new string[] {
                    "This place is adorable!",
                    $"I'd like a {orderText}, please!"
                };
            }
            break;
        }

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
        PlayInteractionSound();

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
            yield return new WaitForSeconds(4.5f); // Wait before showing the next text
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

    private string FormatIngredientName(string rawName)
    {
        switch (rawName)
        {
            case "CSyrup": return "caramel syrup";
            case "VSyrup": return "vanilla syrup";
            case "CHSyrup": return "chocolate syrup"; // if you use this tag
            default: return rawName.ToLower();
        }
    }

}