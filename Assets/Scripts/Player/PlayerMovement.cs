using System;
using System.Collections;
using System.Collections.Generic;
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
    private Player player;
    Rigidbody2D rb;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.disabled) return;
        rb.velocity = new(horizontal * speed, rb.velocity.y);
        // Set the speed parameter of the animator
        anim.SetFloat("Speed", Mathf.Abs(horizontal));
        Flip();
        JumpAnimations();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (player.disabled) return;
        if (context.performed && IsGrounded())
        {
            rb.velocity = new(rb.velocity.x, jumpingPower);
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
}
