using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    // Visible in Editor
    public float maxHealth;
    public Transform currentCheckpoint;
    public Slider healthSlider;

    // Not Visible in Editor
    [NonSerialized] public float health;
    [NonSerialized] public bool disabled;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        Respawn();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void CheckDeath()
    {
        if (health <= 0)
        {
            health = 0;
            spriteRenderer.enabled = false;
            disabled = true;
            Invoke(nameof(Respawn), 2f);
        }
    }

    void Respawn()
    {
        transform.position = currentCheckpoint.position;
        if (!spriteRenderer.enabled)
        {
            spriteRenderer.enabled = true;
        }
        disabled = false;
        health = maxHealth;
        UpdateHealthBar(0f);
    }

    void UpdateHealthBar(float healthChange)
    {
        health += healthChange;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        healthSlider.value = health / maxHealth;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !anim.GetBool("Blocking"))
        {
            UpdateHealthBar(-1f);
            CheckDeath();
        }
        if (collision.gameObject.CompareTag("Enemy") && anim.GetBool("Blocking"))
        {
            anim.SetTrigger("BlockFlash");
        }
        if (collision.gameObject.CompareTag("FallDeath"))
        {
            UpdateHealthBar(-100f);
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
        if (collision.CompareTag("Food"))
        {
            UpdateHealthBar(1f);
            Destroy(collision.gameObject);
        }
    }
}
