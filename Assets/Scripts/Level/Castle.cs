using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour, IInteractable
{
    public IEnumerator Interact(HeroModel model)
    {
        Debug.Log("CASTLE// Working... :C Earned " + World.Config.workGoldPay + " gold");
        model.playerGold += World.Config.workGoldPay;

        yield return new WaitForSeconds(1);
        FXManager.ShowPopupAt(transform.position, "+" + World.Config.workGoldPay+ " GOLD", 2, Color.yellow);
    }
}
