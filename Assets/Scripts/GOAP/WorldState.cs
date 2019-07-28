using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WorldState 
{
    //PLAYER
    public float playerMaxLife;
    public float playerCurrLife;
    public float playerBaseAtk;
    public int playerGold;
    public bool playerSeriouslyInjured;

    public string currentWeapon;
    public int weaponUsesRemaining;

    //ENEMY
    public float bossLife;
}
