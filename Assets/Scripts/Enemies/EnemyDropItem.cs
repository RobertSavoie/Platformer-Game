using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropItem : MonoBehaviour
{
    public GameObject upgrade;
    Enemy enemy;

    private void Start()
    {
        enemy = gameObject.GetComponent<Enemy>();
        if (GameObject.FindGameObjectWithTag("Player").GetComponent <Player>().climbingGloves)
        {
            Destroy(enemy.gameObject);
        }
    }

    private void Update()
    {
        if(enemy.health <= 0)
        {
            upgrade.SetActive(true);
            upgrade.transform.position = enemy.transform.position;
        }
    }
}
