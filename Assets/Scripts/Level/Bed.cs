using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    public void Interact(HeroModel model)
    {
        Debug.Log("BED// Restoring " + (World.Config.restLifeHeal + " HP").Colored(Color.green) +" to Hero");
        model.characterCurrLife = Mathf.Clamp(model.characterCurrLife + World.Config.restLifeHeal, 0, model.characterMaxLife);
        model.playerSeriouslyInjured = false;
    }
}
