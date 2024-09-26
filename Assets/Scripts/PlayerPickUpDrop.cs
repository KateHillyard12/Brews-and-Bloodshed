using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickupLayerMask;
    private ObjectGrabbable objectGrabbable;


   private void Update(){
        if(Input.GetKeyDown(KeyCode.E)){
            if(objectGrabbable == null){
                // Code for picking up the object
                float pickupDistance = 2f;
                if(Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupDistance, pickupLayerMask)){
                    Debug.Log("Hit: " + raycastHit.transform.name);
                    if(raycastHit.transform.TryGetComponent(out ObjectGrabbable grabbedObject)){
                        grabbedObject.Grab(objectGrabPointTransform);
                        objectGrabbable = grabbedObject;
                        Debug.Log("ObjectGrabbable");
                    }
                }
            } else {
                // Code for dropping the object
                objectGrabbable.Drop();
                objectGrabbable = null;
            }
        }
    }
}

