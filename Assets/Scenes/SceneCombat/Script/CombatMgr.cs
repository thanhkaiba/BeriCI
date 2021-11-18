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
        CombatAction actionProcess = listActions[actionCount];
        int speedAdd = CalculateSpeedAddThisLoop();
        SpeedUpAllSailors(speedAdd);
        UIMgr.UpdateListSailorInQueue(combatState.GetQueueNextActionSailor());
        UIMgr.ShowActionCount(actionCount);
        switch (actionProcess.type)
        {
            case CombatAcionType.BaseAttack:
                CombatSailor actionChar = GetActorAction(actionProcess);
                CombatSailor target = GetTargetAction(actionProcess);
                Debug.Log("target.cs.position: " + target.cs.position);
                Debug.Log("actionProcess: " + actionCount);
                Debug.Log("actionChar.cs.position: " + actionChar.cs.position + " " + actionChar.cs.team);
                //float delayTime = actionChar.DoCombatAction(combatState) + 0.2f;
                float delayTime = actionChar.BaseAttack(target, actionProcess.haveCrit) + 0.2f;
                combatState.lastTeamAction = actionChar.cs.team;
                StartCoroutine(EndLoop(actionChar, delayTime));
                return delayTime;
            case CombatAcionType.GameResult:
                return 0;
        }
        return 0;
    }
    private CombatSailor GetActorAction(CombatAction action)
    {
        string actor_id = action.actor.id;
        Team team = yourTeamIndex == action.actorTeam ? Team.A : Team.B;
        return combatState.GetSailor(team, actor_id);
    }
    private CombatSailor GetTargetAction(CombatAction action)
    {
        Debug.Log("action.target.id: " + action.target.id);
        Debug.Log("action.actorTeam: " + action.actorTeam);
        string actor_id = action.target.id;
        Team team = yourTeamIndex != action.actorTeam ? Team.A : Team.B;
        return combatState.GetSailor(team, actor_id);
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
        combatState.GetAllAliveCombatSailors().ForEach(character => character.SpeedUp(speedAdd));
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
