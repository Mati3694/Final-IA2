using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/WorldStateConfig")]
public class WorldStateConfig : ScriptableObject
{
    [Header("Initial State")]
    public float playerMaxLife = 20;
    public float playerBaseAtk = 1;
    public int playerGold = 0;
    public string currentWeapon = "None";
    public int weaponUsesRemaining = 0;
    [Space()]
    public float bossMaxLife = 100;
    public float bossAtk = 8;

    [Header("Heuristic")]
    public float bossLifeWeight = 1;

    [Header("AStar Params")]
    public int maxSteps = 5000;

    [Header("Actions Parameters")]
    public float attackCost = 2;
    public float swordAtkDmg = 5;
    public float playerInjuredLife = 5;
    public float restCost = 5;
    public float restLifeHeal = 10;
    public float trainCost = 4;
    public int trainWeaponUses = 10;
    public float trainAtkIncr = 2, trainMaxLifeIncr = 5;
    public float buyWeaponCost = 1;
    public int buyWeaponGoldNeeded = 20;
    public float workCost = 4;
    public int workGoldPay = 10;
}
