using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour, IInteractable
{
    public IEnumerator Interact(HeroModel model)
    {
        yield return new WaitForSeconds(1);
        Debug.Log("CASTLE// Working... :C Earned " + World.Config.workGoldPay + " gold");
        model.playerGold += World.Config.workGoldPay;

        FXManager.ShowPopupAt(transform.position, "+" + World.Config.workGoldPay+ " GOLD", 2, Color.yellow);
    }
}
