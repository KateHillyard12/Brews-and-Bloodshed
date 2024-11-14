using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class StartScreenController : MonoBehaviour
{
    public GameObject startScreen;
    public Animator imageAnimator;
    public Animator buttonAnimator;
    public Button startButton;

    void Start()
    {
        // Optionally, disable the start button initially
        startButton.gameObject.SetActive(false);

        // Start the animations for the image and button
        StartCoroutine(ShowStartScreen());
    }

    private IEnumerator ShowStartScreen()
    {
        // Play the image slide-in animation
        imageAnimator.Play("ImageSlideIn");

        // Wait for the image animation to finish (adjust timing as needed)
        yield return new WaitForSeconds(1.5f);

        // Play the button slide-in animation
        buttonAnimator.Play("ButtonSlideIn");

        // Show the start button after animation
        startButton.gameObject.SetActive(true);

        // Wait for the button press to start the game
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        // Start the game (load the next scene)
        SceneManager.LoadScene("Brews and Bloodshed");

        // Optionally, disable the start screen before transitioning (if not handled by scene)
        startScreen.SetActive(false);
    }
}
