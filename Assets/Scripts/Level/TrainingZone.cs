using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingZone : MonoBehaviour, IInteractable
{
    public IEnumerator Interact(HeroModel model)
    {
        Debug.Log("TRAINING ZONE// Estoy entrenanding :)");
        model.CharacterMaxLife +=  World.Config.trainMaxLifeIncr;
        model.playerBaseAtk += model.currentWeapon == "Sword" ? World.Config.trainAtkIncr * World.Config.trainAtkIncr : World.Config.trainAtkIncr;
        model.ConsumeWeapon(World.Config.trainWeaponUses);

        yield return new WaitForSeconds(1);
        FXManager.ShowPopupAt(transform.position, "Level Up!", 2, Color.yellow);
    }
}
