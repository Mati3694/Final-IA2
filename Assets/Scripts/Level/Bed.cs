using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    public Vector3 SleepPos { get { return transform.position + Vector3.up * 1.5f; } }
    public IEnumerator Interact(HeroModel model)
    {
        yield return model.Sleep(this);
        //yield return new WaitForSeconds(1);
        Debug.Log("BED// Restoring " + (World.Config.restLifeHeal + " HP").Colored(Color.green) +" to Hero");

        FXManager.ShowPopupAt(transform.position, "+" + World.Config.restLifeHeal+ " HP", 2, Color.green);
    }
}
