using System.Collections;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public bool isOccupied = false;
    public MachineType machineType;

    [SerializeField] private AudioSource boopSound;
    private GameObject lastBoopedObject = null;
    private float boopCooldown = 2f;
    private float boopTimer = 2f;

    [SerializeField] private AudioSource machineAudioSource;
    [SerializeField] private AudioClip machineClip;
    [SerializeField] private bool loopAudio = false;

    [SerializeField] private float machineProcessTime = 3f;
    [SerializeField] private PourEffectController pourEffectController;

    private GameObject currentMug;

    private bool isProcessing = false;
    private float postProcessCooldown = 0.2f; // brief window to prevent re-use


    private void Start()
    {
        if (pourEffectController != null)
            machineProcessTime = pourEffectController.pourDuration;
    }

    private void Update()
    {
        if (boopTimer > 0f)
        {
            boopTimer -= Time.deltaTime;
            if (boopTimer <= 0f)
            {
                lastBoopedObject = null;
            }
        }
    }

    public void SnapToPoint(Transform mugTransform)
    {
        if (!mugTransform.CompareTag("Mug")) return;

        // === NPC Logic ===
        if (CompareTag("NPC"))
        {
            if (isOccupied)
            {
                BounceObject(mugTransform.gameObject);
                return;
            }

            mugTransform.position = transform.position;
            mugTransform.rotation = transform.rotation;
            mugTransform.parent = transform;

            Rigidbody rb = mugTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            isOccupied = true;
            currentMug = mugTransform.gameObject;

            NPCInteractable npc = GetComponentInParent<NPCInteractable>();
            npc?.ReceiveMug(currentMug);

            return;
        }

        // === Machine Logic ===
        if (CompareTag("Machine"))
        {
            // Prevent re-use if it's already processing or cooling down
            if (isOccupied || isProcessing)
                return;

            mugTransform.position = transform.position;
            mugTransform.rotation = transform.rotation;

            Rigidbody rb = mugTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            isOccupied = true;
            currentMug = mugTransform.gameObject;

            if (machineAudioSource != null && machineClip != null)
            {
                machineAudioSource.clip = machineClip;
                machineAudioSource.loop = loopAudio;
                machineAudioSource.Play();
            }

            if (pourEffectController != null)
                pourEffectController.StartPouring();

            StartCoroutine(RunMachineProcess(currentMug));
        }
    }


    private IEnumerator RunMachineProcess(GameObject mug)
    {
        isProcessing = true;

        Debug.Log("Machine starting process...");

        yield return new WaitForSeconds(machineProcessTime);

        MugState mugState = mug.GetComponent<MugState>();
        if (mugState != null)
        {
            mugState.AddIngredient(machineType.ToString());
            Debug.Log($"Added {machineType} to mug.");
        }

        mug.transform.parent = null;
        Release();

        // Post-processing cooldown
        yield return new WaitForSeconds(postProcessCooldown);

        isProcessing = false;
    }



    public void Release()
    {
        if (CompareTag("NPC"))
            return;

        Debug.Log($"[SnapPoint] Release called on {gameObject.name}");

        isOccupied = false;
        currentMug = null;
    }


    private void OnTriggerStay(Collider other)
    {
        GameObject obj = other.gameObject;

        if (obj.CompareTag("IgnoreSnap") && !IsBeingHeld(obj) && obj != lastBoopedObject)
        {
            BounceObject(obj);
            return;
        }

        if (CompareTag("NPC") && obj.CompareTag("Mug") && obj != currentMug && !IsBeingHeld(obj))
        {
            BounceObject(obj);
        }

    }

    private void BounceObject(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Vector3 bounceDirection = (obj.transform.position - transform.position).normalized + Vector3.up;
            rb.AddForce(bounceDirection * 5f, ForceMode.Impulse);
        }

        if (boopSound != null && obj != lastBoopedObject)
        {
            boopSound.Play();
            lastBoopedObject = obj;
            boopTimer = boopCooldown;
        }
    }

    private bool IsBeingHeld(GameObject obj)
    {
        var grabbable = obj.GetComponent<ObjectGrabbable>();
        return grabbable != null && grabbable.IsBeingHeld();
    }
}
