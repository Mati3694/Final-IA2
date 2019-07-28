using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour, IInteractable
{
    public void Interact(HeroModel model)
    {
        Debug.Log("CASTLE// Working... :C Earned " + World.Config.workGoldPay + " gold");
        model.playerGold += World.Config.workGoldPay;
    }
}
