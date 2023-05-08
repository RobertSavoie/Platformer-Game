using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Visible In Editor
    public float speed;
    public float jumpingPower;
    public Transform groundCheck;
    public LayerMask groundLayer;

    // Not Visible In Editor
    [NonSerialized] public bool isFacingRight;
    [NonSerialized] public float horizontal;
    [NonSerialized] public float vertical;
    private bool doubleJump;
    private Player player;
    private PlayerLadder ladder;
    private PlayerDash dash;
    private Rigidbody2D rb;
    private Animator anim;

    // Start is called before the first frame update
    private void Start()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
        ladder = GetComponent<PlayerLadder>();
        dash = GetComponent<PlayerDash>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (player.disabled) return;
        if (dash.isDashing) return;

        Flip();
        JumpAnimations();
        if (ladder.isLadder && Mathf.Abs(vertical) > 0 && player.climbingGloves)
        {
            ladder.isClimbing = true;
        }
    }

    private void FixedUpdate()
    {
        if (dash.isDashing) return;

        rb.velocity = new(horizontal * speed, rb.velocity.y);
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
        if (player.disabled) return;

        if (!context.performed && IsGrounded())
        {
            doubleJump = false;
        }

        if (context.performed)
        {
            if (player.jumpBoots)
            {
                if (IsGrounded() || doubleJump)
                {
                    rb.velocity = new(rb.velocity.x, jumpingPower);

                    doubleJump = !doubleJump;
                }
            }
            else
            {
                if (IsGrounded())
                {
                    rb.velocity = new(rb.velocity.x, jumpingPower);
                }
            }
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (player.disabled)
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
        if (player.disabled) return;

        if (player.dashCloak)
        {
            if (context.performed && dash.canDash)
            {
                StartCoroutine(dash.Dash());
            }
        }
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

    public void StopMovement()
    {
        horizontal = 0f;
        vertical = 0f;
        anim.SetFloat("Speed", Mathf.Abs(0));
        rb.velocity = new(0, 0);
    }
}
