using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Visible In Editor
    // Regular Movement
    public float speed;

    // Jumping
    public float jumpingPower;
    public Transform groundCheck;
    public LayerMask groundLayer;

    // Wall Jumping
    public Transform wallCheck;
    public LayerMask wallLayer;

    // Not Visible In Editor
    // Regular Movement
    [NonSerialized] public bool isFacingRight;
    [NonSerialized] public float horizontal;

    // Jump
    [NonSerialized] public float vertical;

    // Coyote Time
    private readonly float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    // Jump Buffering
    private readonly float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    // Double Jump
    private bool doubleJump;

    // Wall Slide
    private bool isWallSliding;
    private readonly float wallSlidingSpeed = 2f;

    // Wall Jump
    private bool isWallJumping;
    private float wallJumpingDirection;
    private readonly float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private readonly float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower;

    // Componenets
    private Player player;
    private PlayerLadder ladder;
    private PlayerDash dash;
    private Rigidbody2D rb;
    private Animator anim;

    // Start is called before the first frame update
    private void Start()
    {
        isFacingRight = true;
        wallJumpingPower = new(speed, jumpingPower);
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
        ladder = GetComponent<PlayerLadder>();
        dash = GetComponent<PlayerDash>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (player.disabled || dash.isDashing || PauseManager.paused) return;

        JumpAnimations();

        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (ladder.isLadder && Mathf.Abs(vertical) > 0 && player.climbingGloves)
        {
            ladder.isClimbing = true;
        }

        if (player.wallHook)
        {
            WallSlide();
            WallJump();
        }

        if (!isWallJumping)
        {
            Flip();
        }
    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        if (player.disabled || dash.isDashing || PauseManager.paused) return;

        if (!isWallJumping)
        {
            rb.velocity = new(horizontal * speed, rb.velocity.y);
        }

        if (ladder.isClimbing)
        {
            rb.gravityScale = 0;
            rb.velocity = new(rb.velocity.x, vertical * speed);
        }
        else if (!ladder.isClimbing && ladder.leftLadder)
        {
            rb.gravityScale = 2;
            ladder.leftLadder = false;
        }
        else
        {
            anim.SetFloat("Speed", Mathf.Abs(horizontal));
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (player.disabled || dash.isDashing || PauseManager.paused) return;

        // Set double jump to false if jump button not pressed and character on the ground
        if (!context.performed && IsGrounded())
        {
            doubleJump = false;
        }

        // Jump buffer check
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Jump behaviours
        if (jumpBufferCounter > 0)
        {
            // Regular Jump
            if (!player.jumpBoots)
            {
                if (coyoteTimeCounter > 0)
                {
                    rb.velocity = new(rb.velocity.x, jumpingPower);
                    jumpBufferCounter = 0f;
                }
            }
            // Double Jump
            else
            {
                if (coyoteTimeCounter > 0 || doubleJump)
                {
                    rb.velocity = new(rb.velocity.x, jumpingPower);
                    jumpBufferCounter = 0f;
                    doubleJump = !doubleJump;
                }
            }

            // Wall Jumping
            if (wallJumpingCounter > 0)
            {
                isWallJumping = true;
                rb.velocity = new(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
                jumpBufferCounter = 0f;
                wallJumpingCounter = 0f;

                if (player.jumpBoots)
                {
                    doubleJump = !doubleJump;
                }

                if (transform.localScale.x != wallJumpingDirection)
                {
                    isFacingRight = !isFacingRight;
                    Vector3 localScale = transform.localScale;
                    localScale.x *= -1f;
                    transform.localScale = localScale;
                }

                Invoke(nameof(StopWallJumping), wallJumpingDuration);
            }
        }

        // Causes jump to be short if button not held
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (player.disabled || dash.isDashing || PauseManager.paused)
        {
            horizontal = 0f;
            vertical = 0f;
            return;
        };

        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (player.disabled || dash.isDashing || PauseManager.paused) return;

        if (player.dashCloak)
        {
            if (context.performed && dash.canDash)
            {
                StartCoroutine(dash.Dash());
            }
        }
    }

    /// <summary>
    /// Determines if player is on a wall
    /// </summary>
    /// <returns>boolean</returns>
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    /// <summary>
    /// Makes it so the player slides down the wall while attached to a wall and not jumping
    /// </summary>
    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    /// <summary>
    /// Allows player to jump off wall
    /// </summary>
    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Stops wall jumping
    /// </summary>
    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    /// <summary>
    /// Determines if the player is on the ground
    /// </summary>
    /// <returns></returns>
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    /// <summary>
    /// Flips the character on the x axis
    /// </summary>
    private void Flip()
    {
        if (!isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
        else if (isFacingRight && horizontal < 0f)
        {
            isFacingRight = !isFacingRight;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    /// <summary>
    /// Determines booleans for jump and falling animations
    /// </summary>
    private void JumpAnimations()
    {
        if (!IsGrounded() && rb.velocity.y > 0.1)
        {
            anim.SetBool("Jumping", true);
            anim.SetBool("Falling", false);
        }
        else if (!IsGrounded() && rb.velocity.y < 0.1)
        {
            anim.SetBool("Jumping", false);
            anim.SetBool("Falling", true);
        }
        else if (IsGrounded())
        {
            anim.SetBool("Jumping", false);
            anim.SetBool("Falling", false);
        }
    }

    /// <summary>
    /// Sets the direction the character is knocked back based on what direction
    /// they're hit from
    /// </summary>
    public void Knockback(Collision2D collision)
    {
        if (collision.transform.position.x > transform.position.x)
        {
            KnockbackOn();
            rb.velocity = new(-5f, jumpingPower / 3);
        }
        else if (collision.transform.position.x < transform.position.x)
        {
            KnockbackOn();
            rb.velocity = new(5f, jumpingPower / 3);
        }
    }

    /// <summary>
    /// Disables player and triggers the knockback animation
    /// </summary>
    public void KnockbackOn()
    {
        player.disabled = true;
        anim.SetTrigger("Knockback");
    }

    /// <summary>
    /// Re-enables player
    /// </summary>
    public void KnockbackOff()
    {
        player.disabled = false;
    }

    /// <summary>
    /// Stops all movement and turns off walking animation
    /// </summary>
    public void StopMovement()
    {
        horizontal = 0f;
        vertical = 0f;
        anim.SetFloat("Speed", Mathf.Abs(0));
        rb.velocity = new(0, 0);
    }
}
