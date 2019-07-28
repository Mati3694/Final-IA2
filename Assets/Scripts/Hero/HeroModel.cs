using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroModel : CharacterModel
{
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

    Rigidbody rb;
    [Header("Refs")]
    public GameObject sword;

    [Header("Movement")]
    public float moveSpeed;

    [Header("Interaction")]
    public float interactionRange = 1f;

    

    public float PlayerAtk { get { return (playerBaseAtk * (playerSeriouslyInjured ? 0.5f : 1f)) + (currentWeapon == "Sword" ? World.Config.swordAtkDmg : 0); } }


    private Vector3 startPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        sword.SetActive(currentWeapon == "Sword");
    }

    public void ResetModel(WorldStateConfig config)
    {
        characterMaxLife = config.playerMaxLife;
        characterCurrLife = characterMaxLife;
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

    public void Move(Vector3 dir)
    {
        //rb.AddForce(dir * moveSpeed, ForceMode.VelocityChange);
        dir.y = 0;
        rb.velocity += (dir * moveSpeed) * Time.deltaTime;
        Face(dir);
    }

    public void Face(Vector3 dir)
    {
        transform.forward = new Vector3(dir.x, 0, dir.z);
    }

    public void Stop()
    {
        rb.velocity *= 0f;
    }

    Collider[] results = new Collider[10];
    public void TryInteract()
    {
        if (Physics.OverlapSphereNonAlloc(transform.position, interactionRange, results) > 0)
        {
            foreach (var col in results)
            {
                if (col == null) continue;
                var interactable = col.GetComponentInParent<IInteractable>();
                if (interactable == null) continue;
                interactable.Interact(this);
            }
        }
    }

    public void TryAttack()
    {
        if (Physics.OverlapSphereNonAlloc(transform.position, interactionRange, results) > 0)
        {
            foreach (var col in results)
            {
                if (col == null) continue;
                var damageable = col.GetComponentInParent<IDamageable>();
                if (damageable == null) continue;
                if (damageable == this) continue;
                damageable.ReceiveDmg(PlayerAtk, this);
                ConsumeWeapon(1);
            }
        }
    }

    public override void ReceiveDmg(float dmg, CharacterModel model)
    {
        base.ReceiveDmg(dmg, model);
        if (characterCurrLife < World.Config.playerInjuredLife)
            playerSeriouslyInjured = true;
    }

    public void ConsumeWeapon(int usesConsumed)
    {
        weaponUsesRemaining -= usesConsumed;
        if (weaponUsesRemaining <= 0)
        {
            weaponUsesRemaining = 0;
            currentWeapon = "None";
            sword.SetActive(false);
        }
    }

    public void GetWeapon()
    {
        currentWeapon = "Sword";
        sword.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

}
