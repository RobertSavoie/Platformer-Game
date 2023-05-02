using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyGroundPatrol : MonoBehaviour
{
    // Visible In Editor
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private int currentPatrolPointIndex = 0;

    // Not Visible In Editor
    private Enemy enemy;
    private Transform currentPatrolPoint;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        currentPatrolPoint = patrolPoints[currentPatrolPointIndex];
        spriteRenderer = GetComponent<SpriteRenderer>();
        DirectionSwitch();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(enemy.speed * Time.deltaTime * Vector2.right);
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
            enemy.speed = -1f;
            spriteRenderer.flipX = false;
        }
        else
        {
            enemy.speed = 1f;
            spriteRenderer.flipX = true;
        }
    }
}
