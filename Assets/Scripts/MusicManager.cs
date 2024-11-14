using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip backgroundMusic;  // Assign your background music clip in the Inspector
    private AudioSource audioSource;    // Reference to the AudioSource component

    private void Awake()
    {
        // Ensure only one instance of MusicManager exists
        if (FindObjectsOfType<MusicManager>().Length > 1)
        {
            Destroy(gameObject);  // Destroy duplicate instances
        }
        else
        {
            DontDestroyOnLoad(gameObject);  // Keep this instance alive across scenes
        }
    }

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;  // Set the audio to loop
        audioSource.volume = 0.5f;  // Adjust volume (0.0f to 1.0f)
        audioSource.Play();  // Start playing the music
    }
}
