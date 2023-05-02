using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHitDetection : MonoBehaviour
{
    // Visible In Editor
    [SerializeField] private GameObject player;

    // Not Visible In Editor
    private Enemy enemy;
    private Animator anim;
    private SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword"))
        {
            anim.SetTrigger("Hit");
            enemy.health -= 1;
            player.GetComponent<Player>().UpdateEnergyBar(1f);
            Invoke(nameof(CheckDeath), .2f);
        }
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
