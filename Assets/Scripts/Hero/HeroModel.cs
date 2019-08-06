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
    public Animator ani;
    public ParticleSystem bloodParticles;

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
        bloodParticles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void ResetModel(WorldStateConfig config)
    {
        CharacterMaxLife = config.playerMaxLife;
        CharacterCurrLife = CharacterMaxLife;
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
    public IEnumerator TryInteract()
    {
        if (Physics.OverlapSphereNonAlloc(transform.position, interactionRange, results) > 0)
        {
            foreach (var col in results)
            {
                if (col == null) continue;
                var interactable = col.GetComponentInParent<IInteractable>();
                if (interactable == null) continue;
                yield return interactable.Interact(this);
                yield break;
            }
        }
    }

    public IEnumerator TryAttack()
    {
        if (Physics.OverlapSphereNonAlloc(transform.position, interactionRange, results) > 0)
        {
            foreach (var col in results)
            {
                if (col == null) continue;
                var damageable = col.GetComponentInParent<IDamageable>();
                if (damageable == null) continue;
                if (damageable == this) continue;

                ani.Play("Hero_Attack");
                yield return new WaitForSeconds(0.5f);
                yield return damageable.ReceiveDmg(PlayerAtk, this);
                ConsumeWeapon(1);
                yield break;
            }
        }
    }

    public override IEnumerator ReceiveDmg(float dmg, CharacterModel model)
    {
        yield return base.ReceiveDmg(dmg, model);
        if (CharacterCurrLife < World.Config.playerInjuredLife)
        { playerSeriouslyInjured = true; bloodParticles.Play(); }

        FXManager.ShowPopupAt(transform.position, "-" + dmg + " HP", 2, Color.red);
        yield return null;
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

    public IEnumerator Sleep(Bed bed)
    {
        Vector3 lastPos = transform.position;
        Quaternion lastRot = transform.rotation;
        transform.position = bed.SleepPos;
        transform.rotation = bed.transform.rotation * Quaternion.Euler(-90, 0, 0);
        rb.isKinematic = true;


        if (bloodParticles.isEmitting)
            bloodParticles.Stop(false, ParticleSystemStopBehavior.StopEmitting);

        yield return new WaitForSeconds(2);

        rb.isKinematic = false;
        transform.position = lastPos;
        transform.rotation = lastRot;

        CharacterCurrLife = Mathf.Clamp(CharacterCurrLife + World.Config.restLifeHeal, 0, CharacterMaxLife);
        playerSeriouslyInjured = false;

    }

    protected override void Death()
    {
        ani.Play("Hero_Death");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

}
