using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartScreenController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public InputActionAsset inputActions; // assign in inspector
    private InputAction anyInputAction;

    private bool inputAllowed = false;

    void Start()
    {
        // Prepare and start video playback
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();

        // Get the input action
        anyInputAction = inputActions.FindAction("anyInput");
        anyInputAction.performed += OnAnyInput;
        anyInputAction.Enable();

        // Start coroutine to allow input after 13 seconds
        StartCoroutine(AllowInputAfterDelay(13f));
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("Video prepared, starting playback.");
        videoPlayer.Play();
    }

    private IEnumerator AllowInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        inputAllowed = true;
        Debug.Log("13 seconds passed! Waiting for player input...");
    }


    private void OnAnyInput(InputAction.CallbackContext context)
    {
        if (inputAllowed)
        {
            Debug.Log("Input detected. Loading main scene...");
            SceneManager.LoadScene("Brews and Bloodshed");
        }
    }

    void OnDestroy()
    {
        anyInputAction.performed -= OnAnyInput;
        anyInputAction.Disable();
    }
}
