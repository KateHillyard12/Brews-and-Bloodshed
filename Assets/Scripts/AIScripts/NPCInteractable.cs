using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public GameObject textPrefab;  // The UI Text prefab for chat bubbles
    public Transform canvasTransform;  // Reference to the Canvas where the text will be displayed
    public Camera mainCamera;  // Reference to the camera
    public AudioClip talkingSound;  // The talking sound clip
    private AudioSource audioSource;  // Reference to the AudioSource component

    [SerializeField] private string interactText;
    [SerializeField] private string[] interactTexts;  // Array of interaction texts
    private bool isInteracting = false;  // Flag to prevent overlapping dialogue

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;  // Automatically assign the main camera if not assigned
        }

        // Initialize the AudioSource and assign the talking sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = talkingSound;

        // Set default text based on the NPC's tag
        switch (gameObject.tag)
        {
            case "Stacy":
                interactTexts = new string[] { "It's a lovely day!", "Have you heard about the murder down the street?", "My bestie told me the murder attends this cafe often", "Anyways, could I get a regular black coffee please?" };
                break;
            case "Mark":
                interactTexts = new string[] { "Hey", "Could I get a coffee with milk?", "Make it quick, I dont have a lot of paitence." };
                break;
            case "Dave":
                interactTexts = new string[] { "This place is adorable!", "I'll have a coffee with milk and caramel syrup pretty please!" };
                break;
        }
    }

    public void Interact()
    {
        if (!isInteracting)  // Only allow interaction if not already interacting
        {
            isInteracting = true;  // Set flag to indicate interaction is in progress
            Debug.Log($"Interacting with NPC: {gameObject.name}");
            StartCoroutine(DisplayDialogue());
        }
    }

    private IEnumerator DisplayDialogue()
    {
        foreach (var text in interactTexts)
        {
            ChatBubble.Create(canvasTransform, text, textPrefab, mainCamera, transform);
            audioSource.PlayOneShot(talkingSound); // Play the talking sound
            yield return new WaitForSeconds(3f); // Wait for 3 seconds before showing the next sentence
        }
        
        isInteracting = false;  // Reset flag after dialogue is finished
    }

    public string GetInteractText()
    {
        return interactText;
    }
}
