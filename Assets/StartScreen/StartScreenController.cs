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
        // Hook up video finished event
        videoPlayer.loopPointReached += OnVideoFinished;

        // Hook up prepare completed event
        videoPlayer.prepareCompleted += OnVideoPrepared;

        // Prepare the video (preload)
        videoPlayer.Prepare();

        // Get the input action
        anyInputAction = inputActions.FindAction("anyInput");
        anyInputAction.performed += OnAnyInput;
        anyInputAction.Enable();
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("Video prepared, starting playback.");
        videoPlayer.Play();
    }


    private void OnVideoFinished(VideoPlayer vp)
    {
        inputAllowed = true;
        Debug.Log("Video finished! Waiting for player input...");
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
