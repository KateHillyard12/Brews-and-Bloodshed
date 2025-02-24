using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("Sensitivity Settings")]
    public float mouseSensitivity = 200f;
    public float controllerSensitivity = 2.5f;

    public bool isResolutionActive = false;
    private Rigidbody rb;
    private Transform playerCamera;

    private float verticalRotation = 0f;
    public Vector3 resolutionCameraPosition = new Vector3(0f, 5.2f, 6.05f);
    private bool isTransitioning = false;
    private bool isNPCSelected = false;

    public Transform[] npcFocusPoints;
    private int currentFocusIndex = 1;
    private NPC currentFocusedNPC;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isUsingController = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        playerCamera = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if (!isResolutionActive) // Disable movement when in resolution phase
        {
            HandlePlayerMovement();
        }
    }

    private void LateUpdate()
    {
        if (!isResolutionActive) // Disable normal camera movement during resolution phase
        {
            HandleCameraRotation();
        }
    }

    private void Update()
    {
        if (isResolutionActive && !isNPCSelected)
        {
            if (!isTransitioning)
            {
                isTransitioning = true;
                StartCoroutine(SmoothCameraTransition(resolutionCameraPosition, 2f));
            }
            else
            {
                HandleFocusSwitching(); // Enable focus controls

                if (Gamepad.current != null) // Controller input check
                {
                    if (Gamepad.current.buttonSouth.wasPressedThisFrame) // "A" button
                    {
                        SelectCurrentNPC();
                    }
                }
                else if (Keyboard.current.qKey.wasPressedThisFrame) // Keyboard input check
                {
                    SelectCurrentNPC();
                }
            }
        }
    }

    private void HandlePlayerMovement()
    {
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        rb.velocity = moveDirection.normalized * moveSpeed;
    }

    private void HandleCameraRotation()
    {
        float sensitivity = isUsingController ? controllerSensitivity : mouseSensitivity;

        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        isUsingController = context.control.device is Gamepad;
    }

    public void OnFocusLeft(InputAction.CallbackContext context)
    {
        if (isResolutionActive && context.performed)
        {
            ChangeFocus(1);
        }
    }

    public void OnFocusRight(InputAction.CallbackContext context)
    {
        if (isResolutionActive && context.performed)
        {
            ChangeFocus(-1);
        }
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            FindObjectOfType<PauseMenu>().TogglePauseMenu();
        }
    }


    private void HandleFocusSwitching()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame ||
            Gamepad.current?.buttonWest.wasPressedThisFrame == true) // "X" Button on Controller
        {
            ChangeFocus(1); // Move left
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame ||
                 Gamepad.current?.buttonEast.wasPressedThisFrame == true) // "B" Button on Controller
        {
            ChangeFocus(-1); // Move right
        }
    }

    private void SelectCurrentNPC()
    {
        if (currentFocusedNPC != null)
        {
            isNPCSelected = true;
            currentFocusedNPC.SelectNPC();
        }
    }

    private void ChangeFocus(int direction)
    {
        int newFocusIndex = Mathf.Clamp(currentFocusIndex + direction, 0, npcFocusPoints.Length - 1);

        if (newFocusIndex != currentFocusIndex)
        {
            if (currentFocusedNPC != null)
            {
                currentFocusedNPC.ResetMaterial();
            }

            currentFocusIndex = newFocusIndex;
            StartCoroutine(SmoothFocusTransition(npcFocusPoints[currentFocusIndex], 0.5f));

            currentFocusedNPC = npcFocusPoints[currentFocusIndex].parent.GetComponent<NPC>();
            if (currentFocusedNPC != null)
            {
                currentFocusedNPC.HoverMaterial();
            }
        }
    }

    IEnumerator SmoothFocusTransition(Transform targetFocus, float duration)
    {
        Vector3 startPosition = playerCamera.position;
        Quaternion startRotation = playerCamera.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(targetFocus.position - playerCamera.position);

        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            playerCamera.position = resolutionCameraPosition;
            playerCamera.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        playerCamera.rotation = targetRotation;
    }

    public IEnumerator SmoothCameraTransition(Vector3 targetPosition, float duration)
    {
        Vector3 startPos = playerCamera.position;

        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            playerCamera.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }

        playerCamera.position = targetPosition;
        isTransitioning = false;
    }
}
