using UnityEngine;

public class MachineTypeDisplay : MonoBehaviour
{
    public MachineType machineType;         // Machine type (e.g., Coffee, Milk)
    public string customMachineName;        // Custom name for the machine

    private void Reset()
    {
        // Set a default name based on the machine type if no custom name is set
        customMachineName = machineType.ToString();
    }
}
