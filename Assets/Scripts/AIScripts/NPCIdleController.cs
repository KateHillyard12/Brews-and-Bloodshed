using System.Collections;
using UnityEngine;

public class NPCIdleController : MonoBehaviour
{
    public Animator animator;
    public float minIdleDelay = 5f;
    public float maxIdleDelay = 10f;
    public float idleAnimationLength = 0.5f;

    private bool hasMug = false;

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        StartCoroutine(IdleRoutine());
    }

    private IEnumerator IdleRoutine()
    {
        while (!hasMug)
        {
            animator.SetBool("IsIdling", true); // enter IdleLoop
            yield return new WaitForSeconds(idleAnimationLength); // wait for it to finish

            animator.SetBool("IsIdling", false); // exit back to neutral
            yield return new WaitForSeconds(Random.Range(2f, 4f)); // pause between flicks
        }

        animator.SetBool("IsIdling", false);
    }



    public void SwitchToMugPose()
    {
        Debug.Log("Switching to mug pose!");

        hasMug = true;
        animator.SetBool("IsIdling", false); // Stop idle loop immediately
        animator.SetTrigger("HasMug");       // Start mug-holding pose transition
    }

    public void ResetNPC()
    {
        hasMug = false;
        animator.ResetTrigger("HasMug");
        animator.SetBool("IsIdling", true);
        StartCoroutine(IdleRoutine());
    }
}
