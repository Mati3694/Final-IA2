using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [ReadOnly, SerializeField]
    private float characterMaxLife;
    public float CharacterMaxLife { get { return characterMaxLife; } set { characterMaxLife = value; healthBar.SetValue(characterCurrLife / characterMaxLife); } }
    [ReadOnly, SerializeField]
    private float characterCurrLife;
    public float CharacterCurrLife { get { return characterCurrLife; } set { characterCurrLife = value; healthBar.SetValue(characterCurrLife / characterMaxLife); } }

    [Header("View")]
    public HealthBar healthBar;

    public virtual IEnumerator ReceiveDmg(float dmg, CharacterModel model)
    {
        Debug.Log(name.Bold() + " received " + (dmg + " DMG").Colored(Color.red) + " from " + model.name.Bold());
        CharacterCurrLife -= dmg;
        if (CharacterCurrLife <= 0)
            Death();

        yield return null;
    }

    protected virtual void Death() { Destroy(gameObject, 0.5f); }
}
