using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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

    private void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>(); // Get existing input system
        eventSystem = EventSystem.current;
    }

    private void Start()
    {
        menuButtons = pauseMenuUI.GetComponentsInChildren<Button>();
    }

    private void OnEnable()
    {
        Debug.Log("Pause Menu enabled");
        playerInput.actions["Pause"].performed += _ => TogglePauseMenu();
        playerInput.actions["Navigate"].performed += ctx => navigationInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Cancel"].performed += _ => ResumeGame();
        playerInput.actions["Submit"].performed += _ => SelectButton();
    }

    private void OnDisable()
    {
        playerInput.actions["Pause"].performed -= _ => TogglePauseMenu();
        playerInput.actions["Navigate"].performed -= ctx => navigationInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Cancel"].performed -= _ => ResumeGame();
        playerInput.actions["Submit"].performed -= _ => SelectButton();
    }

    private void Update()
    {
        if (isPaused)
        {
            HandleNavigation();
        }
    }

    private void HandleNavigation()
    {
        if (navigationInput.y > 0.5f)
        {
            selectedButtonIndex = Mathf.Max(0, selectedButtonIndex - 1);
        }
        else if (navigationInput.y < -0.5f)
        {
            selectedButtonIndex = Mathf.Min(menuButtons.Length - 1, selectedButtonIndex + 1);
        }
        
        eventSystem.SetSelectedGameObject(menuButtons[selectedButtonIndex].gameObject);
    }

    private void SelectButton()
    {
        menuButtons[selectedButtonIndex].onClick.Invoke();
    }

    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

        if (isPaused)
        {
            playerInput.SwitchCurrentActionMap("UI"); // Switch to UI action map
            if (menuButtons.Length > 0)
            {
                eventSystem.SetSelectedGameObject(menuButtons[0].gameObject);
            }
        }
        else
        {
            playerInput.SwitchCurrentActionMap("Player"); // Switch back to Player controls
        }
    }


    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        playerInput.SwitchCurrentActionMap("Player"); // Resume player control
    }

    public void QuitGame()
    {
        Application.Quit();
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
