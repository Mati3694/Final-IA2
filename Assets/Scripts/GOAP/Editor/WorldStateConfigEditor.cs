using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldStateConfig))]
public class WorldStateConfigEditor : Editor
{
    private WorldStateConfig t;

    private void OnEnable()
    {
        t = target as WorldStateConfig;
        //if (actions == null)
        if (autoCalculateOutput)
        {
            SetupGOAP();
            goapOutput = CalculateGOAP();
        }
    }

    private void OnDisable()
    {
        GC.Collect();
    }

    string goapOutput = "";
    static bool autoCalculateOutput = false;
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (autoCalculateOutput && EditorGUI.EndChangeCheck())
        {
            SetupGOAP();
            goapOutput = CalculateGOAP();
        }

        autoCalculateOutput = GUILayout.Toggle(autoCalculateOutput, "Auto Calculate GOAP Result");

        GUILayout.Label("GOAP Result", EditorStyles.boldLabel);
        if (GUILayout.Button("Calculate GOAP Result"))
        {
            SetupGOAP();
            goapOutput = CalculateGOAP();
        }
        GUI.enabled = false;
        GUILayout.TextArea(goapOutput);
        GUI.enabled = true;
    }

    static List<GoapAction<WorldState>> actions;

    void SetupGOAP()
    {
        Func<WorldState, bool> PlayerIsAlive = (state) => state.playerCurrLife > 0;
        Func<WorldState, bool> HasSword = (state) => state.currentWeapon == "Sword";
        Func<WorldState, int, WorldState> UseSword = (state, uses) => { state.weaponUsesRemaining -= uses; if (state.weaponUsesRemaining <= 0) state.currentWeapon = "None"; return state; };
        Func<WorldState, float> GetPlayerDmg = (state) => (state.playerBaseAtk * (state.playerSeriouslyInjured ? 0.5f : 1f)) + (HasSword(state) ? t.swordAtkDmg : 0);

        actions = new List<GoapAction<WorldState>>
        {
            new GoapAction<WorldState>(
                "Attack",
                (state) => PlayerIsAlive(state), //precondition
                (state) =>
                    {
                        state.bossLife -= GetPlayerDmg(state);
                        state.playerCurrLife -= t.bossAtk;
                        if(HasSword(state))
                            state = UseSword(state,1);
                        if(state.playerCurrLife < t.playerInjuredLife) state.playerSeriouslyInjured = true;
                        return state;
                    }, //effect
                t.attackCost //cost
                ),

            new GoapAction<WorldState>(
                "Rest",
                (state) => PlayerIsAlive(state), //precondition
                (state) =>
                    {
                        state.playerCurrLife += t.restLifeHeal;
                        state.playerCurrLife = Mathf.Clamp(state.playerCurrLife, 0, state.playerMaxLife);
                        state.playerSeriouslyInjured = false;
                        return state;
                    }, //effect
                t.restCost //cost
                ),

            new GoapAction<WorldState>(
                "Train",
                (state) => PlayerIsAlive(state) && !state.playerSeriouslyInjured, //precondition
                (state) =>
                    {
                        state.playerBaseAtk += HasSword(state) ? t.trainAtkIncr * t.trainWithWeaponMultiplier : t.trainAtkIncr;
                        state.playerMaxLife += t.trainMaxLifeIncr;
                        if(HasSword(state))
                             state = UseSword(state,t.trainWeaponUses);
                        return state;
                    }, //effect
                t.trainCost //cost
                ),

            new GoapAction<WorldState>(
                "Buy Weapon",
                (state) => PlayerIsAlive(state) && state.playerGold >= t.buyWeaponGoldNeeded, //precondition
                (state) =>
                    {
                        state.currentWeapon = "Sword";
                        state.weaponUsesRemaining = t.swordMaxUses;
                        state.playerGold -= t.buyWeaponGoldNeeded;
                        return state;
                    }, //effect
                t.buyWeaponCost//cost
                ),

            new GoapAction<WorldState>(
                "Work",
                (state) => PlayerIsAlive(state) && !state.playerSeriouslyInjured, //precondition
                (state) =>
                    {
                        state.playerGold += t.workGoldPay;
                        return state;
                    }, //effect
                t.workCost//cost
                )
        };
    }

    string CalculateGOAP()
    {
        var initialState = new WorldState()
        {
            playerMaxLife = t.playerMaxLife,
            playerCurrLife = t.playerMaxLife,
            playerBaseAtk = t.playerBaseAtk,
            playerGold = t.playerGold,
            playerSeriouslyInjured = false,

            currentWeapon = t.currentWeapon,
            weaponUsesRemaining = t.weaponUsesRemaining,

            bossLife = t.bossMaxLife
        };

        var currentActionsList = GOAP.Run(initialState, (state) => state.bossLife <= 0, actions, state => state.bossLife / t.bossMaxLife * t.bossLifeWeight, t.maxSteps);

        if (currentActionsList == null) { return "No possible actions"; }

        string output = "";
        foreach (var action in currentActionsList)
        {
            output += action.name + "\n";
        }
        return output;
    }
}
