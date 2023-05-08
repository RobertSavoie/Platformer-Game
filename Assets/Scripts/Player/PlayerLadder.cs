using System;
using UnityEngine;

public class PlayerLadder : MonoBehaviour
{
    // Visible In Editor
    [NonSerialized] public bool isLadder;
    [NonSerialized] public bool isClimbing;
    [NonSerialized] public bool leftLadder;

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
            leftLadder = true;
        }
    }
}
