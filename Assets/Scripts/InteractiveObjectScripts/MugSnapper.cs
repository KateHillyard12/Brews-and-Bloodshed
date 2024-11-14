using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MugSnapper : MonoBehaviour
{
    private SnapPoint currentSnapPoint = null;

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

    public void DropMug()
    {
        if (currentSnapPoint != null)
        {
            currentSnapPoint.SnapToPoint(transform);

            // Output debug message based on the type of machine
            switch (currentSnapPoint.machineType)
            {
                case MachineType.Coffee:
                    Debug.Log("Coffee added");
                    break;
                case MachineType.Milk:
                    Debug.Log("Milk added");
                    break;
                case MachineType.Syrup:
                    Debug.Log("Syrup added");
                    break;
            }
        }
        else
        {
            // Handle the normal drop behavior if no snap point is nearby
        }
    }

    public void PickUpMug()
    {
        if (currentSnapPoint != null)
        {
            currentSnapPoint.Release();
            currentSnapPoint = null;
        }
    }
}
