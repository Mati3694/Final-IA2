using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [ReadOnly]
    public float characterMaxLife;
    [ReadOnly]
    public float characterCurrLife;

    public virtual void ReceiveDmg(float dmg, CharacterModel model)
    {
        Debug.Log(name.Bold() + " received " + (dmg + " DMG").Colored(Color.red) + " from " + model.name.Bold());
        characterCurrLife -= dmg;
        if (characterCurrLife <= 0)
            Death();
    }

    protected virtual void Death() { Destroy(gameObject); }
}
