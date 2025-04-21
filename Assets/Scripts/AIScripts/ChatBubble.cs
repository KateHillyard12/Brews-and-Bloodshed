using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatBubble : MonoBehaviour
{
    private Transform npcTransform;  // Reference to the NPC this chat bubble is tied to
    private Camera mainCamera;  // Reference to the camera
    private RectTransform rectTransform;  // The RectTransform of the chat bubble

    // Create method now receives only the necessary parameters
    public static void Create(Transform canvasTransform, string text, GameObject textPrefab, Camera mainCamera, Transform npcTransform)
    {
        // Instantiate the UI text object on the Canvas
        GameObject chatBubbleText = Instantiate(textPrefab, canvasTransform);
        
        // Get components and set initial properties
        ChatBubble chatBubble = chatBubbleText.GetComponent<ChatBubble>();
        chatBubble.rectTransform = chatBubbleText.GetComponent<RectTransform>();
        chatBubble.npcTransform = npcTransform;
        chatBubble.mainCamera = mainCamera;

        // Set the actual text
        TextMeshProUGUI chatText = chatBubbleText.GetComponent<TextMeshProUGUI>();
        chatText.text = text;

        // Destroy after a set duration (e.g., 2 seconds)
        Destroy(chatBubbleText, 4f);
    }

    private void Update()
    {
        if (npcTransform != null && mainCamera != null)
        {
            Vector3 worldOffset = npcTransform.position + new Vector3(0, 2, 0);
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldOffset);

            // Only show if the NPC is in front of the camera
            if (screenPosition.z > 0)
            {
                rectTransform.position = screenPosition;
                rectTransform.gameObject.SetActive(true);
            }
            else
            {
                rectTransform.gameObject.SetActive(false);
            }
        }
    }

}
