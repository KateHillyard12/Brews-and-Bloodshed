using System.Collections;
using UnityEngine;
using TMPro;

public class MachineNameDisplay : MonoBehaviour
{
    public Camera playerCamera;             // Reference to the player's camera
    public float detectionRange = 30f;       // Range to detect machines
    public TextMeshProUGUI machineNameText; // Reference to the UI Text element
    public float fadeDuration = 0.5f;       // Time for fading the text in and out

    private void Update()
    {
        DetectMachine();
    }

    private void DetectMachine()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main; // Reassign dynamically if null
            if (playerCamera == null)
            {
                Debug.LogWarning("Player camera is null! Skipping detection.");
                return;
            }
        }

        if (machineNameText == null)
        {
            machineNameText = FindObjectOfType<TextMeshProUGUI>(); // Reassign dynamically if null
            if (machineNameText == null)
            {
                Debug.LogWarning("MachineNameText is null! Skipping detection.");
                return;
            }
        }

        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * detectionRange, Color.red);

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, detectionRange, LayerMask.GetMask("Machine")))
        {
            MachineTypeDisplay machine = hit.transform.GetComponentInParent<MachineTypeDisplay>();
            if (machine != null)
            {
                if (!machineNameText.gameObject.activeSelf)
                {
                    StartCoroutine(FadeInText(machine.customMachineName));
                }
                machineNameText.text = machine.customMachineName;
                return;
            }
        }

        if (machineNameText.gameObject.activeSelf)
        {
            StartCoroutine(FadeOutText());
        }
    }


    private IEnumerator FadeInText(string newText)
    {
        machineNameText.text = newText;
        machineNameText.gameObject.SetActive(true);

        float time = 0f;
        Color originalColor = machineNameText.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        while (time < fadeDuration)
        {
            machineNameText.color = Color.Lerp(originalColor, targetColor, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        machineNameText.color = targetColor; // Ensure it's fully visible at the end
    }

    private IEnumerator FadeOutText()
    {
        float time = 0f;
        Color originalColor = machineNameText.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        while (time < fadeDuration)
        {
            machineNameText.color = Color.Lerp(originalColor, targetColor, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        machineNameText.color = targetColor; // Ensure it's fully invisible at the end
        machineNameText.gameObject.SetActive(false); // Hide the text once fully faded out
    }
}
