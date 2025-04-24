using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject howToPlayUI;
    [SerializeField] private Button firstButton;

    public bool isPaused = false;
    private bool isRestarting = false;

    private PlayerInput playerInput;
    private EventSystem eventSystem;
    private Vector2 navigationInput;
    private int selectedButtonIndex = 0;
    private Button[] menuButtons;
    private MovementScript playerMovement;
    private Vector3 defaultButtonScale = new Vector3(0.3f, 0.3f, 0.3f);
    private Vector3 highlightedButtonScale = new Vector3(0.4f, 0.4f, 0.4f);
    private float animationSpeed = 10f;

    private void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        eventSystem = EventSystem.current;
        playerMovement = FindObjectOfType<MovementScript>();

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        if (howToPlayUI != null)
            howToPlayUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        var inputModule = FindObjectOfType<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        playerInput = FindObjectOfType<PlayerInput>();

        if (playerInput != null && inputModule != null)
        {
            playerInput.uiInputModule = inputModule;
        }

        if (pauseMenuUI != null)
        {
            menuButtons = pauseMenuUI.GetComponentsInChildren<Button>();
            if (menuButtons.Length > 0)
            {
                selectedButtonIndex = 0;
                StartCoroutine(SmoothHighlight(menuButtons[selectedButtonIndex]));
            }
            else
            {
                Debug.LogWarning("PauseMenu: No buttons found in pause menu UI.");
            }
        }

        playerInput.actions["Pause"].performed += OnPause;
        playerInput.actions["Cancel"].performed += OnCancel;
        playerInput.actions["Navigate"].performed += OnNavigate;
        playerInput.actions["Submit"].performed += OnSubmit;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (howToPlayUI != null && howToPlayUI.activeSelf)
            {
                howToPlayUI.SetActive(false);
                pauseMenuUI.SetActive(true);

                if (firstButton != null)
                {
                    eventSystem.SetSelectedGameObject(null);
                    eventSystem.SetSelectedGameObject(firstButton.gameObject);
                }
                return;
            }

            if (!playerMovement.isResolutionActive)
            {
                if (pauseMenuUI == null)
                {
                    Debug.LogWarning("pauseMenuUI is null during OnPause. Skipping toggle.");
                    return;
                }

                TogglePauseMenu();
            }
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
        if (menuButtons == null || menuButtons.Length == 0) return;

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
            StopAllCoroutines();
            foreach (Button btn in menuButtons)
                btn.transform.localScale = defaultButtonScale;

            eventSystem.SetSelectedGameObject(menuButtons[selectedButtonIndex].gameObject);
            StartCoroutine(SmoothHighlight(menuButtons[selectedButtonIndex]));
        }
    }

    private IEnumerator SmoothHighlight(Button button)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        Vector3 startScale = rect.localScale;
        Vector3 endScale = highlightedButtonScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * animationSpeed;
            rect.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        rect.localScale = endScale;
    }

    private void SelectButton()
    {
        if (menuButtons != null && menuButtons.Length > 0)
        {
            menuButtons[selectedButtonIndex].onClick.Invoke();
        }
    }

    public void TogglePauseMenu()
    {
        
        if (pauseMenuUI == null)
        {
            Debug.LogError("PauseMenuUI is null! Cannot toggle pause.");
            return;
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

            if (firstButton != null)
            {
                foreach (Button btn in menuButtons)
                    btn.transform.localScale = defaultButtonScale;

                eventSystem.SetSelectedGameObject(null);
                eventSystem.SetSelectedGameObject(firstButton.gameObject);
                selectedButtonIndex = System.Array.IndexOf(menuButtons, firstButton);
                StartCoroutine(SmoothHighlight(firstButton));
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
        pauseMenuUI?.SetActive(false);
        howToPlayUI?.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;

        MusicManager.Instance?.FadeInAllMusic(0.75f);
        MusicManager.Instance?.ApplyLowPass(false, 0.5f); 

        if (playerInput != null)
            playerInput.SwitchCurrentActionMap("Player");
        else
            Debug.LogWarning("playerInput is null on ResumeGame.");
    }



    public void QuitGame()
    {
        Application.Quit();
    }


    public void RestartGame()
    {
        if (isRestarting) return;
        isRestarting = true;

        if (MurderManager.Instance != null)
        {
            Destroy(MurderManager.Instance.gameObject);
            MurderManager.Instance = null;
        }

        if (MusicManager.Instance != null)
        {
            Destroy(MusicManager.Instance.gameObject);
            MusicManager.Instance = null;
        }

        StartCoroutine(RestartSceneWithReset());
    }


    private IEnumerator RestartSceneWithReset()
    {
        eventSystem = EventSystem.current;
        playerInput = FindObjectOfType<PlayerInput>();
        playerMovement = FindObjectOfType<MovementScript>();

        Time.timeScale = 1f;
        isPaused = false;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        pauseMenuUI = GameObject.Find("Canvas")?.transform.Find("PauseMenuUI")?.gameObject;
        howToPlayUI = GameObject.Find("Canvas")?.transform.Find("HowToPlayUI")?.gameObject;

        playerInput = FindObjectOfType<PlayerInput>();
        playerMovement = FindObjectOfType<MovementScript>();

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            menuButtons = pauseMenuUI.GetComponentsInChildren<Button>();
            selectedButtonIndex = 0;
        }

        if (howToPlayUI != null)
            howToPlayUI.SetActive(false);

        if (playerInput != null)
        {
            var inputModule = FindObjectOfType<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            playerInput.uiInputModule = inputModule;
            playerInput.SwitchCurrentActionMap("Player");
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowHowToPlay()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (howToPlayUI != null)
            howToPlayUI.SetActive(true);
        else
            Debug.LogWarning("howToPlayUI is null in ShowHowToPlay!");

        if (eventSystem == null)
            eventSystem = EventSystem.current;

        if (eventSystem != null)
            eventSystem.SetSelectedGameObject(null);
    }

    public void BackToPauseMenu()
    {
        howToPlayUI.SetActive(false);
        pauseMenuUI.SetActive(true);

        if (firstButton != null)
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(firstButton.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.actions["Pause"].performed -= OnPause;
            playerInput.actions["Cancel"].performed -= OnCancel;
            playerInput.actions["Navigate"].performed -= OnNavigate;
            playerInput.actions["Submit"].performed -= OnSubmit;
        }
    }

}
