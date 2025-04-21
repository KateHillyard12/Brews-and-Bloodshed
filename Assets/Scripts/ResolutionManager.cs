using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{
    public List<SnapPoint> npcSnapPoints;
    public Camera playerCamera;
    public MovementScript playerMovement;

    public bool isResolutionActive = false;

    public TextMeshProUGUI resolutionText;
    public TextMeshProUGUI finalText;
    public GameObject darkPanel;
    public GameObject uiToHide;
    public float delayBeforePause = 1f;

    void Start()
    {
        playerMovement = FindObjectOfType<MovementScript>();

        darkPanel.SetActive(false);
        resolutionText.gameObject.SetActive(false);
        finalText.gameObject.SetActive(false);

        if (uiToHide != null)
            uiToHide.SetActive(true);
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

        if (uiToHide != null)
            uiToHide.SetActive(false);

        playerMovement.isResolutionActive = true;

        // 💡 Smooth music transition
        MusicManager.Instance?.PlaySuspenseMusic();

        StartCoroutine(ShowInitialResolutionText());
    }

    private IEnumerator ShowInitialResolutionText()
    {
        resolutionText.gameObject.SetActive(true);

        yield return StartCoroutine(AnimateText("Choose the culprit...", resolutionText, 50f));
    }

    public void EndResolution(NPC selectedNPC)
    {
        MusicManager.Instance?.PlayResolutionMusic();
        StartCoroutine(PauseAndDisplayPanel(selectedNPC));
    }

    private IEnumerator PauseAndDisplayPanel(NPC selectedNPC)
    {
        resolutionText.gameObject.SetActive(false);
        yield return new WaitForSeconds(delayBeforePause);

        darkPanel.SetActive(true);
        finalText.gameObject.SetActive(true);
        finalText.text = "";

        string message = GetResolutionMessage(selectedNPC);
        yield return StartCoroutine(AnimateText(message, finalText));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    private string GetResolutionMessage(NPC selectedNPC)
    {
        string murderer = MurderManager.Instance != null ? MurderManager.Instance.murdererName : "Dave";
        bool guessedCorrectly = selectedNPC.name == murderer;

        if (guessedCorrectly)
        {
            switch (selectedNPC.name)
            {
                case "Stacy":
                    return "You accuse Stacy. Her confident facade crumbles. She breaks down, confessing to the crime in tears. Justice is served.";
                case "Mark":
                    return "Mark stares at you coldly. After a tense silence, he admits everything. The case is closed — you got the right guy.";
                case "Dave":
                    return "Dave's cheerful demeanor shifts instantly. The police cuff him on the spot. Turns out the bubbly boy was hiding a dark secret.";
            }
        }
        else
        {
            switch (selectedNPC.name)
            {
                case "Stacy":
                    return "You accuse Stacy, but she’s stunned. No evidence links her to the crime. Meanwhile, the real killer roams free.";
                case "Mark":
                    return "Mark is furious at your accusation. With no proof, the case collapses, and your reputation is ruined.";
                case "Dave":
                    return "You point the finger at Dave. He laughs nervously, but it’s clear he’s innocent. The real murderer slips away in the chaos.";
            }
        }

        return "Something went wrong with the resolution...";
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
}
