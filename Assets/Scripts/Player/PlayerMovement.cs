using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;

    public float speed;
    public float jumpingPower;
    public bool isFacingRight;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private float horizontal;
    private Player player;

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

        if (!IsGrounded())
        {
            anim.SetBool("Jumping", true);
        }

        if (IsGrounded())
        {
            anim.SetBool("Jumping", false);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
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
        anim.SetBool("Knockback", true);
    }

    public void KnockbackOff()
    {
        player.disabled = false;
        anim.SetBool("Knockback", false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !anim.GetBool("Blocking"))
        {
            Knockback();
        }
    }
}
