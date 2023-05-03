using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Visible In Editor
    public float health;
    public float damageToPlayer;
    [NonSerialized] public float speed;
}
