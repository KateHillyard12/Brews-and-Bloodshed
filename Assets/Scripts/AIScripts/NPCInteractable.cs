using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public GameObject textPrefab; // UI Text prefab for chat bubbles
    public Transform canvasTransform; // Canvas where chat bubbles are displayed
    public Camera mainCamera; // Reference to the camera
    public AudioClip talkingSound; // Talking sound
    private AudioSource audioSource; // Reference to AudioSource

    [SerializeField] private List<string> desiredIngredients; // Ingredients NPC expects
    [SerializeField] private string correctResponse; // Response for correct order
    [SerializeField] private string incorrectResponse; // Response for incorrect order
    [SerializeField] private string[] interactTexts; // Interaction texts before receiving a mug

    public bool HasReceivedMug { get; private set; } = false;

    private string lastResponse; // Last response (correct/incorrect line)
    private NPC npcCurrentlyLookingAt; // To track the NPC currently being looked at
     private MovementScript playerMovementScript; // Reference to player movement script

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
                correctResponse = "Thanks! Just how I like it.";
                incorrectResponse = "That's not what I ordered!";
                interactTexts = new string[] { "It's a lovely day!", "Have you heard about the murder down the street?", "My bestie told me the murder attends this cafe often.", "Anyways, could I get a regular black coffee please?" };
                break;
            case "Mark":
                desiredIngredients = new List<string> { "Coffee", "Milk" };
                correctResponse = "Finally, my coffee with milk.";
                incorrectResponse = "Ugh, this isn't right.";
                interactTexts = new string[] { "Hey", "Could I get a coffee with milk?", "Make it quick, I don’t have a lot of patience." };
                break;
            case "Dave":
                desiredIngredients = new List<string> { "Coffee", "Milk", "CSyrup" }; // Caramel syrup
                correctResponse = "Perfect! Thank you.";
                incorrectResponse = "I asked for caramel syrup!";
                interactTexts = new string[] { "This place is adorable!", "I'll have a coffee with milk and caramel syrup pretty please!" };
                break;
        }

        // Initialize AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = talkingSound;
    }

    void Update()
    {
        // Ensure the hover effect and selection only work when the resolution is active
        if (playerMovementScript.isResolutionActive)
        {
            Debug.Log("Resolution active.");
            // Raycast to detect which NPC the player is looking at
            float radius = 0.1f; 
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.SphereCast(ray.origin, radius, ray.direction, out hit))
            {
                // Check if the raycast hit an NPC child (tagged as NPC)
                if (hit.collider.CompareTag("NPC"))
                {
                    Debug.Log("Looking at NPC child: " + hit.collider.name);

                    // Get the parent of the NPC child
                    Transform npcParent = hit.collider.transform.parent;
                    if (npcParent != null)
                    {
                        Debug.Log($"Looking at NPC parent: {npcParent.name}");

                        // Apply hover effect to the parent
                        NPC npc = npcParent.GetComponent<NPC>();
                        if (npc != null)
                        {
                            npc.SetHoverMaterial(true);
                            npcCurrentlyLookingAt = npc;

                            // If the player presses 'Q', select the NPC
                            if (Input.GetKeyDown(KeyCode.Q))
                            {
                                npc.SelectNPC(); // Change material to selected
                            }
                        }
                    }
                }
            }
            else
            {
                // If the player is not looking at any NPC, disable hover
                if (npcCurrentlyLookingAt != null)
                {
                    npcCurrentlyLookingAt.SetHoverMaterial(false);
                    npcCurrentlyLookingAt = null;
                }
            }
        }
        else
        {
            // Ensure hover effect is disabled when not in resolution
            if (npcCurrentlyLookingAt != null)
            {
                npcCurrentlyLookingAt.SetHoverMaterial(false);
                npcCurrentlyLookingAt = null;
            }
        }
    }


    public void EnableHoverEffect()
    {
        Debug.Log("Hover effect enabled.");
        // Raycast logic to detect if the player is looking at the NPC
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("NPC"))
            {
                NPC npc = hit.collider.GetComponent<NPC>();
                if (npc != null)
                {
                    npc.SetHoverMaterial(true); // Apply hover material
                }
            }
        }
    }

    public void DisableHoverEffect()
    {
        Debug.Log("Hover effect disabled.");
        // Raycast logic to stop the hover effect
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("NPC"))
            {
                NPC npc = hit.collider.GetComponent<NPC>();
                if (npc != null)
                {
                    npc.SetHoverMaterial(false); // Reset material
                }
            }
        }
    }

    public void Interact()
    {
        if (HasReceivedMug)
        {
            // After receiving a mug, repeat the last response
            ChatBubble.Create(canvasTransform, lastResponse, textPrefab, mainCamera, transform);
            Debug.Log($"NPC {name} says: {lastResponse}");
        }
        else
        {
            StartCoroutine(DisplayDialogue(interactTexts));
        }
    }

    public void ReceiveMug(GameObject mug)
    {
        Debug.Log($"NPC {name} received the mug.");
        MugSnapper mugSnapper = mug.GetComponent<MugSnapper>();

        if (mugSnapper != null)
        {
            bool isOrderCorrect = CheckIngredients(mugSnapper);
            lastResponse = isOrderCorrect ? correctResponse : incorrectResponse;

            ChatBubble.Create(canvasTransform, lastResponse, textPrefab, mainCamera, transform);
            audioSource.PlayOneShot(talkingSound);

            Debug.Log($"NPC {name} says: {lastResponse}");
            HasReceivedMug = true;
            Debug.Log($"{name} has received their mug.");
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
        foreach (var text in dialogueTexts)
        {
            ChatBubble.Create(canvasTransform, text, textPrefab, mainCamera, transform);
            audioSource.PlayOneShot(talkingSound);
            yield return new WaitForSeconds(3f); // Wait before showing the next text
        }
    }
}
