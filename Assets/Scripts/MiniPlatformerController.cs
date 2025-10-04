using UnityEngine;
using UnityEngine.InputSystem;

public class MiniPlatformerController : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidbody2D;
    [SerializeField] float moveSpeed = 5f;

    [Header("Jump Physics")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;
    private bool jumpPressed;
    private bool wasGrounded;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    [Header("Jump Buffer")]
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    private bool jumpHeld;

    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        CheckGroundStatus();
    }

    void CheckGroundStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        Debug.Log(groundCheck);
        Debug.Log(groundCheckRadius);
        Debug.Log(groundLayer);
    }

    void Update()
    {
        Run();

        if ((isGrounded || coyoteTimeCounter > 0f) && jumpBufferCounter > 0f)
        {
            PerformJump();
        }

        // Update coyote time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Update jump buffer
        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        ApplyBetterJump();

        wasGrounded = isGrounded;
    }

    void ApplyBetterJump()
    {
        if (myRigidbody2D.linearVelocity.y < 0)
        {
            myRigidbody2D.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (myRigidbody2D.linearVelocity.y > 0 && !jumpHeld)
        {
            myRigidbody2D.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }


    void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

    void OnJump(InputValue inputValue)
    {
        jumpPressed = inputValue.isPressed;
        jumpHeld = inputValue.isPressed;
        Debug.Log("Jump");
    }

    void PerformJump()
    {
        Vector2 velocity = myRigidbody2D.linearVelocity;
        velocity.y = jumpForce;
        myRigidbody2D.linearVelocity = velocity;
        jumpPressed = false;
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, myRigidbody2D.linearVelocity.y);
        myRigidbody2D.linearVelocity = playerVelocity;
    }

}

