using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Viewable In Editor
    public float speed;
    public float jumpingPower;
    public Transform groundCheck;
    public LayerMask groundLayer;

    // Not Viewable In Editor
    [NonSerialized] public bool isFacingRight;
    private float horizontal;
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

        if (!isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (isFacingRight && horizontal < 0f)
        {
            Flip();
        }

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
            return;
        };
        horizontal = context.ReadValue<Vector2>().x;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

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

    private void Knockback()
    {
        if (isFacingRight)
        {
            KnockbackOn();
            rb.velocity = new(-5f, jumpingPower / 3);
        }
        else
        {
            KnockbackOn();
            rb.velocity = new(5f, jumpingPower / 3);
        }
    }

    public void KnockbackOn()
    {
        player.disabled = true;
        anim.SetTrigger("Knockback");
    }

    public void KnockbackOff()
    {
        player.disabled = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !anim.GetBool("Blocking"))
        {
            Knockback();
        }
        if (collision.gameObject.CompareTag("Enemy") && !anim.GetBool("Blocking"))
        {
            player.UpdateHealthBar(-1f);
            player.CheckDeath();
        }
        if (collision.gameObject.CompareTag("Enemy") && anim.GetBool("Blocking"))
        {
            anim.SetTrigger("BlockFlash");
        }
        if (collision.gameObject.CompareTag("FallDeath"))
        {
            player.UpdateHealthBar(-100f);
            player.CheckDeath();
        }
    }

    // This function will run whenever the player collides with a trigger collider
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            player.currentCheckpoint = collision.transform;
        }
        if (collision.CompareTag("Food"))
        {
            player.UpdateHealthBar(collision.GetComponent<Food>().health);
            player.UpdateEnergyBar(collision.GetComponent<Food>().energy);
            Destroy(collision.gameObject);
        }
    }
}
