using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public bool isOccupied = false;  // Flag to indicate if snap point is occupied
    public MachineType machineType;  // Type of machine this snap point belongs to

    public void SnapToPoint(Transform mugTransform)
    {
        // Move the mug to this snap point's position and rotation
        mugTransform.position = transform.position;
        mugTransform.rotation = transform.rotation;

        isOccupied = true;  // Mark the snap point as occupied
    }

    public void Release()
    {
        isOccupied = false;  // Release the snap point
    }
}
