// Assets/Scripts/NPC/WaypointMover.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NPC))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class WaypointMover : MonoBehaviour
{
    public float moveSpeed = 2f;

    [Header("Footstep Audio Settings")]
    public AudioClip footstepSound;
    public float baseStepInterval = 0.5f;
    [Range(0f, 1f)]
    public float footstepVolume = 1f;
    [Range(0f, 0.5f)]
    public float pitchRandomize = 0.1f;
    public LayerMask groundMask;

    private NPC npc;
    private Coroutine moveCoroutine;
    private Coroutine footstepCoroutine;
    private Animator animator;
    private AudioSource audioSource;
    private CapsuleCollider2D capsuleCollider;

    void Awake()
    {
        npc = GetComponent<NPC>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    public void MoveToWaypoint(Transform waypoint, NPCDialogue nextDialogue)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        if (footstepCoroutine != null)
        {
            StopCoroutine(footstepCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveToPosition(waypoint, nextDialogue));
    }

    private IEnumerator MoveToPosition(Transform waypoint, NPCDialogue nextDialogue)
    {
        Vector3 targetPosition = waypoint.position;

        Vector3 direction = (targetPosition - transform.position).normalized;
        if (animator != null)
        {
            animator.SetFloat("moveX", direction.x);
            animator.SetFloat("moveY", direction.y);
            animator.SetBool("isWalking", true);
        }

        if (footstepSound != null && audioSource != null)
        {
            footstepCoroutine = StartCoroutine(PlayFootstepSounds());
        }

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }

        if (footstepCoroutine != null)
        {
            StopCoroutine(footstepCoroutine);
            footstepCoroutine = null;
        }

        if (nextDialogue != null)
        {
            npc.dialogueData = nextDialogue;
            npc.Interact();
        }
    }

    private IEnumerator PlayFootstepSounds()
    {
        while (true)
        {
            audioSource.pitch = 1f + Random.Range(-pitchRandomize, pitchRandomize);
            audioSource.PlayOneShot(footstepSound, footstepVolume);
            yield return new WaitForSeconds(baseStepInterval);
        }
    }
}
