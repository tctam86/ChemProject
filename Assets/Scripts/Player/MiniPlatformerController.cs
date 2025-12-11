// File: Assets/Scripts/Player/MiniPlatformerController.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class MiniPlatformerController : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D rb;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpSpeed = 10f;
    Animator anim;
    CapsuleCollider2D myCapsuleCollider;

    private Vector2 voiceMoveInput;
    private float voiceInputStopTime;
    [Tooltip("Time (seconds) for which the voice command to move is in effect")]
    [SerializeField] float voiceMoveDuration = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {

        if (Time.time > voiceInputStopTime)
        {
            voiceMoveInput = Vector2.Lerp(voiceMoveInput, Vector2.zero, Time.deltaTime * 10f);
        }

        Run();
        FlipSprite();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        if (!myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }
        if (context.performed)
        {
            rb.linearVelocity += new Vector2(0f, jumpSpeed);
        }
    }

    void Run()
    {
        float horizontalInput = Mathf.Abs(moveInput.x) > Mathf.Epsilon ? moveInput.x : voiceMoveInput.x;

        Vector2 playerVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = playerVelocity;

        bool hasHorizontalSpeed = Mathf.Abs(rb.linearVelocity.x) > Mathf.Epsilon;
        anim.SetBool("isWalking", hasHorizontalSpeed);
    }

    void FlipSprite()
    {
        bool hasHorizontalSpeed = Mathf.Abs(rb.linearVelocity.x) > Mathf.Epsilon;
        if (hasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.linearVelocity.x), 1f);
        }
    }

    public void ExecuteVoiceCommand(string command)
    {
        switch (command)
        {
            case "MOVE_LEFT":
                voiceMoveInput = new Vector2(-1, 0);
                voiceInputStopTime = Time.time + voiceMoveDuration;
                break;

            case "MOVE_RIGHT":
                voiceMoveInput = new Vector2(1, 0);
                voiceInputStopTime = Time.time + voiceMoveDuration;
                break;

            case "JUMP":
                if (myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
                {
                    rb.linearVelocity += new Vector2(0f, jumpSpeed);
                }
                break;

            case "STOP_MOVE":
                voiceMoveInput = Vector2.zero;
                break;
        }
    }
}
