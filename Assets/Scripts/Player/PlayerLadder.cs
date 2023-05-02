using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLadder : MonoBehaviour
{
    // Visible In Editor
    public bool isLadder;
    public bool isClimbing;

    // Not Visible In Editor
    private Rigidbody2D rb;
    private Player player;
    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isLadder && Mathf.Abs(playerMovement.vertical) > 0 && player.climbingGloves)
        {
            isClimbing = true;
        }
    }

    private void FixedUpdate()
    {
        if(isClimbing)
        {
            rb.gravityScale = 0;
            rb.velocity = new(rb.velocity.x, playerMovement.vertical * playerMovement.speed);
        }
        else
        {
            rb.gravityScale = 2;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
        }
    }
}
