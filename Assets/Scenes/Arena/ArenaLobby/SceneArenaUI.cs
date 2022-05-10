
using DG.Tweening;
using Piratera.Config;
using Piratera.GUI;
using Piratera.Network;
using Piratera.Sound;
using Sfs2X.Entities.Data;
using Spine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneArenaUI : MonoBehaviour
{
    [SerializeField]
    private Text LbTicket;
    [SerializeField]
    private Text LbElo;
    [SerializeField]
    private Text LbRank, LBSeason;
    [SerializeField]
    private Text LbResetTicket;
    [SerializeField]
    private List<Transform> nodeSailors;
    [SerializeField]
    private Transform background;
    [SerializeField]
    private SpriteRenderer battleField;
    [SerializeField]
    private GameObject popupRanking, popupInfo, popupHistory;
    [SerializeField]
    private ArenaTalk talks;
    void Start()
    {
        SoundMgr.PlayBGMusic(PirateraMusic.LOBBY);
        NetworkController.Listen(OnReceiveServerAction);
        UpdateTicket();
        UpdateRank();
        if (!PvPData.Instance.HaveJoin)
        {
            GuiManager.Instance.AddGui("prefap/PopupCongratJoinArena");
            SceneTransition.Instance.ShowWaiting(true, false);
            NetworkController.Send(SFSAction.PVP_JOIN);
        } else
        {
            SyncData();
        }
        ShowListSailors();
        ShowAppearEffect();
        UpdateBattleFieldImage();
    }
    private void SyncData()
    {
        SceneTransition.Instance.ShowWaiting(true, false);
        NetworkController.Send(SFSAction.PVP_DATA);
    }
    private void UpdateRank()
    {
        var data = PvPData.Instance;
        var startTime = GameUtils.FromUnixTime(data.StartSeason);
        var endTime = GameUtils.FromUnixTime(data.EndSeason);
        LBSeason.text = "<color=#b8daff>Season "
            + data.SeasonId + "</color>\n<color=#dbdbdb>"
            + startTime.ToString("dd/MM/yyyy")
            + " - " + endTime.ToString("dd/MM/yyyy")
            + "</color>";
        LbRank.text = "Rank: " + data.Rank;
        LbElo.text = "Elo: " + data.Elo;
    }
    private void UpdateTicket()
    {
        LbTicket.text = PvPData.Instance.Ticket + "/" + GlobalConfigs.PvP.max_ticket;
    }
    void Awake()
    {
        Input.multiTouchEnabled = false;
    }

    private float timeSeqChat = 4.0f;
    private void Update()
    {
        System.TimeSpan remaining = System.TimeSpan.FromMilliseconds(GameTimeMgr.GetTimeToNextDayUTC());
        LbResetTicket.text = $"Reset After: {string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds)}";
        timeSeqChat -= Time.deltaTime;
        if (timeSeqChat < 0)
        {
            ShowRandomChat();
            timeSeqChat = MathUtils.RandomInt(6, 10);
        }
    }

    private void ShowRandomChat()
    {
        if (sailors.Count == 0) return;
        int idx = MathUtils.RandomInt(0, talks.talks.Count - 1);
        string text = talks.talks[idx];
        var sailor = sailors[MathUtils.RandomInt(0, sailors.Count - 1)];
        var chat = GameUtils.ShowChat(sailor, text, 6);
        chat.GetComponent<Canvas>().sortingOrder = 5;
        chat.transform.localScale = chat.transform.localScale * 0.64f;
    }
    private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        switch (action)
        {
            case SFSAction.PVP_PLAY:
                {
                    if (errorCode != SFSErrorCode.SUCCESS)
                    {
                        SceneTransition.Instance.ShowWaiting(false);
                    }
                    break;
                }
            case SFSAction.PVP_DATA:
                {
                    SceneTransition.Instance.ShowWaiting(false);
                    if (errorCode == SFSErrorCode.SUCCESS)
                    {
                        UpdateTicket();
                        UpdateRank();
                        UpdateBattleFieldImage();
                    }
                    break;
                }
            case SFSAction.PVP_JOIN:
                {
                    SceneTransition.Instance.ShowWaiting(false);
                    if (errorCode == SFSErrorCode.SUCCESS)
                    {
                        PvPData.Instance.HaveJoin = true;
                        SyncData();
                    }
                    break;
                }
            case SFSAction.PVP_RANKING:
                {
                    SceneTransition.Instance.ShowWaiting(false);
                    if (errorCode == SFSErrorCode.SUCCESS)
                    {
                        GameObject GO = GuiManager.Instance.AddGui(popupRanking);
                        PopupTopArena popup = GO.GetComponent<PopupTopArena>();
                        popup.NewFromSFSObject(packet.GetSFSArray("list"));
                    }
                    break;
                }
            case SFSAction.PVP_HISTORY:
                {
                    SceneTransition.Instance.ShowWaiting(false);
                    if (errorCode == SFSErrorCode.SUCCESS)
                    {
                        GameObject GO = GuiManager.Instance.AddGui(popupHistory);
                        PopupArenaHistory popup = GO.GetComponent<PopupArenaHistory>();
                        popup.NewFromSFSObject(packet.GetSFSArray("history"));
                    }
                    break;
                }
        }
    }
    public void ShowCommingSoon()
    {
        GuiManager.Instance.ShowPopupNotification("Coming Soon!");
    }
    public void ShowGuide()
    {
        GuiManager.Instance.AddGui(popupInfo);
    }
    public void ShowTop()
    {
        SceneTransition.Instance.ShowWaiting(true, false);
        SFSObject s = new SFSObject();
        s.PutInt("from", 0);
        s.PutInt("to", 50);
        NetworkController.Send(SFSAction.PVP_RANKING, s);
    }
    public void ShowHistory()
    {
        SceneTransition.Instance.ShowWaiting(true, false);
        NetworkController.Send(SFSAction.PVP_HISTORY);
    }
    private void OnDestroy()
    {
        NetworkController.RemoveListener(OnReceiveServerAction);

    }
    public void OnBackToLobby()
    {
        SceneManager.LoadScene("SceneLobby");
    }
    public void OpenSceneLineUp()
    {
        CrewData.Instance.OnConfirmSquad();
        SceneManager.LoadScene("SceneLineUpDefense");
    }
    public void OnFight()
    {
        if (PvPData.Instance.Ticket <= 0)
        {
            GuiManager.Instance.ShowPopupNotification("Not Enough Ticket!");
            return;
        }
        SceneTransition.Instance.ShowWaiting(true, false);
        NetworkController.Send(SFSAction.PVP_PLAY);
    }
    private List<Transform> sailors = new List<Transform>();
    private void ShowListSailors()
    {
        for (int i = 0; i < nodeSailors.Count; i++)
        {
            var model = PvPData.Instance.DefenseCrew.SailorAt(new CombatPosition(i % 3, i / 3));
            if (model == null) continue;
            GameObject GO = Instantiate(GameUtils.GetSailorModelPrefab(model.config_stats.root_name), nodeSailors[i]);
            Transform modelTransform = GO.transform.FindDeepChild("model");
            modelTransform.GetComponent<Renderer>().sortingOrder = 3;
            modelTransform.Find("shadow").GetComponent<Renderer>().sortingOrder = 3;
            sailors.Add(GO.transform);
        }
    }
    private void ShowAppearEffect() {
        var pos = background.position;
        var oriPos = Camera.main.transform.position;
        pos.z = oriPos.z;
        //Camera.main.transform.position = pos;
        Camera.main.orthographicSize = 10.5f;
        Camera.main.DOOrthoSize(8, 0.8f).SetEase(Ease.OutCirc);
        //Camera.main.transform.DOMove(oriPos, 0.8f).SetEase(Ease.OutCirc);
        //var scale = new Vector3(0.6f, 0.6f, 0.6f);
        //background.localScale += scale;
        //background.DOScale(-scale, 0.8f).SetRelative().SetEase(Ease.OutCirc).SetTarget(background).SetLink(background.gameObject);
    }
    private void UpdateBattleFieldImage()
    {
        battleField.sprite = PvPData.Instance.GetAdvantageBackgroundSprite();
    }
}
