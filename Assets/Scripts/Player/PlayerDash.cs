using System;
using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    // Visible In Editor
    [SerializeField] private TrailRenderer tr;

    // Not Visible In Editor
    [NonSerialized] public bool canDash = true;
    [NonSerialized] public bool isDashing;
    private readonly float dashingPower = 24f;
    private readonly float dashingTime = 0.2f;
    private readonly float dashingCooldown = 1f;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
