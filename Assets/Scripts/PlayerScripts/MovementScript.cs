using System.Collections;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 200f;
    public bool isResolutionActive = false; // Flag to check if resolution is active
    private Rigidbody rb;
    private Transform playerCamera;

    private float verticalRotation = 0f;

    // Resolution phase camera settings
    public Vector3 resolutionCameraPosition = new Vector3(0f, 5.2f, 6.05f);

    private bool isTransitioning = false; // Track if camera is transitioning
    private bool isNPCSelected = false; // Flag to prevent further actions after selection

    public Transform[] npcFocusPoints; // Array of positions to focus on each NPC
    private int currentFocusIndex = 1; // Start by looking at the center NPC (index 1)
    private NPC currentFocusedNPC; // Track currently focused NPC

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Disable physics-based rotation
        playerCamera = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked; // Lock cursor at game start
    }

    private void Update()
    {
        // Resolution phase
        if (isResolutionActive)
        {
            if (!isNPCSelected && !isTransitioning)
            {
                // Transition camera to resolution position
                isTransitioning = true;
                StartCoroutine(SmoothCameraTransition(resolutionCameraPosition, 2f));
            }
            else
            {
                HandleFocusSwitching(); // Allow player to switch focus between NPCs
                HandleNPCSelection();  // Handle NPC selection via input
            }
        }
        // Normal gameplay phase
        else
        {
            HandlePlayerMovement(); // Enable normal movement
            HandleCameraRotation(); // Normal camera rotation
        }
    }

    // Normal player movement logic
    void HandlePlayerMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        rb.velocity = moveDirection.normalized * moveSpeed;
    }

    // Normal camera rotation logic
    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f); // Prevent extreme up/down

        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // Switch between NPC focus points (left and right)
    void HandleFocusSwitching()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeFocus(1); // Move focus left
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeFocus(-1); // Move focus right
        }
    }

    // Handle NPC selection when Q key is pressed
    void HandleNPCSelection()
    {
        if (Input.GetKeyDown(KeyCode.Q) && currentFocusedNPC != null)
        {
            isNPCSelected = true; // Prevent further focus or selection
            currentFocusedNPC.SelectNPC(); // Mark the NPC as selected
        }
    }

    // Switch focus between NPCs
    void ChangeFocus(int direction)
    {
        int newFocusIndex = Mathf.Clamp(currentFocusIndex + direction, 0, npcFocusPoints.Length - 1);

        if (newFocusIndex != currentFocusIndex)
        {
            // Reset the previous NPC's material
            currentFocusedNPC?.ResetMaterial();

            // Update focus index and look at the new NPC
            currentFocusIndex = newFocusIndex;
            StartCoroutine(SmoothFocusTransition(npcFocusPoints[currentFocusIndex], 0.5f));

            // Update the currentFocusedNPC and highlight it
            currentFocusedNPC = npcFocusPoints[currentFocusIndex].parent.GetComponent<NPC>();
            currentFocusedNPC?.HoverMaterial();
        }
    }

    // Smooth camera transition to new NPC focus point
    IEnumerator SmoothFocusTransition(Transform targetFocus, float duration)
    {
        Quaternion startRotation = playerCamera.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(targetFocus.position - playerCamera.position);

        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            playerCamera.position = Vector3.Lerp(playerCamera.position, resolutionCameraPosition, t);
            playerCamera.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        playerCamera.rotation = targetRotation;
    }

    // Smooth transition to resolution camera position
    public IEnumerator SmoothCameraTransition(Vector3 targetPosition, float duration)
    {
        Vector3 startPos = playerCamera.position;

        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            playerCamera.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }

        playerCamera.position = targetPosition;
        isTransitioning = false; // Allow focus switching after transition
    }
}
