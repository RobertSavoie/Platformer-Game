using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Visible In Editor
    public float maxHealth;
    public float health;
    public float maxEnergy;
    public float energy;
    public bool climbingGloves;

    // Not Visible In Editor
    public bool disabled;
    public string sceneName;
    public string currentBonfireName;
    public GameObject gameManager;
    public GameManager gm;
    public GameObject[] bonfires;
    public Rigidbody2D rb;
    private Animator anim;
    private PlayerMovement playerMovement;
    private bool loadAtEntrance;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        gm = gameManager.GetComponent<GameManager>();
        UpdateHealthBar(maxHealth);
        UpdateEnergyBar(0f);
    }

    private void Update()
    {
        if (sceneName != string.Empty && loadAtEntrance)
        {
            foreach (GameObject entrance in gameManager.GetComponent<GameManager>().entrances)
            {
                if (entrance.name == sceneName)
                {
                    transform.position = entrance.transform.position;
                }
            }
        }
        else
        {
            if (currentBonfireName == string.Empty)
            {
                currentBonfireName = GameObject.FindGameObjectWithTag("Bonfire").name;
                transform.position = GameObject.FindGameObjectWithTag("Bonfire").transform.position;
            }
        }
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        sceneName = SceneManager.GetActiveScene().name;
    }

    public void Respawn()
    {
        loadAtEntrance = false;
        sceneName = string.Empty;
        transform.position = GameObject.FindGameObjectWithTag("Bonfire").transform.position;
        disabled = false;
        anim.SetBool("Dead", false);
        health = maxHealth;
        energy = 0f;
        UpdateHealthBar(0f);
        UpdateEnergyBar(0f);
        gm.Invoke(nameof(gm.ToggleDeathLoadingScreen), 1f);
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
            gm.ToggleYouDiedMenu();
        }
    }

    public void UpdateHealthBar(float healthChange)
    {
        health += healthChange;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        gm.sliders[0].value = health / maxHealth;
    }

    public void UpdateEnergyBar(float energyChange)
    {
        energy += energyChange;
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
        gm.sliders[1].value = energy / maxEnergy;
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
            UpdateHealthBar(collision.gameObject.GetComponent<Enemy>().damageToPlayer * -1f);
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
        if (collision.CompareTag("Bonfire"))
        {
            currentBonfireName = collision.gameObject.name;
            PlayerPrefs.SetString("BONFIRE", collision.gameObject.name);
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
            PlayerPrefs.SetInt("CLIMBING_GLOVES", 1);
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("Exit"))
        {
            loadAtEntrance = true;

            GetComponent<PlayerMovement>().StopMovement();

            gm.ToggleLoadingScreen();

            SceneManager.LoadScene(collision.gameObject.name);

            gm.Invoke(nameof(gm.ToggleLoadingScreen), 1f);
        }
    }
}
