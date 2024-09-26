using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

    private void Awake(){
        objectRigidbody = GetComponent<Rigidbody>();
    }

    public void Grab(Transform objectGrabPointTransform){
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.useGravity = false;
    }

    public void Drop(){
        this.objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;
        
    }

    private void FixedUpdate(){
        if(objectGrabPointTransform != null){
            Vector3 newPosition = Vector3.Lerp(objectRigidbody.position, objectGrabPointTransform.position, Time.deltaTime * 10f);
            objectRigidbody.MovePosition(newPosition);
        }
    }
}
