using System.Collections;
using System.Collections.Generic;
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
        if (!mugTransform.CompareTag("Mug"))
            return;

        if (CompareTag("NPC") && isOccupied)
        {
            Rigidbody rb = mugTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                Vector3 bounceDirection = (mugTransform.position - transform.position).normalized + Vector3.up;
                rb.AddForce(bounceDirection * 5f, ForceMode.Impulse);
            }

            return;
        }

        mugTransform.position = transform.position;
        mugTransform.rotation = transform.rotation;
        isOccupied = true;

        if (CompareTag("Machine"))
        {
            if (machineAudioSource != null && machineClip != null)
            {
                machineAudioSource.clip = machineClip;
                machineAudioSource.loop = loopAudio;
                Debug.Log("sounds yippeeeee");
                Debug.Log($"AudioSource: {machineAudioSource}, Clip: {machineAudioSource.clip}, Volume: {machineAudioSource.volume}");

                machineAudioSource.Play();
            }
        }
        else if (CompareTag("NPC"))
        {
            mugTransform.parent = transform;

            NPCInteractable npc = GetComponentInParent<NPCInteractable>();
            if (npc != null)
            {
                npc.ReceiveMug(mugTransform.gameObject);
            }
        }
    }

    public void Release()
    {
        if (CompareTag("NPC"))
            return;

        isOccupied = false;
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject obj = other.gameObject;

        ObjectGrabbable grabbable = obj.GetComponent<ObjectGrabbable>();
        bool isHeld = grabbable != null && grabbable.IsBeingHeld();

        if (obj.tag == "IgnoreSnap" && !isHeld && obj != lastBoopedObject)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDir = (obj.transform.position - transform.position).normalized + Vector3.up;
                rb.AddForce(pushDir * 5f, ForceMode.Impulse);

                if (boopSound != null)
                    boopSound.Play();

                lastBoopedObject = obj;
                boopTimer = boopCooldown;
            }

            return;
        }

        if (obj.CompareTag("Mug") && isOccupied && !isHeld && obj.transform.parent != transform && obj != lastBoopedObject)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDir = (obj.transform.position - transform.position).normalized + Vector3.up;
                rb.AddForce(pushDir * 5f, ForceMode.Impulse);

                if (boopSound != null)
                    boopSound.Play();

                lastBoopedObject = obj;
                boopTimer = boopCooldown;
            }
        }
    }

    private bool IsBeingHeld(GameObject obj)
    {
        var grabbable = obj.GetComponent<ObjectGrabbable>();
        return grabbable != null && grabbable.IsBeingHeld();
    }
}
