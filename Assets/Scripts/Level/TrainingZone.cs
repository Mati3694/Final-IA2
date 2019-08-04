using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingZone : MonoBehaviour, IInteractable
{
    public IEnumerator Interact(HeroModel model)
    {
        Debug.Log("TRAINING ZONE// Estoy entrenanding :)");
        yield return new WaitForSeconds(1);
        model.CharacterMaxLife +=  World.Config.trainMaxLifeIncr;
        model.playerBaseAtk += model.currentWeapon == "Sword" ? World.Config.trainAtkIncr * World.Config.trainWithWeaponMultiplier : World.Config.trainAtkIncr;
        model.ConsumeWeapon(World.Config.trainWeaponUses);

        FXManager.ShowPopupAt(transform.position, "Level Up!", 2, Color.yellow);
    }
}
