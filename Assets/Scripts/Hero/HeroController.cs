using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroController : MonoBehaviour
{

    public HeroModel model;

    public Vector2 Position2D { get { return new Vector2(transform.position.x, transform.position.z); } }

    WorldState initialState;
    List<GoapAction<WorldState>> actions;

    Dictionary<string, Func<IEnumerator>> actionRoutineDict = new Dictionary<string, Func<IEnumerator>>();

    void Start()
    {
        Func<WorldState, bool> PlayerIsAlive = (state) => state.playerCurrLife > 0;
        Func<WorldState, bool> HasSword = (state) => state.currentWeapon == "Sword";
        Func<WorldState, int, WorldState> UseSword = (state, uses) => { state.weaponUsesRemaining -= uses; if (state.weaponUsesRemaining <= 0) state.currentWeapon = "None"; return state; };
        Func<WorldState, float> GetPlayerDmg = (state) => (state.playerBaseAtk * (state.playerSeriouslyInjured ? 0.5f : 1f)) + (HasSword(state) ? World.Config.swordAtkDmg : 0);

        actions = new List<GoapAction<WorldState>>
        {
            new GoapAction<WorldState>(
                "Attack",
                (state) => PlayerIsAlive(state), //precondition
                (state) =>
                    {
                        state.bossLife -= GetPlayerDmg(state);
                        state.playerCurrLife -= World.Config.bossAtk;
                        if(HasSword(state))
                            state = UseSword(state,1);
                        if(state.playerCurrLife < World.Config.playerInjuredLife) state.playerSeriouslyInjured = true;
                        return state;
                    }, //effect
                World.Config.attackCost //cost
                ),

            new GoapAction<WorldState>(
                "Rest",
                (state) => PlayerIsAlive(state), //precondition
                (state) =>
                    {
                        state.playerCurrLife += World.Config.restLifeHeal;
                        state.playerCurrLife = Mathf.Clamp(state.playerCurrLife, 0, state.playerMaxLife);
                        state.playerSeriouslyInjured = false;
                        return state;
                    }, //effect
                World.Config.restCost //cost
                ),

            new GoapAction<WorldState>(
                "Train",
                (state) => PlayerIsAlive(state) && !state.playerSeriouslyInjured, //precondition
                (state) =>
                    {
                        state.playerBaseAtk += HasSword(state) ? World.Config.trainAtkIncr * World.Config.trainWithWeaponMultiplier : World.Config.trainAtkIncr;
                        state.playerMaxLife += World.Config.trainMaxLifeIncr;
                        if(HasSword(state))
                             state = UseSword(state,World.Config.trainWeaponUses);
                        return state;
                    }, //effect
                World.Config.trainCost //cost
                ),

            new GoapAction<WorldState>(
                "Buy Weapon",
                (state) => PlayerIsAlive(state) && state.playerGold >= World.Config.buyWeaponGoldNeeded, //precondition
                (state) =>
                    {
                        state.currentWeapon = "Sword";
                        state.weaponUsesRemaining = World.Config.swordMaxUses;
                        state.playerGold -= World.Config.buyWeaponGoldNeeded;
                        return state;
                    }, //effect
                World.Config.buyWeaponCost//cost
                ),

            new GoapAction<WorldState>(
                "Work",
                (state) => PlayerIsAlive(state) && !state.playerSeriouslyInjured, //precondition
                (state) =>
                    {
                        state.playerGold += World.Config.workGoldPay;
                        return state;
                    }, //effect
                World.Config.workCost//cost
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
        if (currentActionsList == null && Input.GetKeyDown(KeyCode.Space))
            CalculateGOAP();

        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }


    private List<GoapAction<WorldState>> currentActionsList;
    private void CalculateGOAP()
    {
        initialState = new WorldState()
        {
            playerMaxLife = World.Config.playerMaxLife,
            playerCurrLife = World.Config.playerMaxLife,
            playerBaseAtk = World.Config.playerBaseAtk,
            playerGold = World.Config.playerGold,
            playerSeriouslyInjured = false,

            currentWeapon = World.Config.currentWeapon,
            weaponUsesRemaining = World.Config.weaponUsesRemaining,

            bossLife = World.Config.bossMaxLife
        };

        Debug.Log("--Calculating GOAP...--".Bold());
        currentActionsList = GOAP.Run(initialState, (state) => state.bossLife <= 0, actions, state => state.bossLife / World.Config.bossMaxLife * World.Config.bossLifeWeight, World.Config.maxSteps);

        if (currentActionsList == null) { Debug.Log("No possible actions"); return; }

        foreach (var action in currentActionsList)
            Debug.Log(action.name);


        model.ResetModel(World.Config);

        StopAllCoroutines();
        StartCoroutine(ExecuteGOAP());
    }

    IEnumerator ExecuteGOAP()
    {
        if (currentActionsList == null) yield break;
        Debug.Log("--Executing GOAP...--".Bold());
        foreach (var action in currentActionsList)
        {
            //Debug.Log("Executing action : " + action.name);
            yield return actionRoutineDict[action.name]();
            //Debug.Log("Waiting for " + action.cost + " seconds");
            //yield return new WaitForSeconds(action.cost);
            yield return new WaitForSeconds(1);
        }

        Debug.Log("Finished GOAP");
    }

    public float distanceToInteract = 3;

    IEnumerator AttackRoutine()
    {
        yield return PathfindTo(World.Instance.boss.position);
        yield return model.TryAttack();
    }

    IEnumerator RestRoutine()
    {
        yield return PathfindTo(World.Instance.bed.position);
        yield return model.TryInteract();
    }

    IEnumerator TrainRoutine()
    {
        yield return PathfindTo(World.Instance.train.position);
        yield return model.TryInteract();
    }

    IEnumerator BuyRoutine()
    {
        yield return PathfindTo(World.Instance.shop.position);
        yield return model.TryInteract();
    }

    IEnumerator WorkRoutine()
    {
        yield return PathfindTo(World.Instance.castle.position);
        yield return model.TryInteract();
    }


    List<Vector3> currentPath;
    IEnumerator PathfindTo(Vector3 targetPos)
    {
        currentPath = WaypointMatrix.Instance.GetPositionPathTo(transform.position, targetPos);
        int pathIndex = 0;
        while (GetDistance2D(targetPos, transform.position) > distanceToInteract && pathIndex < currentPath.Count)
        {
            model.Move((currentPath[pathIndex] - transform.position).normalized);
            if (GetDistance2D(currentPath[pathIndex], transform.position) < 1)
                pathIndex++;
            yield return null;
        }
        currentPath = null;
        model.Face(targetPos - transform.position);
    }

    public float GetDistance2D(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
    }


    private void OnDrawGizmos()
    {
        

        if (currentPath == null) return;
        Gizmos.color = Color.red;
        foreach (var node in currentPath)
        {
            Gizmos.DrawWireSphere(node, 0.5f);
        }
    }
}
