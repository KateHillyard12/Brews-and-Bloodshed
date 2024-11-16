using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MachineNameDisplay : MonoBehaviour
{
    public Camera playerCamera;             // Reference to the player's camera
    public float detectionRange = 30f;       // Range to detect machines
    public TextMeshProUGUI machineNameText; // Reference to the UI Text element

    private void Update()
    {

        DetectMachine();
    }

    private void DetectMachine()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * detectionRange, Color.red);

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange, LayerMask.GetMask("Machine")))
        {

            MachineTypeDisplay machine = hit.transform.GetComponentInParent<MachineTypeDisplay>(); // Look for MachineTypeDisplay on parent
            if (machine != null)
            {
                machineNameText.text = machine.customMachineName;
                machineNameText.gameObject.SetActive(true);
                return;
            }
        }

        machineNameText.gameObject.SetActive(false);
    }

}
