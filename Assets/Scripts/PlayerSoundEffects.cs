using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerSoundEffects : MonoBehaviour
{
    [Header("Clips")]
    public AudioClip footstepSound;
    public AudioClip jumpSound;

    [Header("Footstep Settings")]
    [Tooltip("Seconds between steps at speed 1")]
    public float baseStepInterval = 0.4f;
    [Tooltip("Minimum velocity magnitude to count as walking")]
    public float minMoveSpeed = 0.1f;
    [Range(0f, 1f)] public float footstepVolume = 0.8f;
    public float pitchRandomize = 0.05f;

    public AudioClip correctLetterSound;
    public AudioClip wrongLetterSound;
    [Range(0f, 1f)] public float letterVolume = 1f;

    private AudioSource audioSource;
    private Animator animator;
    private Rigidbody2D rb;
    private float stepTimer;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void OnEnable()
    {
        stepTimer = 0f;
    }

    void Update()
    {
        bool isWalking = animator.GetBool("isWalking");
        float speed = rb.linearVelocity.magnitude; // khớp với code hiện có
        if (isWalking && speed > minMoveSpeed && footstepSound)
        {
            float speedFactor = Mathf.Clamp(speed, 0.5f, 4f);
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = baseStepInterval / speedFactor;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    public void PlayFootstep()
    {
        if (!footstepSound) return;
        audioSource.pitch = 1f + Random.Range(-pitchRandomize, pitchRandomize);
        audioSource.PlayOneShot(footstepSound, footstepVolume);
    }

    public void PlayJump()
    {
        if (!jumpSound) return;
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(jumpSound, 1f);
    }


    public void PlayCorrectLetter()
    {
        if (!correctLetterSound) return;
        audioSource.PlayOneShot(correctLetterSound, letterVolume);

    }

    public void PlayWrongLetter()
    {
        if (!wrongLetterSound) return;
        audioSource.PlayOneShot(wrongLetterSound, letterVolume);

    }
}
