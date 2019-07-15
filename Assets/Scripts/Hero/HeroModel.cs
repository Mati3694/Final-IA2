using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroModel : MonoBehaviour
{
    Rigidbody rb;

    [Header("Movement")]
    public float moveSpeed;

    [Header("Stats")]
    [ReadOnly]
    public float playerMaxLife;
    [ReadOnly]
    public float playerCurrLife;
    [ReadOnly]
    public float playerBaseAtk;
    [ReadOnly]
    public int playerGold;
    [ReadOnly]
    public bool playerSeriouslyInjured;

    [ReadOnly]
    public string currentWeapon;
    [ReadOnly]
    public int weaponUsesRemaining;


    private Vector3 startPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    public void ResetModel(WorldStateConfig config)
    {
        playerMaxLife = config.playerMaxLife;
        playerCurrLife = playerMaxLife;
        playerSeriouslyInjured = false;

        playerBaseAtk = config.playerBaseAtk;

        currentWeapon = config.currentWeapon;
        weaponUsesRemaining = config.weaponUsesRemaining;

        playerGold = config.playerGold;

        transform.position = startPos;
        rb.velocity *= 0f;
    }

    private void FixedUpdate()
    {
        var vel = rb.velocity;
        vel.x *= 0.9f;
        vel.z *= 0.9f;
        rb.velocity = vel;
    }

    public void Move(Vector2 dir)
    {
        rb.AddForce(dir * moveSpeed, ForceMode.Acceleration);
    }
}
