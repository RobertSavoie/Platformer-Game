using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    // Visible In Editor
    public float attackDelay;
    public GameObject swordRt;
    public GameObject swordLt;
    public GameObject shieldRt;
    public GameObject shieldLt;

    // Not Visible In Editor
    private float lastAttackTime;
    private Animator anim;
    private PlayerMovement playerMovement;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (player.disabled) return;
        if (context.performed && Time.time > lastAttackTime + attackDelay)
        {
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    public void Block(InputAction.CallbackContext context)
    {
        if (player.disabled) return;
        if(context.performed)
        {
            anim.SetBool("Blocking", true);
            ShieldBlocking();
        }
        if(context.canceled)
        {
            anim.SetBool("Blocking", false);
            ShieldBlockingDone();
        }
    }

    void SwordAttack()
    {
        if (playerMovement.isFacingRight)
        {
            swordRt.SetActive(true);
        }
        else
        {
            swordLt.SetActive(true);
        }
    }

    void SwordAttackDone()
    {
        if (playerMovement.isFacingRight)
        {
            swordRt.SetActive(false);
        }
        else
        {
            swordLt.SetActive(false);
        }
    }

    void ShieldBlocking()
    {
        if (playerMovement.isFacingRight)
        {
            shieldRt.SetActive(true);
        }
        else
        {
            shieldLt.SetActive(true);
        }
    }

    void ShieldBlockingDone()
    {
        if (playerMovement.isFacingRight)
        {
            shieldRt.SetActive(false);
        }
        else
        {
            shieldLt.SetActive(false);
        }
    }
}
