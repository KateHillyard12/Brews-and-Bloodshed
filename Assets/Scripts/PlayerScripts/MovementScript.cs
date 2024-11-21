using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 200f;
    public bool isResolutionActive = false; // Flag to check if resolution is active
    private Rigidbody rb;
    private Transform playerCamera;
    private float verticalRotation = 0f;

    // Define rotation limits for resolution phase
    public float minYaw = -70f; // Minimum horizontal angle (yaw)
    public float maxYaw = 70f;  // Maximum horizontal angle (yaw)
    public float minPitch = -70f; // Minimum vertical angle (pitch)
    public float maxPitch = 70f;  // Maximum vertical angle (pitch)

    private float currentYaw = 0f; // Starting horizontal rotation (yaw)

    // Hardcoded camera position and rotation for resolution phase
    public Vector3 resolutionCameraPosition = new Vector3(0f, 5.2f, 6.05f);
    public Vector3 resolutionCameraRotation = new Vector3(13f, 180f, 0f); // Euler angles

    private bool hasSetResolutionCamera = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // We will rotate the player using mouse input
        playerCamera = Camera.main.transform;

        // Lock cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (isResolutionActive)
        {
            // During resolution phase
            if (!hasSetResolutionCamera)
            {
                hasSetResolutionCamera = true;
                // Set camera to the predefined position and rotation
                playerCamera.position = resolutionCameraPosition;
                playerCamera.rotation = Quaternion.Euler(resolutionCameraRotation);
            }

            rb.velocity = Vector3.zero; // Ensure player doesn't move
            HandleRestrictedCameraRotation(); // Apply rotation restrictions within the square
        }
        else
        {
            // Normal gameplay
            HandlePlayerMovement(); // Enable player movement
            HandleCameraRotation(); // Normal camera rotation

            // Reset the flag for the next resolution phase
            hasSetResolutionCamera = false;
        }
    }

    void HandlePlayerMovement()
    {
        // Player Movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        rb.velocity = moveDirection.normalized * moveSpeed;
    }

    void HandleCameraRotation()
    {
        // Standard camera rotation for normal gameplay
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f); // Prevent extreme up/down

        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleRestrictedCameraRotation()
    {
        // Restricted camera rotation during resolution phase
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Update yaw (horizontal rotation) based on mouseX input, with boundaries
        currentYaw += mouseX;
        currentYaw = Mathf.Clamp(currentYaw, minYaw, maxYaw); // Clamp yaw to the limits of the square (left-right limits)

        // Update pitch (vertical rotation) based on mouseY input, with boundaries
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minPitch, maxPitch); // Clamp pitch to the limits (up-down limits)

        // Apply vertical (pitch) rotation to the camera
        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 180f, 0f);

        // Apply horizontal (yaw) rotation to the player object
        transform.localRotation = Quaternion.Euler(0f, currentYaw, 0f);


        // Set the hardcoded position and rotation
        playerCamera.position = resolutionCameraPosition;
    }
}
