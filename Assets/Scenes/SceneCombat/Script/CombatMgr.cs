using Piratera.Config;
using Piratera.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ModeID
{
    Test,
    PvE,
    Arena,
    Training,
};

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
    private int actionCount = 0;
    public int actionCountShow = 0;
    public static CombatMgr Instance;
    private bool serverGame = false;
    private ModeID modeId = ModeID.Test;
    private int trainLevel = 0;
    private List<CombatAction> listActions;
    public byte yourTeamIndex = 0;
    private HomefieldAdvantage defenseAdvantage;
    private void Start()
    {
#if PIRATERA_DEV
        // GlobalConfigs.InitDevConfig();
#endif
        // GlobalConfigs.InitDevConfig();
        Instance = this;
        if (UIMgr == null) UIMgr = GameObject.Find("UI_Ingame").GetComponent<UIIngameMgr>();
        GameUtils.SetTimeScale(PlayerPrefs.GetFloat($"TimeCombatScale {UserData.Instance.UID}", 1));
        //return;
        PreparingGame();
        StartCoroutine(StartGame());
        if (modeId == ModeID.Arena) ChangeBattleFieldPvP();
        if (modeId == ModeID.Training) ChangeBattleFieldTraining();
    }

    private void OnDestroy()
    {
        GameUtils.SetTimeScale(1);
    }
    public void PreparingGame()
    {
        actionCount = 0;
        actionCountShow = 0;
        if (TempCombatData.Instance.waitForAServerGame)
        {
            combatState.CreateTeamFromServer();
            TempCombatData.Instance.waitForAServerGame = false;
            modeId = TempCombatData.Instance.modeID;
            listActions = TempCombatData.Instance.ca;
            yourTeamIndex = TempCombatData.Instance.yourTeamIndex;
            serverGame = true;
            if (modeId == ModeID.Arena) defenseAdvantage = TempCombatData.Instance.defense_advantage;
        }
        else if (TempCombatData.Instance.trainingGameLevel != -1)
        {
            trainLevel = TempCombatData.Instance.trainingGameLevel;
            combatState.CreateTrainGame(trainLevel);
            TempCombatData.Instance.trainingGameLevel = -1;
            modeId = ModeID.Training;
            serverGame = false;
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
        UIMgr.ShowActionCount(actionCountShow);
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
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                var sailorA = combatState.GetSailor(Team.A, new CombatPosition(x, y));
                if (sailorA != null) sailorA.ActiveStartPassive();
                var sailorB = combatState.GetSailor(Team.B, new CombatPosition(x, y));
                if (sailorB != null) sailorB.ActiveStartPassive();
            }
        }
    }
    // serverGame
    void CombatLoopServer()
    {
        float delayTime = ProcessAction();
        actionCount++;
        StartCoroutine(WaitAndDo(2 * delayTime / 3, () => UIMgr.UpdateListSailorInQueue(combatState.GetQueueNextActionSailor())));
        StartCoroutine(NextLoopServer(delayTime));
    }
    private float ProcessAction()
    {
        if (listActions.Count == 0) return 0;
        CombatAction actionProcess = listActions[actionCount];
        int speedAdd = CalculateSpeedAddThisLoop();
        AddCurSpeedAllSailor(speedAdd);
        UIMgr.UpdateListSailorInQueue(combatState.GetQueueNextActionSailor());
        if (
            actionProcess.type == CombatAcionType.BaseAttack
            || actionProcess.type == CombatAcionType.UseSkill
            || actionProcess.type == CombatAcionType.Immobile
            )
        {
            actionCountShow++;
        }
        StartCoroutine(WaitAndDo(0.0f, () => UIMgr.ShowActionCount(actionCountShow)));

        switch (actionProcess.type)
        {
            case CombatAcionType.BaseAttack:
                {
                    CombatSailor actor = GetActorAction(actionProcess);
                    CombatSailor target = GetTargetAction(actionProcess);
                    var mapStatus = actionProcess.mapStatus;
                    /* Debug.Log("actionProcess: " + actionCount);
                     Debug.Log("actor id: " + actionProcess.actor);
                     Debug.Log("target id: " + actionProcess.target);*/
                    float targetHealthLose = actionProcess._params[0];
                    float healthWildGain = 0;
                    if (actionProcess._params.Count > 1) healthWildGain = actionProcess._params[1];
                    float delayTime = actor.BaseAttack(target, actionProcess.haveCrit, targetHealthLose, healthWildGain) + 0.2f;
                    combatState.lastTeamAction = actor.cs.team;
                    StartCoroutine(EndLoop(actor, delayTime, mapStatus));
                    return delayTime;
                }
            case CombatAcionType.UseSkill:
                {
                    CombatSailor actor = GetActorAction(actionProcess);
                    var mapStatus = actionProcess.mapStatus;
                    float delayTime = actor.ProcessSkill(actionProcess.targets, actionProcess._params);
                    Debug.Log("actionProcess: " + actionCount);
                    combatState.lastTeamAction = actor.cs.team;
                    StartCoroutine(EndLoop(actor, delayTime, mapStatus));
                    return delayTime;
                }
            case CombatAcionType.GameResult:
                {
                    Debug.Log("modeId: " + modeId);
                    GameUtils.SetTimeScale(1);
                    GameEndData data = actionProcess.gameEndData;
                    switch (modeId)
                    {
                        case ModeID.PvE:
                            {
                                StartCoroutine(WaitAndDo(0.5f, () => ShowResult(data)));
                                break;
                            }
                        case ModeID.Arena:
                            {
                                StartCoroutine(WaitAndDo(0.5f, () => ShowPvPResult((GameEndPvPData)data)));
                                break;
                            }
                    }
                    return 0;
                }
            case CombatAcionType.Immobile:
                {
                    CombatSailor actor = GetActorAction(actionProcess);
                    var mapStatus = actionProcess.mapStatus;
                    float delayTime = actor.RunImmobile() + 0.2f;
                    combatState.lastTeamAction = actor.cs.team;
                    StartCoroutine(EndLoop(actor, delayTime, mapStatus));
                    return delayTime;
                }
            default:
                return 0;
        }
    }
    private CombatSailor GetActorAction(CombatAction action)
    {
        return combatState.GetSailor(action.actor);
    }
    private CombatSailor GetTargetAction(CombatAction action)
    {
        return combatState.GetSailor(action.target);
    }


    IEnumerator NextLoopServer(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (actionCount < listActions.Count) CombatLoopServer();
    }
    // end server game

    // client game
    void CombatLoop()
    {
        int speedAdd = CalculateSpeedAddThisLoop();
        //Debug.Log(" ----> speedAdd: " + speedAdd);
        CombatSailor actionChar = AddCurSpeedAllSailor(speedAdd);
        actionCount++;
        actionCountShow++;
        UIMgr.UpdateListSailorInQueue(combatState.GetQueueNextActionSailor());
        StartCoroutine(WaitAndDo(0.0f, () => UIMgr.ShowActionCount(actionCountShow)));
        Debug.Log(
            " ----> combat action character: " + actionChar.Model.config_stats.root_name
            + " | team: " + (actionChar.cs.team == Team.A ? "A" : "B")
            + " | position: " + actionChar.cs.position
        );
        float delayTime = actionChar.DoCombatAction(combatState) + 0.2f;
        combatState.lastTeamAction = actionChar.cs.team;
        StartCoroutine(WaitAndDo(2 * delayTime / 3, () => UIMgr.UpdateListSailorInQueue(combatState.GetQueueNextActionSailor())));
        StartCoroutine(EndLoop(actionChar, delayTime));
        StartCoroutine(NextLoop(delayTime));
    }
    IEnumerator EndLoop(CombatSailor actor, float delay, List<MapStatusItem> mapStatus = null)
    {
        yield return new WaitForSeconds(delay);
        if (mapStatus != null) combatState.SyncServerSailorStatus(mapStatus);
        combatState.RunEndAction(actor);
    }
    IEnumerator NextLoop(float delay)
    {
        yield return new WaitForSeconds(delay);
        Team winTeam = CheckTeamWin();
        if (winTeam == Team.NONE) CombatLoop();
        else GameOver(winTeam);
    }
    void ShowResult(GameEndData d)
    {
        GameObject go = GuiManager.Instance.AddGui<GuiReward>("Prefap/GuiReward", LayerId.GUI);
        go.GetComponent<GuiReward>().SetReward(d);
    }
    void ShowPvPResult(GameEndPvPData d)
    {
        GameObject go = GuiManager.Instance.AddGui<GuiRewardPvP>("Prefap/GuiRewardPvP", LayerId.GUI);
        go.GetComponent<GuiRewardPvP>().SetReward(d);
    }
    void GameOver(Team winTeam)
    {
        Debug.Log(">>>>>>> Game Over <<<<<<<<<");
        Debug.Log("Team " + winTeam + " win");
        GameUtils.SetTimeScale(1);
        UIMgr.UpdateListSailorInQueue(combatState.GetQueueNextActionSailor());
        if (modeId == ModeID.Training)
        {
            combatState.ShowTrainComplete(trainLevel);
            StartCoroutine(WaitAndDo(0.5f, () => {
                LoadServerDataUI.NextScene = "ScenePickTeam";
                SceneManager.LoadScene("SceneLoadServerData");
            }));
        } else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SceneLoadServerData");
        }
    }
    // end client game
    IEnumerator WaitAndDo(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
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


    CombatSailor AddCurSpeedAllSailor(int speedAdd)
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
        if (Time.timeScale == 1) GameUtils.SetTimeScale(2);
        else if (Time.timeScale == 2) GameUtils.SetTimeScale(3);
        else GameUtils.SetTimeScale(1);
        PlayerPrefs.SetFloat($"TimeCombatScale {UserData.Instance.UID}", Time.timeScale);
    }
    [SerializeField]
    private SpriteRenderer battleField;
    private void ChangeBattleFieldPvP()
    {
        var scale = battleField.transform.localScale;
        battleField.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        battleField.sprite = PvPData.Instance.GetAdvantageBackgroundSprite(defenseAdvantage);
    }
    private void ChangeBattleFieldTraining()
    {
        //battleField.sprite = PvPData.Instance.GetAdvantageBackgroundSprite(defenseAdvantage);
    }
}
