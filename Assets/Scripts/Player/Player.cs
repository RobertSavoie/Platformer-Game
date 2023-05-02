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
    public Transform currentCheckpoint;
    public Slider[] slider;

    // Not Visible In Editor
    [NonSerialized] public float health;
    [NonSerialized] public float energy;
    [NonSerialized] public bool disabled;
    private Animator anim;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        Respawn();
    }

    public void Respawn()
    {
        Debug.Log("Respawning");
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
}
