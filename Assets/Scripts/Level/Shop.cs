using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    public IEnumerator Interact(HeroModel model)
    {
        if (model.playerGold < World.Config.buyWeaponGoldNeeded) {
            Debug.LogWarning("SHOP// There's not enough gold to buy");
            yield break;
        }
        yield return new WaitForSeconds(1);
        Debug.Log("SHOP// Buying weapon");

        model.playerGold -= World.Config.buyWeaponGoldNeeded;
        model.GetWeapon();
        model.weaponUsesRemaining = World.Config.swordMaxUses;

        FXManager.ShowPopupAt(transform.position, "-" + World.Config.buyWeaponGoldNeeded + " GOLD", 2, Color.yellow);
    }
}
