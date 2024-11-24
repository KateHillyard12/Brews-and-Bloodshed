using System.Collections;
using System.Collections.Generic; // For List<>
using UnityEngine;
using TMPro; // For TextMeshPro
using UnityEngine.SceneManagement; // For resetting the scene
using UnityEngine.UI; // For UI components

public class ResolutionManager : MonoBehaviour
{
    public List<SnapPoint> npcSnapPoints;
    public Camera playerCamera;
    public MovementScript playerMovement;

    public bool isResolutionActive = false;

    public TextMeshProUGUI resolutionText; // "Choose the culprit" text
    public TextMeshProUGUI finalText; // Final NPC selection text
    public GameObject darkPanel; // The dark panel
    public Button resetButton; // Reset button
    public GameObject uiToHide; // Specific UI element to hide
    public float delayBeforePause = 1f;

    private MusicManager musicManager;

    void Start()
    {
        musicManager = FindObjectOfType<MusicManager>();
        playerMovement = FindObjectOfType<MovementScript>();

        // Hide all UI elements at the start
        darkPanel.SetActive(false);
        resolutionText.gameObject.SetActive(false);
        finalText.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);

        if (uiToHide != null)
        {
            uiToHide.SetActive(true); // Ensure the UI element is visible at the start
        }

        resetButton.onClick.AddListener(ResetGame);
    }

    void Update()
    {
        if (!isResolutionActive && AllNPCsOccupied())
        {
            ActivateResolution();
        }
    }

    bool AllNPCsOccupied()
    {
        foreach (SnapPoint snapPoint in npcSnapPoints)
        {
            if (!snapPoint.isOccupied)
                return false;
        }
        return true;
    }

    void ActivateResolution()
    {
        isResolutionActive = true;

        // Hide specific UI element
        if (uiToHide != null)
        {
            uiToHide.SetActive(false); // Hide the UI element when entering resolution phase
        }

        playerMovement.isResolutionActive = true;

        if (musicManager != null && musicManager.resolutionMusic != null)
        {
            musicManager.ChangeMusic(musicManager.resolutionMusic, 2f);
        }

        // Show "Choose the culprit" message but do not display the panel yet
        StartCoroutine(ShowInitialResolutionText());
    }

    private IEnumerator ShowInitialResolutionText()
    {
        resolutionText.gameObject.SetActive(true);

        yield return StartCoroutine(AnimateText("Choose the culprit...", resolutionText, 50f));

        // Keep the "Choose the culprit" text visible while waiting for player input
    }

    public void EndResolution(NPC selectedNPC)
    {
        StartCoroutine(PauseAndDisplayPanel(selectedNPC));
    }

    private IEnumerator PauseAndDisplayPanel(NPC selectedNPC)
    {
        // Hide the "Choose the culprit" text
        resolutionText.gameObject.SetActive(false);

        // Add a slight delay before showing the panel
        yield return new WaitForSeconds(delayBeforePause);

        // Show the panel, final text, and reset button
        darkPanel.SetActive(true);
        finalText.gameObject.SetActive(true);
        resetButton.gameObject.SetActive(true);

        // Ensure the text starts empty
        finalText.text = "";

        // Get the resolution message and animate it
        string message = GetResolutionMessage(selectedNPC);
        yield return StartCoroutine(AnimateText(message, finalText));

        // Unlock the cursor for interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause the game
        Time.timeScale = 0f;
    }

    private string GetResolutionMessage(NPC selectedNPC)
    {
        if (selectedNPC.name == "Stacy")
        {
            return "You have selected Stacy. She looks nervous!";
        }
        else if (selectedNPC.name == "Mark")
        {
            return "Mark seems impatient. He’s watching closely.";
        }
        else if (selectedNPC.name == "Dave")
        {
            return "Dave smiles warmly. He seems relaxed.";
        }

        return "Unknown NPC selected.";
    }

    private IEnumerator AnimateText(string message, TextMeshProUGUI targetText, float speed = 50f)
    {
        // Ensure the text starts empty
        targetText.text = "";

        foreach (char c in message)
        {
            targetText.text += c;
            yield return new WaitForSeconds(1f / speed);
        }
    }

    private void ResetGame()
    {
        Time.timeScale = 1f;
        // Reset music to the original track
        if (musicManager != null)
        {
            musicManager.ResetMusic();
        }

        // Optionally reset the player camera if necessary
        if (playerCamera != null)
        {
            // Reassign or reset the player camera as needed
            playerCamera.transform.position = Vector3.zero;  // Example position reset
            playerCamera.transform.rotation = Quaternion.identity;  // Example rotation reset
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}