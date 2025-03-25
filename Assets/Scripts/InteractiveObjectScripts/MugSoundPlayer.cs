using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MugSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] collisionSounds; // Array to hold multiple sound clips
    [SerializeField] private AudioClip[] interactionSounds; // Array for pick up/drop sounds

    public void PlayRandomInteractionSound()
    {
        if (interactionSounds.Length > 0 && audioSource != null)
        {
            AudioClip randomClip = interactionSounds[Random.Range(0, interactionSounds.Length)];
            audioSource.PlayOneShot(randomClip);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionSounds.Length > 0 && audioSource != null)
        {
            // Select a random sound from the array
            AudioClip randomClip = collisionSounds[Random.Range(0, collisionSounds.Length)];
            audioSource.PlayOneShot(randomClip);
        }
    }
}