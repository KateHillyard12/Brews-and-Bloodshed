using System.Collections;
using System.Collections.Generic;
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

    public TextMeshProUGUI resolutionText;
    public TextMeshProUGUI finalText;
    public GameObject darkPanel;
    public Button resetButton;
    public GameObject uiToHide;
    public float delayBeforePause = 1f;

    private MusicManager musicManager;

    void Start()
    {
        musicManager = FindObjectOfType<MusicManager>();
        playerMovement = FindObjectOfType<MovementScript>();

        // Hide UI elements initially
        darkPanel.SetActive(false);
        resolutionText.gameObject.SetActive(false);
        finalText.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);

        if (uiToHide != null)
        {
            uiToHide.SetActive(true); // Ensure this UI element is visible at the start
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
            if (!snapPoint.isOccupied) return false;
        }
        return true;
    }

    void ActivateResolution()
    {
        isResolutionActive = true;

        // Hide specific UI element
        if (uiToHide != null)
        {
            uiToHide.SetActive(false);
        }

        playerMovement.isResolutionActive = true;

        musicManager?.ChangeMusic(musicManager.resolutionMusic, 2f); // Null check for music manager

        // Start showing resolution text
        StartCoroutine(ShowInitialResolutionText());
    }

    private IEnumerator ShowInitialResolutionText()
    {
        resolutionText.gameObject.SetActive(true);

        yield return StartCoroutine(AnimateText("Choose the culprit...", resolutionText, 50f));
    }

    public void EndResolution(NPC selectedNPC)
    {
        StartCoroutine(PauseAndDisplayPanel(selectedNPC));
    }

    private IEnumerator PauseAndDisplayPanel(NPC selectedNPC)
    {
        resolutionText.gameObject.SetActive(false);

        yield return new WaitForSeconds(delayBeforePause);

        darkPanel.SetActive(true);
        finalText.gameObject.SetActive(true);
        resetButton.gameObject.SetActive(true);

        finalText.text = "";

        string message = GetResolutionMessage(selectedNPC);
        yield return StartCoroutine(AnimateText(message, finalText));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f; // Pause the game
    }

    private string GetResolutionMessage(NPC selectedNPC)
    {
        switch (selectedNPC.name)
        {
            case "Stacy": return "You have selected Stacy. She looks nervous!";
            case "Mark": return "Mark seems impatient. He’s watching closely.";
            case "Dave": return "Dave smiles warmly. He seems relaxed.";
            default: return "Unknown NPC selected.";
        }
    }

    private IEnumerator AnimateText(string message, TextMeshProUGUI targetText, float speed = 50f)
    {
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

        musicManager?.ResetMusic(); // Null check for music manager

        if (playerCamera != null)
        {
            playerCamera.transform.position = Vector3.zero; 
            playerCamera.transform.rotation = Quaternion.identity;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
