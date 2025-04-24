using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource orderMusicSource;
    public AudioSource suspenseMusicSource;
    public AudioSource resolutionMusicSource;

    public AudioMixer audioMixer;
    public AudioMixerSnapshot ordersSnapshot;
    public AudioMixerSnapshot suspenseSnapshot;
    public AudioMixerSnapshot resolutionSnapshot;

    private AudioLowPassFilter orderFilter;
    private AudioLowPassFilter suspenseFilter;
    private AudioLowPassFilter resolutionFilter;

    private Coroutine volumeFadeCoroutine;
    private float currentVolume = 0.5f;
    private float targetVolume = 0.5f;

    private enum MusicState { Orders, Suspense, Resolution }
    private MusicState currentState = MusicState.Orders;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        orderMusicSource.loop = true;
        suspenseMusicSource.loop = true;
        resolutionMusicSource.loop = true;

        orderMusicSource.Play();
        suspenseMusicSource.Play();
        resolutionMusicSource.Play();

        orderFilter = orderMusicSource.GetComponent<AudioLowPassFilter>();
        suspenseFilter = suspenseMusicSource.GetComponent<AudioLowPassFilter>();
        resolutionFilter = resolutionMusicSource.GetComponent<AudioLowPassFilter>();

        PlayOrdersMusic();
    }

    public void PlayOrdersMusic(float transitionTime = 1f)
    {
        StopAllTracks();
        orderMusicSource.Play();
        ordersSnapshot.TransitionTo(transitionTime);
        currentState = MusicState.Orders;
    }

    public void PlaySuspenseMusic(float transitionTime = 1f)
    {
        StopAllTracks();
        suspenseMusicSource.Play();
        suspenseSnapshot.TransitionTo(transitionTime);
        currentState = MusicState.Suspense;
    }

    public void PlayResolutionMusic(float transitionTime = 2f)
    {
        StopAllTracks();
        resolutionMusicSource.Play();
        resolutionSnapshot.TransitionTo(transitionTime);
        currentState = MusicState.Resolution;
    }

    public void ResetMusic()
    {
        PlayOrdersMusic();
    }

    private void StopAllTracks()
    {
        orderMusicSource.Stop();
        suspenseMusicSource.Stop();
        resolutionMusicSource.Stop();
    }

    public void FadeOutAllMusic(float fadeDuration = 1f)
    {
        if (volumeFadeCoroutine != null) StopCoroutine(volumeFadeCoroutine);
        volumeFadeCoroutine = StartCoroutine(FadeVolumeTo(0f, fadeDuration));
    }

    public void FadeInAllMusic(float fadeDuration = 1f)
    {
        if (volumeFadeCoroutine != null) StopCoroutine(volumeFadeCoroutine);

        // Resume music if needed
        switch (currentState)
        {
            case MusicState.Orders:
                if (!orderMusicSource.isPlaying) orderMusicSource.Play();
                break;
            case MusicState.Suspense:
                if (!suspenseMusicSource.isPlaying) suspenseMusicSource.Play();
                break;
            case MusicState.Resolution:
                if (!resolutionMusicSource.isPlaying) resolutionMusicSource.Play();
                break;
        }

        volumeFadeCoroutine = StartCoroutine(FadeVolumeTo(targetVolume, fadeDuration));
    }

    private IEnumerator FadeVolumeTo(float target, float duration)
    {
        float start = currentVolume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            currentVolume = Mathf.Lerp(start, target, timer / duration);

            orderMusicSource.volume = currentVolume;
            suspenseMusicSource.volume = currentVolume;
            resolutionMusicSource.volume = currentVolume;

            yield return null;
        }

        currentVolume = target;
        orderMusicSource.volume = currentVolume;
        suspenseMusicSource.volume = currentVolume;
        resolutionMusicSource.volume = currentVolume;
    }

    public void ApplyLowPass(bool enabled, float duration = 1f)
    {
        float targetCutoff = enabled ? 800f : 22000f;
        StartCoroutine(LerpLowPass(targetCutoff, duration));
    }

    private IEnumerator LerpLowPass(float target, float duration)
    {
        float start = orderFilter.cutoffFrequency;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float freq = Mathf.Lerp(start, target, time / duration);

            orderFilter.cutoffFrequency = freq;
            suspenseFilter.cutoffFrequency = freq;
            resolutionFilter.cutoffFrequency = freq;

            yield return null;
        }

        orderFilter.cutoffFrequency = target;
        suspenseFilter.cutoffFrequency = target;
        resolutionFilter.cutoffFrequency = target;
    }
}
