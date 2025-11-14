using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;

public class MiniPlatformerController : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D rb;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpSpeed = 10f;
    Animator anim;
    CapsuleCollider2D myCapsuleCollider;
    Vector2 voiceMoveInput;
    float voiceInputStopTime;
    [Tooltip("Duration for which voice input affects movement after receiving a command")]
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
        FLipSprite();

    }


    void FLipSprite()
    {
        bool hasHorizontalSpeed = Mathf.Abs(rb.linearVelocity.x) > Mathf.Epsilon;
        if (hasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.linearVelocity.x), 1f);
        }
    }


    void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

    void OnJump(InputValue inputValue)
    {
        if (!myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }
        if (inputValue.isPressed)
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
        }
    }

}



