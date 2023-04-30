using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnemyScript : MonoBehaviour
{
    // Public
    public float speed;
    public int currentPatrolPointIndex = 0;
    public Transform[] patrolPoints;
    public Transform currentPatrolPoint;

    // Private
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        currentPatrolPoint = patrolPoints[currentPatrolPointIndex];
        spriteRenderer = GetComponent<SpriteRenderer>();
        DirectionSwitch();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.right);
        if(Vector2.Distance(currentPatrolPoint.position, transform.position) < .46f)
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
}
