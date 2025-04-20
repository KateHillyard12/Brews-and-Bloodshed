using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // For resetting the scene

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject howToPlayUI;
    public Button firstButton;
    public bool isPaused = false;
    
    private PlayerInput playerInput;
    private EventSystem eventSystem;
    private Vector2 navigationInput;
    private int selectedButtonIndex = 0;
    private Button[] menuButtons;
    private MovementScript playerMovement;
    private Vector3 defaultButtonScale = new Vector3(0.3f, 0.3f, 0f);
    private Vector3 highlightedButtonScale = new Vector3(0.4f, 0.4f, 0f);


    private void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>(); // Use existing PlayerInput
        eventSystem = EventSystem.current;
        playerMovement = FindObjectOfType<MovementScript>();
        pauseMenuUI.SetActive(false); // Ensure the menu starts hidden
        howToPlayUI.SetActive(false); // Ensure the how to play UI starts hidden
        Cursor.lockState = CursorLockMode.Locked; // Keep cursor locked
    }

    private void Start()
    {
        menuButtons = pauseMenuUI.GetComponentsInChildren<Button>();
        if (menuButtons.Length > 0)
        {
            selectedButtonIndex = 0;
        }
        else
        {
            Debug.LogError("PauseMenu: No buttons found in menu UI!");
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed && !playerMovement.isResolutionActive)
        {
            TogglePauseMenu();
        }
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            navigationInput = context.ReadValue<Vector2>();
            HandleNavigation();
        }
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SelectButton();
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ResumeGame();
        }
    }

    private void HandleNavigation()
    {
        if (menuButtons == null || menuButtons.Length == 0) return; // Prevent out-of-bounds error

        int previousIndex = selectedButtonIndex;
        
        if (navigationInput.y > 0.5f)
        {
            selectedButtonIndex = (selectedButtonIndex - 1 + menuButtons.Length) % menuButtons.Length;
        }
        else if (navigationInput.y < -0.5f)
        {
            selectedButtonIndex = (selectedButtonIndex + 1) % menuButtons.Length;
        }
        
        if (previousIndex != selectedButtonIndex)
        {
            ResetButtonTransforms(); // Reset all buttons before applying effect
            eventSystem.SetSelectedGameObject(menuButtons[selectedButtonIndex].gameObject);
            HighlightButton(menuButtons[selectedButtonIndex]);
        }
    }

    private void HighlightButton(Button button)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.localScale = highlightedButtonScale; // Make button appear larger (jumping out effect)
    }

    private void ResetButtonTransforms()
    {
        foreach (Button btn in menuButtons)
        {
            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.localScale = defaultButtonScale;
        }
    }

    private void SelectButton()
    {
        menuButtons[selectedButtonIndex].onClick.Invoke();
    }
    
    public void TogglePauseMenu()
    {
        if (pauseMenuUI == null)
        {
            Debug.LogError("PauseMenuUI is null! Attempting to find it again...");
            pauseMenuUI = GameObject.Find("PauseMenuUI");

            if (pauseMenuUI == null)
            {
                Debug.LogError("PauseMenuUI could not be found. Aborting pause toggle.");
                return;
            }
        }

        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.lockState = CursorLockMode.Locked;

        if (isPaused)
        {
            MusicManager.Instance?.FadeOutAllMusic(0.75f);
            MusicManager.Instance?.ApplyLowPass(true, 0.5f);
            playerInput.SwitchCurrentActionMap("UI");

            if (menuButtons != null && menuButtons.Length > 0)
            {
                selectedButtonIndex = 0;
                ResetButtonTransforms();
                eventSystem.SetSelectedGameObject(menuButtons[selectedButtonIndex].gameObject);
                HighlightButton(menuButtons[selectedButtonIndex]);
            }
        }
        else
        {
            MusicManager.Instance?.FadeInAllMusic(0.75f);
            MusicManager.Instance?.ApplyLowPass(false, 0.5f);
            playerInput.SwitchCurrentActionMap("Player");
        }

    }



    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        howToPlayUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        playerInput.SwitchCurrentActionMap("Player");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;

        // Start the reinitialization process after the scene reloads
        StartCoroutine(ReinitializeUI());

        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator ReinitializeUI()
    {
        yield return new WaitForSeconds(0.1f); // Small delay to ensure objects are reloaded

        pauseMenuUI = GameObject.Find("PauseMenuUI"); // Find the UI again
        howToPlayUI = GameObject.Find("HowToPlayUI"); // Find the "How to Play" UI

        if (pauseMenuUI == null)
        {
            Debug.LogError("PauseMenuUI is missing after scene reload!");
            yield break; // Stop execution if UI is missing
        }

        menuButtons = pauseMenuUI.GetComponentsInChildren<Button>();
        eventSystem = EventSystem.current; // Ensure EventSystem is reassigned

        if (menuButtons.Length > 0)
        {
            selectedButtonIndex = 0;
        }
    }


    public void ShowHowToPlay()
    {
        pauseMenuUI.SetActive(false);
        howToPlayUI.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        howToPlayUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
}
