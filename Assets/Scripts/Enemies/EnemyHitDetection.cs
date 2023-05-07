using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHitDetection : MonoBehaviour
{
    // Visible In Editor
    public float energyGiven;

    // Not Visible In Editor
    private GameObject player;
    private Enemy enemy;
    private Animator anim;
    private SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword"))
        {
            anim.SetTrigger("Hit");
            enemy.health -= 1;
            player.GetComponent<Player>().UpdateEnergyBar(energyGiven);
            Invoke(nameof(CheckDeath), .2f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("FallDeath"))
        {
            enemy.health -= 1000;
            CheckDeath();
        }
    }

    void KnockBack()
    {
        float distanceFromPlayer = transform.position.x - player.transform.position.x;

        if (distanceFromPlayer > 0)
        {
            spriteRenderer.flipX = false;
            enemy.speed = 4.5f;
        }
        else
        {
            spriteRenderer.flipX = true;
            enemy.speed = -4.5f;
        }
    }

    void CheckDeath()
    {
        if (enemy.health <= 0)
        {
            // Destroy enemy
            Destroy(gameObject);
        }
    }
}
