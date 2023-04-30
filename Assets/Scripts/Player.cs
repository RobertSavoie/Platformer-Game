using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    float horizontal = 0f;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Air Control
        horizontal = Input.GetAxis("Horizontal");

        // No Air Control
        /*if (Mathf.Abs(rb.velocity.y) < .01)
        {
            horizontal = Input.GetAxis("Horizontal");
        }*/

        // Moves player left and right with the arrow keys and a/d keys
        transform.Translate(horizontal * speed * Time.deltaTime * Vector2.right);

        // Press spacebar makes player jump only if he is not already jumping
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.velocity.y) < .01)
        {
            // Add a force on the y axis to make the player jump
            rb.AddRelativeForce(Vector2.up * jumpForce);
        }
    }
}
