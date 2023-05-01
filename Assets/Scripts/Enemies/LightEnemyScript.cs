using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnemyScript : MonoBehaviour
{
    // Public
    public int health;
    public GameObject player;
    public Transform[] patrolPoints;

    // Private
    private float speed;
    private int currentPatrolPointIndex = 0;
    private Transform currentPatrolPoint;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        currentPatrolPoint = patrolPoints[currentPatrolPointIndex];
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        DirectionSwitch();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.right);
        if (Vector2.Distance(currentPatrolPoint.position, transform.position) < .46f)
        {
            GetNextPatrolPoint();
        }
    }

    void GetNextPatrolPoint()
    {
        if (currentPatrolPointIndex < patrolPoints.Length - 1)
        {
            currentPatrolPointIndex++;
        }
        else
        {
            currentPatrolPointIndex = 0;
        }

        currentPatrolPoint = patrolPoints[currentPatrolPointIndex];

        DirectionSwitch();
    }

    void DirectionSwitch()
    {
        if (currentPatrolPoint.position.x < transform.position.x)
        {
            speed = -1f;
            spriteRenderer.flipX = false;
        }
        else
        {
            speed = 1f;
            spriteRenderer.flipX = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Laser"))
        {
            anim.SetTrigger("Hit");
            // Destroy laser
            Destroy(collision.gameObject);
            health -= 1;
            CheckDeath();
        }
        if (collision.CompareTag("Sword"))
        {
            anim.SetTrigger("Hit");
            health -= 1;
            Invoke(nameof(CheckDeath), .2f);
        }
        if (collision.gameObject.CompareTag("FallDeath"))
        {
            health -= 1000;
            CheckDeath();
        }
    }

    void KnockBack()
    {
        float distanceFromPlayer = transform.position.x - player.transform.position.x;

        if (distanceFromPlayer > 0)
        {
            spriteRenderer.flipX = false;
            speed = 4.5f;
        }
        else
        {
            spriteRenderer.flipX = true;
            speed = -4.5f;
        }
    }

    void CheckDeath()
    {
        if (health <= 0)
        {
            // Destroy enemy
            Destroy(gameObject);
        }
    }
}
