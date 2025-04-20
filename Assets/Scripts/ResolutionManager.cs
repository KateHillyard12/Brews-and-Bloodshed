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
        targetText.text = "";

        foreach (char c in message)
        {
            targetText.text += c;
            yield return new WaitForSeconds(1f / speed);
        }
    }
}
