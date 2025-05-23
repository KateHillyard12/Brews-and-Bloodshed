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
    private SnapPoint lastSnapPointBeforeCooldown;


    // UI elements
    [SerializeField] private Slider cooldownSlider;
    [SerializeField] private GameObject cooldownUI;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        cooldownUI?.SetActive(false); // Hide cooldown UI initially
    }

    public void Grab(Transform grabPoint)
    {
        if (!canPickUp) return;

        objectGrabPointTransform = grabPoint;
        objectRigidbody.useGravity = false;
        objectRigidbody.isKinematic = false;
        isSnapping = false;

        // Release the current snap point if snapped
        if (currentSnapPoint != null)
        {
            currentSnapPoint.Release();
            currentSnapPoint = null;
        }
    }

    public bool IsBeingHeld()
    {
        return objectGrabPointTransform != null;
    }


    public void Drop()
    {
        objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;

        if (CompareTag("IgnoreSnap"))
        {
            currentSnapPoint = null;
        }

        if (currentSnapPoint != null)
        {
            SnapToPoint();
        }

        MugSnapper mugSnapper = GetComponent<MugSnapper>();
        mugSnapper?.DropMug();
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
        if (CompareTag("IgnoreSnap")) return;

        SnapPoint snapPoint = other.GetComponent<SnapPoint>();
        if (snapPoint != null && !snapPoint.isOccupied)
        {
            currentSnapPoint = snapPoint;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        SnapPoint snapPoint = other.GetComponent<SnapPoint>();
        if (snapPoint != null && snapPoint == currentSnapPoint)
        {
            currentSnapPoint.Release();
            currentSnapPoint = null;
        }
    }

    private void SnapToPoint()
    {
        if (currentSnapPoint != null)
        {
            currentSnapPoint.SnapToPoint(transform);
            isSnapping = true;
            if (currentSnapPoint.CompareTag("Machine"))
            {
                objectRigidbody.isKinematic = true;

                StartCoroutine(PickUpCooldown());
            }

            else if (currentSnapPoint.CompareTag("NPC"))
            {
                objectRigidbody.isKinematic = true; // Mug is held
                objectRigidbody.constraints = RigidbodyConstraints.FreezeAll; // Prevent further movement
            }
        }
    }

    private IEnumerator PickUpCooldown()
    {
        lastSnapPointBeforeCooldown = currentSnapPoint;

        canPickUp = false;
        cooldownUI?.SetActive(true);
        cooldownSlider.value = 0;

        float cooldownTime = 5f;
        for (float t = 0; t < cooldownTime; t += Time.deltaTime)
        {
            cooldownSlider.value = t / cooldownTime;
            yield return null;
        }

        if (currentSnapPoint != null && currentSnapPoint.CompareTag("Machine"))
        {
            if (currentSnapPoint.isOccupied && currentSnapPoint == lastSnapPointBeforeCooldown)
            {
                currentSnapPoint.Release();
            }
        }




        canPickUp = true;
        isSnapping = false;
        objectRigidbody.isKinematic = false;
        cooldownUI?.SetActive(false);
    }

    public void ResetGrabbable()
    {
        currentSnapPoint?.Release();
        currentSnapPoint = null;
        isSnapping = false;
        canPickUp = true;
    }


}