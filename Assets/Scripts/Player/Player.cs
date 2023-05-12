using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Visible In Editor
    public float maxHealth;
    public float health;
    public float maxEnergy;
    public float energy;

    // Upgrades
    public bool climbingGloves;
    public bool jumpBoots;
    public bool dashCloak;
    public bool wallHook;

    // Not Visible In Editor
    [NonSerialized] public bool disabled;
    private bool loadAtEntrance;
    private string sceneName;
    private string currentBonfireName;
    private GameObject currentCheckpoint;
    private GameObject gameManager;
    private GameManager gm;
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        gm = gameManager.GetComponent<GameManager>();
        currentBonfireName = string.Empty;
        UpdateHealthBar(maxHealth);
        UpdateEnergyBar(0f);
    }

    private void Update()
    {
        if (sceneName != string.Empty && loadAtEntrance)
        {
            foreach (GameObject entrance in gm.entrances)
            {
                if (entrance.name == sceneName)
                {
                    transform.position = entrance.transform.position;
                }
            }
        }
        else if (currentBonfireName == string.Empty)
        {
            currentBonfireName = GameObject.FindGameObjectWithTag("Bonfire").name;
            transform.position = GameObject.FindGameObjectWithTag("Bonfire").transform.position;
        }
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        gm = gameManager.GetComponent<GameManager>();
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

    public void DamageFloorRespawn()
    {
        gm.ToggleBlackScreen();
        transform.position = currentCheckpoint.transform.position;
        gm.Invoke(nameof(gm.ToggleBlackScreen), 0.5f);
    }

    public void DamageFloor()
    {
        disabled = true;
        GetComponent<PlayerMovement>().StopMovement();
        anim.SetTrigger("FloorDamage");
        UpdateHealthBar(-1f);
        Invoke(nameof(DamageFloorRespawn), 0.6f);
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
            gm.ToggleYouDiedScreen();
            Invoke(nameof(DeathScreen), 1f);
        }
    }

    public void DeathScreen()
    {
        gm.ToggleYouDiedScreen();
        gm.ToggleDeathLoadingScreen();
        SceneManager.LoadScene(PlayerPrefs.GetString("BONFIRE"));
        Invoke(nameof(Respawn), 1.5f);
    }

    public void UpdateHealthBar(float healthChange)
    {
        health += healthChange;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        gm.sliders[0].value = health / maxHealth;
        CheckDeath();
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
        }
        if (collision.gameObject.CompareTag("Enemy") && anim.GetBool("Blocking"))
        {
            anim.SetTrigger("BlockFlash");
        }
        if (collision.gameObject.CompareTag("DamageGround"))
        {
            DamageFloor();
        }
        if (collision.gameObject.CompareTag("FallDeath"))
        {
            UpdateHealthBar(-100f);
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
        if (collision.CompareTag("Checkpoint"))
        {
            currentCheckpoint = collision.gameObject;
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
            collision.GetComponent<ClimbingGloves>().anim.SetTrigger("PlayerTouch");
            collision.GetComponent<BoxCollider2D>().enabled = false;
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
