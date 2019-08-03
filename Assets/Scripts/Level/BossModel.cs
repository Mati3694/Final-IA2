using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossModel : CharacterModel
{
    [ReadOnly]
    public float bossAtk;

    public Animator ani;

    private void Start()
    {
        bossAtk = World.Config.bossAtk;
        CharacterMaxLife = World.Config.bossMaxLife;
        CharacterCurrLife = CharacterMaxLife;
    }

    public override IEnumerator ReceiveDmg(float dmg, CharacterModel model)
    {
        yield return base.ReceiveDmg(dmg, model);
        FXManager.ShowPopupAt(transform.position, "-" + dmg + " HP", 2, new Color(0.8f,0.1f,0.8f));

        yield return new WaitForSeconds(1);
        if (CharacterCurrLife > 0)
        {
            transform.LookAt(model.transform, Vector3.up);
            ani.Play("Boss_Attack");
            yield return new WaitForSeconds(0.5f);
            yield return model.ReceiveDmg(bossAtk, this);
        }
    }

    protected override void Death()
    {
        ani.Play("Boss_Death");
    }
}
