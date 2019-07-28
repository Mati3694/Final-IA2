using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingZone : MonoBehaviour, IInteractable
{
    public void Interact(HeroModel model)
    {
        Debug.Log("TRAINING ZONE// Estoy entrenanding :)");
        model.characterMaxLife +=  World.Config.trainMaxLifeIncr;
        model.playerBaseAtk += model.currentWeapon == "Sword" ? World.Config.trainAtkIncr * World.Config.trainAtkIncr : World.Config.trainAtkIncr;
        model.ConsumeWeapon(World.Config.trainWeaponUses);
    }
}
