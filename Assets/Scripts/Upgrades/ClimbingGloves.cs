using System;
using UnityEngine;

public class ClimbingGloves : MonoBehaviour
{
    // Not Visible In Editor
    private GameObject player;
    [NonSerialized] public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        if (player.GetComponent<Player>().climbingGloves)
        {
            anim.SetTrigger("PlayerTouch");
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
