using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossModel : CharacterModel
{
    [ReadOnly]
    public float bossAtk;
    private void Start()
    {
        bossAtk = World.Config.bossAtk;
        characterMaxLife = World.Config.bossMaxLife;
        characterCurrLife = characterMaxLife;
    }

    public override void ReceiveDmg(float dmg, CharacterModel model)
    {
        base.ReceiveDmg(dmg, model);
        if (characterCurrLife > 0)
            model.ReceiveDmg(bossAtk, this);
    }
}
