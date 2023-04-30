using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Variables
    // Public
    public float speed;
    public float jumpForce;
    public float attackDelay;
    public int health;
    public bool airControl;

    // Private
    float horizontal = 0f;
    float lastAttackTime;
    bool facingRight;
    bool disabled;

    // Class Objects
    // Public
    public GameObject laserBeam;
    public GameObject swordRt;
    public GameObject swordLt;
    public Transform laserPositionRt;
    public Transform laserPositionLt;
    public Transform currentCheckpoint;
    public Text messageText;
    public RuntimeAnimatorController unarmedController;
    public RuntimeAnimatorController swordController;
    public RuntimeAnimatorController gunController;

    // Private
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        facingRight = true;
        disabled = false;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (disabled)
        {
            return;
        }

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
        if (horizontal > 0.01f)
        {
            spriteRenderer.flipX = false;
            facingRight = true;
        }
        else if (horizontal < -0.01f)
        {
            spriteRenderer.flipX = true;
            facingRight = false;
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
        if (!anim.GetBool("Jumping") && Mathf.Abs(rb.velocity.y) > .5)
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

        // Weapon switching
        if (Input.GetKeyDown("u"))
        {
            // Switch to unarmed controller
            anim.runtimeAnimatorController = unarmedController as RuntimeAnimatorController;
            attackDelay = 2;
        }
        if (Input.GetKeyDown("g"))
        {
            // Switch to gun controller
            anim.runtimeAnimatorController = gunController as RuntimeAnimatorController;
            attackDelay = 0.7f;
        }
        if (Input.GetKeyDown("m"))
        {
            // Switch to unarmed controller
            anim.runtimeAnimatorController = swordController as RuntimeAnimatorController;
            attackDelay = 0.2191338f;
        }
    }

    void UnarmedAttack()
    {
        DisplayMessage("I am unarmed and cannot attack...", 2.0f);
    }

    void SwordAttack()
    {
        if (facingRight)
        {
            swordRt.SetActive(true);
        }
        else
        {
            swordLt.SetActive(true);
        }
    }

    void SwordAttackDone()
    {
        if (facingRight)
        {
            swordRt.SetActive(false);
        }
        else
        {
            swordLt.SetActive(false);
        }
    }

    void LaserAttack()
    {
        if(facingRight)
        {
            Instantiate(laserBeam,laserPositionRt.position, transform.rotation);
        }
        else
        {
            Instantiate(laserBeam,laserPositionLt.position, Quaternion.Euler(0f,0f,180f));
        }
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
    void CheckDeath()
    {
        if (health <= 0)
        {
            spriteRenderer.enabled = false;
            disabled = true;
            Invoke(nameof(Respawn), 2f);
        }
    }

    void Respawn()
    {
        transform.position = currentCheckpoint.position;
        spriteRenderer.enabled = true;
        disabled = false;
        health = 25;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            health -= 5;
            CheckDeath();
        }
        if (collision.gameObject.CompareTag("FallDeath"))
        {
            health -= 1000;
            CheckDeath();
        }
    }

    // This function will run whenever the player collides with a trigger collider
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            currentCheckpoint = collision.transform;
        }
    }
}
