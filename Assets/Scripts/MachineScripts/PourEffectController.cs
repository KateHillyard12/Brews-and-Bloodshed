using System.Collections;
using UnityEngine;

public class PourEffectController : MonoBehaviour
{
    public ParticleSystem pourEffect;
    public float startDelay = 1f;
    public float pourDuration = 3f; // Total duration the machine processes the mug

    private Coroutine pourRoutine;

    public void StartPouring()
    {
        if (pourRoutine != null) StopCoroutine(pourRoutine);
        pourRoutine = StartCoroutine(HandlePouring());
    }

    private IEnumerator HandlePouring()
    {
        yield return new WaitForSeconds(startDelay);

        if (pourEffect != null)
            pourEffect.Play();

        yield return new WaitForSeconds(pourDuration - 1f); // stop 1 second before release

        if (pourEffect != null)
            pourEffect.Stop();
    }
}
