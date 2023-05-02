using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    // Visible In Editor
    public float maxHealth;
    public float maxEnergy;
    public bool climbingGloves;
    public Transform currentCheckpoint;
    public Slider[] slider;

    // Not Visible In Editor
    [NonSerialized] public float health;
    [NonSerialized] public float energy;
    [NonSerialized] public bool disabled;
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        Respawn();
    }

    public void Respawn()
    {
        transform.position = currentCheckpoint.position;
        disabled = false;
        anim.SetBool("Dead", false);
        health = maxHealth;
        energy = 0f;
        UpdateHealthBar(0f);
        UpdateEnergyBar(0f);
    }

    public void CheckDeath()
    {
        if (anim.GetBool("Dead")) return;
        if (health <= 0)
        {
            health = 0;
            disabled = true;
            anim.SetBool("Dead", true);
            anim.SetTrigger("Death");
            Invoke(nameof(Respawn), 2f);
        }
    }

    public void UpdateHealthBar(float healthChange)
    {
        health += healthChange;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        slider[0].value = health / maxHealth;
    }

    public void UpdateEnergyBar(float energyChange)
    {
        energy += energyChange;
        if(energy > maxEnergy)
        {
            energy = maxEnergy;
        }
        slider[1].value = energy / maxEnergy;
    }

    public void DisabledOn()
    {
        disabled = true;
    }

    public void DisabledOff()
    {
        disabled = false;
    }

    // This function will run whenever the player collides with a matching tagged collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !anim.GetBool("Blocking"))
        {
            playerMovement.Knockback(collision);
        }
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
            UpdateHealthBar(collision.GetComponent<Food>().health);
            UpdateEnergyBar(collision.GetComponent<Food>().energy);
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("ClimbingGloves"))
        {
            climbingGloves = true;
            Destroy(collision.gameObject);
        }
    }
}
