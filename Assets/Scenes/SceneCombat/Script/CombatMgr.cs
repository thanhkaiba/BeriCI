using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public enum Team
{
    A,
    B,
    NONE,
    BOTH,
};

public class CombatMgr : MonoBehaviour
{
    public CombatState combatState;
    public UIIngameMgr UIMgr;
    public int actionCount = 0;
    public static CombatMgr Instance;
    private bool serverGame = false;
    private List<CombatAction> listActions;
    public byte yourTeamIndex = 0;
    private void Start()
    {
        Instance = this;
        if (UIMgr == null) UIMgr = GameObject.Find("UI_Ingame").GetComponent<UIIngameMgr>();
        Time.timeScale = 1;

        //return;
        PreparingGame();
        StartCoroutine(StartGame());
    }
    public void PreparingGame()
    {
        actionCount = 0;
        if (TempCombatData.Instance.waitForAServerGame)
        {
            combatState.CreateTeamFromServer();
            TempCombatData.Instance.waitForAServerGame = false;
            listActions = TempCombatData.Instance.ca;
            yourTeamIndex = TempCombatData.Instance.yourTeamIndex;
            serverGame = true;
        }
        else
        {
            combatState.CreateDemoTeam();
            serverGame = false;
        }
        
        combatState.CalculateClassBonus();
        combatState.UpdateGameWithClassBonus();
        // ui
        UIMgr.UpdateTotalHealth();
        UIMgr.InitListSailorInQueue(combatState.GetQueueNextActionSailor());
        UIMgr.ShowClassBonus(combatState.classBonusA, combatState.classBonusB);
        UIMgr.ShowActionCount(actionCount);
    }
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1);
        ActivateAllStartPassive();
        yield return new WaitForSeconds(1);
        combatState.status = CombatStatus.STARTED;
        if (!serverGame) CombatLoop();
        else CombatLoopServer();
    }
    private void ActivateAllStartPassive()
    {
        combatState.GetAllAliveCombatSailors().ForEach(sailor =>
        {
            sailor.ActiveStartPassive();
        });
    }
    void CombatLoop()
    {
        int speedAdd = CalculateSpeedAddThisLoop();
        //Debug.Log(" ----> speedAdd: " + speedAdd);
        CombatSailor actionChar = SpeedUpAllSailors(speedAdd);
        actionCount++;
        UIMgr.UpdateListSailorInQueue(combatState.GetQueueNextActionSailor());
        StartCoroutine(WaitAndDo(0.3f, () => UIMgr.ShowActionCount(actionCount) ));
        Debug.Log(
            " ----> combat action character: " + actionChar.Model.config_stats.root_name
            + " | team: " + (actionChar.cs.team == Team.A ? "A" : "B")
            + " | position: " + actionChar.cs.position
        );
        float delayTime = actionChar.DoCombatAction(combatState) + 0.2f;
        combatState.lastTeamAction = actionChar.cs.team;
        StartCoroutine(WaitAndDo(2*delayTime/3, () => UIMgr.UpdateListSailorInQueue(combatState.GetQueueNextActionSailor()) ));
        StartCoroutine(EndLoop(actionChar, delayTime));
        StartCoroutine(NextLoop(delayTime));
    }

    // serverGame
    void CombatLoopServer()
    {
        //Debug.Log(" ----> speedAdd: " + speedAdd);
        float delayTime = ProcessAction();

        actionCount++;

        StartCoroutine(WaitAndDo(2 * delayTime / 3, () => UIMgr.UpdateListSailorInQueue(combatState.GetQueueNextActionSailor())));
        StartCoroutine(NextLoopServer(delayTime));
    }
    private float ProcessAction()
    {
        if (listActions.Count == 0)
        {
            GameOver(CheckTeamWin());
            return 0;
        }
        CombatAction actionProcess = listActions[actionCount];
        int speedAdd = CalculateSpeedAddThisLoop();
        SpeedUpAllSailors(speedAdd);
        UIMgr.UpdateListSailorInQueue(combatState.GetQueueNextActionSailor());
        UIMgr.ShowActionCount(actionCount);

        switch (actionProcess.type)
        {
            case CombatAcionType.BaseAttack:
                {
                    CombatSailor actor = GetActorAction(actionProcess);
                    CombatSailor target = GetTargetAction(actionProcess);
                    Debug.Log("actionProcess: " + actionCount);
                    Debug.Log("actor id: " + actionProcess.actor);
                    Debug.Log("target id: " + actionProcess.target);
                    float targetHealthLose = actionProcess._params[0];
                    float healthWildGain = 0;
                    if (actionProcess._params.Count > 1) healthWildGain = actionProcess._params[1];
                    float delayTime = actor.BaseAttack(target, actionProcess.haveCrit, targetHealthLose, healthWildGain) + 0.2f;
                    combatState.lastTeamAction = actor.cs.team;
                    StartCoroutine(EndLoop(actor, delayTime));
                    return delayTime;
                }
            case CombatAcionType.UseSkill:
                {
                    CombatSailor actor = GetActorAction(actionProcess);
                    float delayTime = actor.ProcessSkill(actionProcess.targets, actionProcess._params);
                    Debug.Log("actionProcess: " + actionCount);
                    combatState.lastTeamAction = actor.cs.team;
                    StartCoroutine(EndLoop(actor, delayTime));
                    return delayTime;
                }
            case CombatAcionType.GameResult:
                return 0;
            case CombatAcionType.Lose:
                return 0;
            case CombatAcionType.GameState:
                return 0;
            default:
                 return 0;
        }
    }
    private CombatSailor GetActorAction(CombatAction action)
    {
        //Team team = yourTeamIndex == action.actorTeam ? Team.A : Team.B;
        return combatState.GetSailor(action.actor);
    }
    private CombatSailor GetTargetAction(CombatAction action)
    {
        //Debug.Log("action.target.id: " + action.target);
        //Debug.Log("action.actorTeam: " + action.actorTeam);
        //Team team = yourTeamIndex != action.actorTeam ? Team.A : Team.B;
        return combatState.GetSailor(action.target);
    }
    IEnumerator NextLoopServer(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (actionCount < listActions.Count) CombatLoopServer();
        else GameOver(Team.A);
    }
    //server game

    IEnumerator EndLoop(CombatSailor actor, float delay)
    {
        yield return new WaitForSeconds(delay);
        combatState.RunEndAction(actor);
    }

    IEnumerator NextLoop(float delay)
    {
        yield return new WaitForSeconds(delay);
        Team winTeam = CheckTeamWin();
        if (winTeam == Team.NONE) CombatLoop();
        else GameOver(winTeam);
    }
    IEnumerator WaitAndDo(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    void GameOver(Team winTeam)
    {
        Time.timeScale = 1;
        UIMgr.UpdateListSailorInQueue(combatState.GetQueueNextActionSailor());
        Debug.Log(">>>>>>> Game Over <<<<<<<<<");
        Debug.Log("Team " + winTeam + " win");
        //UnityEngine.SceneManagement.SceneManager.LoadScene("SceneLobby");
        StartCoroutine(WaitAndDo(2.0f, () => NetworkController.reward.SetReward(TempCombatData.Instance.caReward)));
    }

    int CalculateSpeedAddThisLoop()
    {
        int speedAdd = 99999999;
        combatState.GetAllAliveCombatSailors().ForEach(character =>
        {
            int speedNeed = character.GetSpeedNeeded();
            speedAdd = Math.Min(speedNeed, speedAdd);
        });

        return Math.Max(speedAdd, 0);
    }

    CombatSailor SpeedUpAllSailors(int speedAdd)
    {
        combatState.GetAllAliveCombatSailors().ForEach(character => character.AddCurSpeed(speedAdd));
        return combatState.GetQueueNextActionSailor().First();
    }
    public Team CheckTeamWin()
    {
        bool A_alive = combatState.GetAllTeamAliveSailors(Team.A).Count() > 0;
        bool B_alive = combatState.GetAllTeamAliveSailors(Team.B).Count() > 0;

        if (A_alive && !B_alive) return Team.A;
        if (!A_alive && B_alive) return Team.B;
        if (!A_alive && !B_alive) return Team.BOTH;
        return Team.NONE;
    }
    public void ChangeTimeScale()
    {
        if (Time.timeScale == 1) Time.timeScale = 2;
        else Time.timeScale = 1;
    }
}
