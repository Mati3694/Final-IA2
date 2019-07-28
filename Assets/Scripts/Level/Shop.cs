using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    public void Interact(HeroModel model)
    {
        if (model.playerGold < World.Config.buyWeaponGoldNeeded) {
            Debug.Log("SHOP// There's not enough gold to buy");
            return;
        }
        Debug.Log("SHOP// Buying weapon");
        model.playerGold -= World.Config.buyWeaponGoldNeeded;
        model.GetWeapon();
        model.weaponUsesRemaining = World.Config.swordMaxUses;
    }
}
