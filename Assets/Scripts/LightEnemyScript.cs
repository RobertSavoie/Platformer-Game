using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnemyScript : MonoBehaviour
{
    // Public
    public float speed;
    public int health;
    public int currentPatrolPointIndex = 0;
    public GameObject player;
    public Transform[] patrolPoints;
    public Transform currentPatrolPoint;

    // Private
    SpriteRenderer spriteRenderer;
    Animator anim;

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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            anim.SetTrigger("Hit");
            // Destroy laser
            Destroy(other.gameObject);
            health -= 5;
            Death();
        }
        if (other.CompareTag("Sword"))
        {
            anim.SetTrigger("Hit");
            health -= 5;
            Invoke(nameof(Death), .2f);
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

    void Death()
    {
        if (health <= 0)
        {
            // Destroy enemy
            Destroy(gameObject);
        }
    }
}
