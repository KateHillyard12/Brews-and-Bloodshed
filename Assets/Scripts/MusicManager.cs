using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip backgroundMusic;  // Background music clip
    public AudioClip resolutionMusic;  // Resolution phase music clip
    private AudioSource audioSource;   // AudioSource reference

    private void Awake()
    {
        // Ensure only one instance of MusicManager exists
        if (FindObjectsOfType<MusicManager>().Length > 1)
        {
            Destroy(gameObject);  // Destroy duplicate instances
        }
    }

    private void Start()
    {
        // Add and configure the AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;  // Loop the background music
        audioSource.volume = 0.5f;  // Set initial volume
        audioSource.Play();  // Start background music
    }

    public void ChangeMusic(AudioClip newClip, float fadeDuration = 1f)
    {
        StartCoroutine(FadeOutAndChangeMusic(newClip, fadeDuration));
    }

    private IEnumerator FadeOutAndChangeMusic(AudioClip newClip, float fadeDuration)
    {
        float startVolume = audioSource.volume;

        // Fade out current music
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        // Switch to the new music clip
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in new music
        while (audioSource.volume < startVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

    // Call this to reset music back to original background music
    public void ResetMusic()
    {
        ChangeMusic(backgroundMusic);  // Reset to the original music
    }
}