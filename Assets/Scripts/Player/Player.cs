using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Visible In Editor
    public float maxHealth;
    public float health;
    public float maxEnergy;
    public float energy;
    public bool climbingGloves;
    public Transform currentCheckpoint;
    public Transform startPosition;
    public Slider[] slider;
    public GameObject gameOverPanel;

    // Not Visible In Editor
    [NonSerialized] public bool disabled;
    private Animator anim;
    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        UpdateHealthBar(maxHealth);
        UpdateEnergyBar(0f);
    }

    private void Update()
    {
        if (startPosition == null)
        {
            startPosition = GameObject.FindGameObjectWithTag("Start").transform;
            transform.position = startPosition.position;
        }
        if (currentCheckpoint == null)
        {
            currentCheckpoint = GameObject.FindGameObjectWithTag("Start").transform;
        }
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
            gameOverPanel.SetActive(true);
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
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
        slider[1].value = energy / maxEnergy;
    }

    public void QuitGame()
    {
        Debug.Log("Quit Button Clicked");
        Application.Quit();
    }

    public void Continue()
    {
        gameOverPanel.SetActive(false);
        Respawn();
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
        if (collision.gameObject.CompareTag("Enemy") && !disabled && !anim.GetBool("Blocking"))
        {
            playerMovement.Knockback(collision);
            UpdateHealthBar(collision.gameObject.GetComponent<Enemy>().damageToPlayer * - 1f);
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
        if (collision.CompareTag("Exit"))
        {
            SceneManager.LoadScene("Level 2");
        }
    }
}
