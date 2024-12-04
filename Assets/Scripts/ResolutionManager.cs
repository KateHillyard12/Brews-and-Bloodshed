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
            return "You've chosen Stacy as the suspect. She appears visibly nervous. You reported to the police, convinced Stacy was the one responsible. However, when the case went to court, the judge declared her not guilty. As a result, the police revoked your barista license, leaving you penniless and stranded on the streets.";
        }
        else if (selectedNPC.name == "Mark")
        {
            return "Mark appears impatient, his eyes fixed on you with a tense gaze. After you reported him to the police, he was sentenced to death. However, just days after the execution, another murder occurred, revealing the tragic mistake—an innocent man had been executed.";
        }
        else if (selectedNPC.name == "Dave")
        {
            return "Dave offers a warm smile, appearing completely relaxed. You share your suspicions with the police, and after a thorough investigation, Dave pleads guilty. Your café becomes renowned as the 'Murder Mystery Café,' attracting millions and turning you into a wealthy entrepreneur.";
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

        foreach (var obj in FindObjectsOfType<GameObject>())
        {
            if (obj.scene.name == null) // Objects not part of the current scene
            {
                Destroy(obj);
            }
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}