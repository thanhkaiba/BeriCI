using Piratera.Config;
using Piratera.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Piratera.Sound;

public enum ModeID
{
    Test,
    PvE,
    Arena,
    Training,
    Challenge,
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
    private bool watchReplay = false;
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
        if (modeId == ModeID.Arena)
        {
            ShowAdvantage();
            ChangeBattleFieldPvP();
        }
        else
        {
            advantageGO.gameObject.SetActive(false);
        }
        GameObject.Find("BtnSkip").SetActive(watchReplay || modeId == ModeID.Challenge || modeId == ModeID.Training);
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
            watchReplay = TempCombatData.Instance.isReplayMatch;
        }
        else if (TempCombatData.Instance.trainingGameLevel != -1)
        {
            trainLevel = TempCombatData.Instance.trainingGameLevel;
            combatState.CreateTrainGame(trainLevel);
            TempCombatData.Instance.trainingGameLevel = -1;
            modeId = ModeID.Training;
            serverGame = false;
            ChangeBattleFieldTraining(trainLevel);
        }
        else if (TempCombatData.Instance.challengeGame)
        {
            TempCombatData.Instance.challengeGame = false;
            combatState.CreateChallengeGame();
            modeId = ModeID.Challenge;
            serverGame = false;
            ChangeBattleFieldChallenge();
        }
        else
        {
            combatState.CreateDemoTeam();
            serverGame = false;
        }

        if (!serverGame) ActivateSummonPassive();
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
        yield return new WaitForSeconds(2);
        ActivateAllStartPassive();
        yield return new WaitForSeconds(0.8f);
        combatState.status = CombatStatus.STARTED;

        if (modeId == ModeID.Arena)
        {
            if (defenseAdvantage == HomefieldAdvantage.CANNON)
            {
                GameEffMgr.Instance.CreateCanonShoot();
                yield return new WaitForSeconds(0.5f);
                GameEffMgr.Instance.CreateCanonExplore();
                SoundMgr.PlaySound("Audio/Sailor/bomb");
                yield return new WaitForSeconds(0.3f);
            }
            float delay = ActiveHomeFieldAdvantage();
            yield return new WaitForSeconds(delay);
        }
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
    private void ActivateSummonPassive()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                var sailorA = combatState.GetSailor(Team.A, new CombatPosition(x, y));
                if (sailorA != null) sailorA.ActiveSummonPassive();
                var sailorB = combatState.GetSailor(Team.B, new CombatPosition(x, y));
                if (sailorB != null) sailorB.ActiveSummonPassive();
            }
        }
    }
    private float ActiveHomeFieldAdvantage()
    {
        var config = GlobalConfigs.HomefieldAdvantage.GetAdvantage(defenseAdvantage);
        switch (defenseAdvantage)
        {
            case HomefieldAdvantage.SWEET_HOME:
                combatState.GetAllTeamAliveSailors(Team.B).ForEach(sailor =>
                {
                    sailor.cs.MaxHealth += config._params[0] * sailor.cs.MaxHealth;
                    sailor.cs.CurHealth = sailor.cs.MaxHealth;

                    var img = (new GameObject()).AddComponent<SpriteRenderer>();
                    img.sprite = Resources.Load<Sprite>("UI/Arena/advantage/ad_" + defenseAdvantage.ToString());
                    var pos = sailor.transform.position;
                    pos.y += 2.8f;
                    pos.z -= 0.1f;
                    img.transform.localPosition = pos;

                    Sequence seq = DOTween.Sequence();
                    seq.Append(img.transform.DOScale(2.5f, 3.0f));
                    seq.Join(img.DOFade(0f, 3.0f).SetEase(Ease.InQuint));
                    seq.AppendInterval(2f);
                    seq.AppendCallback(() => Destroy(img));
                });
                return 3;
            case HomefieldAdvantage.ELECTRONIC:
                combatState.GetAllTeamAliveSailors(Team.B).ForEach(sailor =>
                {
                    sailor.AddShield(config._params[0] * sailor.cs.MaxHealth);

                    var img = (new GameObject()).AddComponent<SpriteRenderer>();
                    img.sprite = Resources.Load<Sprite>("UI/Arena/advantage/ad_" + defenseAdvantage.ToString());
                    var pos = sailor.transform.position;
                    pos.y += 2.8f;
                    pos.z -= 0.1f;
                    img.transform.localPosition = pos;

                    Sequence seq = DOTween.Sequence();
                    seq.Append(img.transform.DOScale(2.5f, 3.0f));
                    seq.Join(img.DOFade(0f, 3.0f).SetEase(Ease.InQuint));
                    seq.AppendInterval(2f);
                    seq.AppendCallback(() => Destroy(img));
                });
                return 3;
            case HomefieldAdvantage.ARMOR:
                combatState.GetAllTeamAliveSailors(Team.B).ForEach(sailor =>
                {
                    sailor.cs.BaseArmor += config._params[0];
                    sailor.cs.BaseMagicResist += config._params[1];

                    var img = (new GameObject()).AddComponent<SpriteRenderer>();
                    img.sprite = Resources.Load<Sprite>("UI/Arena/advantage/ad_" + defenseAdvantage.ToString());
                    var pos = sailor.transform.position;
                    pos.y += 2.8f;
                    pos.z -= 0.1f;
                    img.transform.localPosition = pos;

                    Sequence seq = DOTween.Sequence();
                    seq.Append(img.transform.DOScale(2.5f, 3.0f));
                    seq.Join(img.DOFade(0f, 3.0f).SetEase(Ease.InQuint));
                    seq.AppendInterval(2f);
                    seq.AppendCallback(() => Destroy(img));
                });
                return 3;
            case HomefieldAdvantage.CANNON:
                combatState.GetAllTeamAliveSailors(Team.A).ForEach(sailor =>
                {
                float damageTake = sailor.CalcDamageTake(new Damage() { magic = sailor.cs.MaxHealth * config._params[0] }, null);
                    sailor.LoseHealth(new Damage() { magic = damageTake });
                });
                return 2;
            case HomefieldAdvantage.SPEED:
                combatState.GetAllTeamAliveSailors(Team.B).ForEach(sailor =>
                {
                    sailor.cs.Speed += config._params[0];

                    Sequence seq = DOTween.Sequence();
                    var pos = sailor.transform.position;
                    pos.y += 2.8f;
                    pos.z -= 0.1f;
                    var eff = Instantiate(Resources.Load<GameObject>("Effect2D/Duong_FX/VFX_Piratera/fx_speed"), pos, Quaternion.identity);
                    eff.transform.localScale = Vector3.one * 1.8f;
                    var img = (new GameObject()).AddComponent<SpriteRenderer>();
                    img.sprite = Resources.Load<Sprite>("UI/Arena/advantage/ad_" + defenseAdvantage.ToString());
                    img.transform.localPosition = pos;

                    seq.Append(img.transform.DOScale(2.5f, 3.0f));
                    seq.Join(img.DOFade(0f, 3.0f).SetEase(Ease.InQuint));
                    seq.AppendInterval(2f);
                    seq.AppendCallback(() => Destroy(eff));
                    seq.AppendCallback(() => Destroy(img));
                });
                return 3;
        }
        return 1;
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
                    float delayTime = actor.BaseAttack(target, actionProcess.haveCrit, actionProcess.haveDodge, targetHealthLose, healthWildGain) + 0.2f;
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
        GameObject go = GuiManager.Instance.AddGui("Prefap/GuiReward");
        go.GetComponent<GuiReward>().SetReward(d);
    }
    void ShowPvPResult(GameEndPvPData d)
    {
        GameObject go = GuiManager.Instance.AddGui("Prefap/GuiRewardPvP");
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
        } else if (modeId == ModeID.Challenge)
        {
            StartCoroutine(WaitAndDo(0.5f,
                () => SceneManager.LoadScene(TempCombatData.Instance.lastScene)));
        }
        else
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
    public void SkipWatch()
    {
        if (watchReplay)
        {
            CombatAction actionProcess = listActions[listActions.Count - 1];
            actionCount = listActions.Count + 1;
            GameUtils.SetTimeScale(1);
            GameEndData data = actionProcess.gameEndData;
            GameObject go = GuiManager.Instance.AddGui("Prefap/GuiRewardPvP");
            go.GetComponent<GuiRewardPvP>().SetReward((GameEndPvPData)data);
            go.GetComponent<GuiRewardPvP>().isWatchReplay = true;
        }
        else if (modeId == ModeID.Challenge)
        {
            SceneManager.LoadScene(TempCombatData.Instance.lastScene);
        }
        else if (modeId == ModeID.Training)
        {
            combatState.ShowTrainComplete(trainLevel);
            StartCoroutine(WaitAndDo(1.0f, () => {
                LoadServerDataUI.NextScene = "ScenePickTeam";
                SceneManager.LoadScene("SceneLoadServerData");
            }));
        }
    }
    [SerializeField]
    private SpriteRenderer battleField;
    private void ChangeBattleFieldPvP()
    {
        var scale = battleField.transform.localScale;
        battleField.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        battleField.sprite = PvPData.Instance.GetAdvantageBackgroundSprite(defenseAdvantage);
    }
    private void ChangeBattleFieldTraining(int trainLevel)
    {
        battleField.sprite = Resources.Load<Sprite>("Background/train/train_lv_"+trainLevel);
    }private void ChangeBattleFieldChallenge()
    {
        battleField.sprite = Resources.Load<Sprite>("Background/battlefield/island");
    }
    [SerializeField]
    private Transform advantageGO;
    private void ShowAdvantage()
    {
        advantageGO.transform.Find("text").GetComponent<Text>().text = GameUtils.GetHomeAdvantageStr(defenseAdvantage);
        advantageGO.transform.Find("desc").GetComponent<Text>().text = GameUtils.GetHomeAdvantageDesc(defenseAdvantage);
        advantageGO.transform.Find("icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Arena/advantage/ad_" + defenseAdvantage.ToString());
    }
}
