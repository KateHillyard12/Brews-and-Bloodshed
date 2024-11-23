using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickupLayerMask;
    
    private ObjectGrabbable objectGrabbable;

    private const float PickupDistance = 8f;

    private void Update()
    {
        // Handle input for picking up or dropping the object
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectGrabbable == null)
            {
                TryPickUpObject();
            }
            else
            {
                DropObject();
            }
        }
    }

    // Method to handle picking up an object
    private void TryPickUpObject()
    {
        if (TryRaycastForObject(out RaycastHit raycastHit))
        {
            if (raycastHit.transform.TryGetComponent(out ObjectGrabbable grabbedObject))
            {
                grabbedObject.Grab(objectGrabPointTransform);
                objectGrabbable = grabbedObject;
                Debug.Log("ObjectGrabbable: " + grabbedObject.name);
            }
        }
    }

    // Method to try raycasting for an object within pickup range
    private bool TryRaycastForObject(out RaycastHit raycastHit)
    {
        return Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out raycastHit, PickupDistance, pickupLayerMask);
    }

    // Method to handle dropping the object
    private void DropObject()
    {
        objectGrabbable.Drop();
        objectGrabbable = null;
    }
}
