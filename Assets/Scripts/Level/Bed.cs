using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    public IEnumerator Interact(HeroModel model)
    {
        yield return new WaitForSeconds(1);
        Debug.Log("BED// Restoring " + (World.Config.restLifeHeal + " HP").Colored(Color.green) +" to Hero");
        model.CharacterCurrLife = Mathf.Clamp(model.CharacterCurrLife + World.Config.restLifeHeal, 0, model.CharacterMaxLife);
        model.playerSeriouslyInjured = false;

        FXManager.ShowPopupAt(transform.position, "+" + World.Config.restLifeHeal+ " HP", 2, Color.green);
    }
}
