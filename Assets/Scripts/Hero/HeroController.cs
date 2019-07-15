using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public WorldStateConfig worldConfig;

    public HeroModel model;

    WorldState initialState;
    List<GoapAction<WorldState>> actions;

    Dictionary<string, Func<IEnumerator>> actionRoutineDict = new Dictionary<string, Func<IEnumerator>>();

    void Start()
    {
        Func<WorldState, bool> PlayerIsAlive = (state) => state.playerCurrLife > 0;
        Func<WorldState, bool> HasSword = (state) => state.currentWeapon == "Sword";
        Func<WorldState, int, WorldState> UseSword = (state, uses) => { state.weaponUsesRemaining -= uses; if (state.weaponUsesRemaining <= 0) state.currentWeapon = "None"; return state; };
        Func<WorldState, float> GetPlayerDmg = (state) => (state.playerBaseAtk * (state.playerSeriouslyInjured ? 0.5f : 1f)) + (HasSword(state) ? worldConfig.swordAtkDmg : 0);

        actions = new List<GoapAction<WorldState>>
        {
            new GoapAction<WorldState>(
                "Attack",
                (state) => PlayerIsAlive(state), //precondition
                (state) =>
                    {
                        state.bossLife -= GetPlayerDmg(state);
                        state.playerCurrLife -= state.bossAtk;
                        if(HasSword(state))
                            state = UseSword(state,1);
                        if(state.playerCurrLife < worldConfig.playerInjuredLife) state.playerSeriouslyInjured = true;
                        return state;
                    }, //effect
                worldConfig.attackCost //cost
                ),

            new GoapAction<WorldState>(
                "Rest",
                (state) => PlayerIsAlive(state), //precondition
                (state) =>
                    {
                        state.playerCurrLife += worldConfig.restLifeHeal;
                        state.playerCurrLife = Mathf.Clamp(state.playerCurrLife, 0, state.playerMaxLife);
                        state.playerSeriouslyInjured = false;
                        return state;
                    }, //effect
                worldConfig.restCost //cost
                ),

            new GoapAction<WorldState>(
                "Train",
                (state) => PlayerIsAlive(state) && !state.playerSeriouslyInjured, //precondition
                (state) =>
                    {
                        state.playerBaseAtk += worldConfig.trainAtkIncr;
                        state.playerMaxLife += worldConfig.trainMaxLifeIncr;
                        if(HasSword(state))
                             state = UseSword(state,worldConfig.trainWeaponUses);
                        return state;
                    }, //effect
                worldConfig.trainCost //cost
                ),

            new GoapAction<WorldState>(
                "Buy Weapon",
                (state) => PlayerIsAlive(state) && state.playerGold >= worldConfig.buyWeaponGoldNeeded, //precondition
                (state) =>
                    {
                        state.currentWeapon = "Sword";
                        state.weaponUsesRemaining = 30;
                        state.playerGold -= worldConfig.buyWeaponGoldNeeded;
                        return state;
                    }, //effect
                worldConfig.buyWeaponCost//cost
                ),

            new GoapAction<WorldState>(
                "Work",
                (state) => PlayerIsAlive(state) && !state.playerSeriouslyInjured, //precondition
                (state) =>
                    {
                        state.playerGold += worldConfig.workGoldPay;
                        return state;
                    }, //effect
                worldConfig.workCost//cost
                )
        };

        actionRoutineDict.Add("Attack", AttackRoutine);
        actionRoutineDict.Add("Rest", RestRoutine);
        actionRoutineDict.Add("Train", TrainRoutine);
        actionRoutineDict.Add("Buy Weapon", BuyRoutine);
        actionRoutineDict.Add("Work", WorkRoutine);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            CalculateGOAP();
    }


    private List<GoapAction<WorldState>> currentActionsList;
    private void CalculateGOAP()
    {
        initialState = new WorldState()
        {
            playerMaxLife = worldConfig.playerMaxLife,
            playerCurrLife = worldConfig.playerMaxLife,
            playerBaseAtk = worldConfig.playerBaseAtk,
            playerGold = worldConfig.playerGold,
            playerSeriouslyInjured = false,

            currentWeapon = worldConfig.currentWeapon,
            weaponUsesRemaining = worldConfig.weaponUsesRemaining,

            bossLife = worldConfig.bossMaxLife,
            bossAtk = worldConfig.bossAtk
        };

        Debug.Log("Calculating GOAP...");
        currentActionsList = GOAP.Run(initialState, (state) => state.bossLife <= 0, actions, state => state.bossLife / worldConfig.bossMaxLife * worldConfig.bossLifeWeight, worldConfig.maxSteps);

        if (currentActionsList == null)
        {
            Debug.Log("No possible actions");
            return;
        }

        //foreach (var action in currentActionsList)
        //    Debug.Log(action.name);


        model.ResetModel(worldConfig);

        StopAllCoroutines();
        StartCoroutine(ExecuteGOAP());
    }

    IEnumerator ExecuteGOAP()
    {
        if (currentActionsList == null) yield break;
        Debug.Log("Executing GOAP...");
        foreach (var action in currentActionsList)
        {
            //Debug.Log("Executing action : " + action.name);
            yield return actionRoutineDict[action.name]();
            Debug.Log("Waiting for " + action.cost + " seconds");
            yield return new WaitForSeconds(action.cost);
        }

        Debug.Log("Finished GOAP");
    }

    IEnumerator AttackRoutine()
    {
        Debug.Log("ATTACKING!");
        yield return null;
    }

    IEnumerator RestRoutine()
    {
        Debug.Log("Resting. Zzz...");
        yield return null;
    }

    IEnumerator TrainRoutine()
    {
        Debug.Log("TRAINING...");
        yield return null;
    }

    IEnumerator BuyRoutine()
    {
        Debug.Log("Buying.");
        yield return null;
    }

    IEnumerator WorkRoutine()
    {
        Debug.Log("Working... :C");
        yield return null;
    }

}
