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

    private bool hasReceivedMug = false; // Flag to track if the NPC has received a mug
    private string lastResponse; // Last response (correct/incorrect line)

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

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

    public void Interact()
    {
        if (hasReceivedMug)
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
            hasReceivedMug = true; // Mark that the NPC has received a mug
        }
    }

    private bool CheckIngredients(MugSnapper mugSnapper)
    {
        var mugIngredients = mugSnapper.GetIngredients();

        // Check if all desired ingredients are present in the mug
        foreach (string ingredient in desiredIngredients)
        {
            if (!mugIngredients.Contains(ingredient))
            {
                return false;
            }
        }

        return true;
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
