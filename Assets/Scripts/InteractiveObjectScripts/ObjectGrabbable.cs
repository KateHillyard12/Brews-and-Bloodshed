using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;
    private bool isSnapping = false;
    private bool canPickUp = true;

    private SnapPoint currentSnapPoint;

    // UI elements
    public Slider cooldownSlider;
    public GameObject cooldownUI;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        cooldownUI.SetActive(false);  // Initially hide cooldown UI
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        if (!canPickUp) return;

        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.useGravity = false;
        objectRigidbody.isKinematic = false;  // Ensure physics can interact when grabbed
        isSnapping = false;
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;

        if (currentSnapPoint != null)
        {
            SnapToPoint();
        }
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null && !isSnapping)
        {
            Vector3 newPosition = Vector3.Lerp(objectRigidbody.position, objectGrabPointTransform.position, Time.deltaTime * 10f);
            objectRigidbody.MovePosition(newPosition);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SnapPoint snapPoint = other.GetComponent<SnapPoint>();
        if (snapPoint != null && !snapPoint.isOccupied)
        {
            currentSnapPoint = snapPoint;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SnapPoint snapPoint = other.GetComponent<SnapPoint>();
        if (snapPoint == currentSnapPoint)
        {
            currentSnapPoint = null;
        }
    }

    private void SnapToPoint()
    {
        if (currentSnapPoint != null)
        {
            currentSnapPoint.SnapToPoint(transform);
            isSnapping = true;
            objectRigidbody.isKinematic = true;  // Make it kinematic to stop it from moving
            Debug.Log($"{currentSnapPoint.machineType} added");
            StartCoroutine(PickUpCooldown());
        }
    }

    private IEnumerator PickUpCooldown()
    {
        canPickUp = false;
        cooldownUI.SetActive(true);
        cooldownSlider.value = 0;

        float cooldownTime = 5f;
        float currentTime = 0;

        while (currentTime < cooldownTime)
        {
            currentTime += Time.deltaTime;
            cooldownSlider.value = currentTime / cooldownTime;
            yield return null;
        }

        canPickUp = true;  // Allow the mug to be picked up
        isSnapping = false;  // Reset snapping flag
        objectRigidbody.isKinematic = false;  // Allow physics to apply when picking up again
        cooldownUI.SetActive(false);  // Hide the cooldown UI
    }
}
