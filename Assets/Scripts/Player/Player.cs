using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    // Visible in Editor
    public float maxHealth;
    public float maxEnergy;
    public Transform currentCheckpoint;
    public Slider[] slider;

    // Not Visible in Editor
    [NonSerialized] public float health;
    [NonSerialized] public float energy;
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
            DisabledOn();
            anim.SetBool("Dead", true);
            Invoke(nameof(Respawn), 2f);
        }
    }

    void Respawn()
    {
        anim.SetBool("Dead", false);
        transform.position = currentCheckpoint.position;
        DisabledOff();
        health = maxHealth;
        energy = 0f;
        UpdateHealthBar(0f);
        UpdateEnergyBar(0f);
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
            UpdateHealthBar(collision.GetComponent<Food>().health);
            UpdateEnergyBar(collision.GetComponent<Food>().energy);
            Destroy(collision.gameObject);
        }
    }
}
