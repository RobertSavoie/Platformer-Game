using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Public
    public float speed;
    public float jumpForce;
    public float attackDelay;
    public bool airControl;
    public Text messageText;

    // Private
    float horizontal = 0f;
    float lastAttackTime;

    // Class Objects
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // AirControl
        if(airControl)
        {
            horizontal = Input.GetAxis("Horizontal");
        }
        else
        {
            // No AirControl
            if (Mathf.Abs(rb.velocity.y) < .01)
            {
                horizontal = Input.GetAxis("Horizontal");
            }
        }

        // Moves player left and right with the arrow keys and a/d keys
        transform.Translate(horizontal * speed * Time.deltaTime * Vector2.right);

        // Flip the sprite to face the direction he is moving
        if (horizontal > 0f)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontal < 0f)
        {
            spriteRenderer.flipX = true;
        }

        // Set the speed parameter of the animator
        anim.SetFloat("Speed", Mathf.Abs(horizontal));

        // Press spacebar makes player jump only if he is not already jumping
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.velocity.y) < .01)
        {
            // Add a force on the y axis to make the player jump
            rb.AddRelativeForce(Vector2.up * jumpForce);
        }

        // Tells the animator to start the jumping animation
        if (!anim.GetBool("Jumping") && Mathf.Abs(rb.velocity.y) > .05)
        {
            anim.SetBool("Jumping", true);
        }

        // Tells the animator to end the jumping animation
        if (anim.GetBool("Jumping") && Mathf.Abs(rb.velocity.y) < .05)
        {
            anim.SetBool("Jumping", false);
        }

        if (Input.GetButtonDown("Fire1") && Time.time > lastAttackTime + attackDelay)
        {
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    void UnarmedAttack()
    {
        DisplayMessage("I am unarmed and cannot attack...", 2.0f);
    }

    void DisplayMessage(string message, float displayTime)
    {
        messageText.text = message;
        Invoke(nameof(ClearMessage), displayTime);
    }

    void ClearMessage()
    {
        messageText.text = string.Empty;
    }
}
