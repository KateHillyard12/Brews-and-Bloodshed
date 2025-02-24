using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Import New Input System

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickupLayerMask;
    private ObjectGrabbable objectGrabbable;

    public void OnPickUpDrop(InputAction.CallbackContext context)
    {
        if (context.performed) // Ensures action triggers only once per press
        {
            if (objectGrabbable == null)
            {
                // Code for picking up the object
                float pickupDistance = 8f;
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupDistance, pickupLayerMask))
                {
                    Debug.Log("Hit: " + raycastHit.transform.name);
                    if (raycastHit.transform.TryGetComponent(out ObjectGrabbable grabbedObject))
                    {
                        grabbedObject.Grab(objectGrabPointTransform);
                        objectGrabbable = grabbedObject;
                        Debug.Log("ObjectGrabbable");
                    }
                }
            }
            else
            {
                // Code for dropping the object
                objectGrabbable.Drop();
                objectGrabbable = null;
            }
        }
    }
}
