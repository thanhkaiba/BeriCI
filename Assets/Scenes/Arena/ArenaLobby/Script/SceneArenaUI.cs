
using DG.Tweening;
using Piratera.GUI;
using Piratera.Network;
using Sfs2X.Entities.Data;
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
    private Text LbRank;
    [SerializeField]
    private Text LbResetTicket;
    [SerializeField]
    private List<Transform> nodeSailors;
    [SerializeField]
    private Transform background;
    [SerializeField]
    private SpriteRenderer battleField;
    void Start()
    {
        NetworkController.AddServerActionListener(OnReceiveServerAction);
        UpdateTicket();
        UpdateRank();
        if (!PvPData.Instance.HaveJoin)
        {
           
            GuiManager.Instance.AddGui<PopupCongratJoinArena>("prefap/PopupCongratJoinArena");
            GuiManager.Instance.ShowGuiWaiting(true);
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
        GuiManager.Instance.ShowGuiWaiting(true);
        NetworkController.Send(SFSAction.PVP_DATA);
    }
    private void UpdateRank()
    {
        LbRank.text = "Rank: " + PvPData.Instance.Rank;
        LbElo.text = "Elo: " + PvPData.Instance.Elo;
    }
    private void UpdateTicket()
    {
        LbTicket.text = PvPData.Instance.Ticket + "/" + 5;
    }
    void Awake()
    {
        Input.multiTouchEnabled = false;
    }
    private void Update()
    {
        System.TimeSpan remaining = System.TimeSpan.FromMilliseconds(GameTimeMgr.GetTimeToNextDayUTC());
        LbResetTicket.text = $"Reset After: {string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds)}";
    }
    private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        switch (action)
        {
            case SFSAction.PVP_PLAY:
                {
                    if (errorCode != SFSErrorCode.SUCCESS)
                    {
                        GuiManager.Instance.ShowGuiWaiting(false);
                    }
                    break;
                }
            case SFSAction.PVP_DATA:
                {
                    GuiManager.Instance.ShowGuiWaiting(false);
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
                    GuiManager.Instance.ShowGuiWaiting(false);
                    if (errorCode == SFSErrorCode.SUCCESS)
                    {
                        PvPData.Instance.HaveJoin = true;
                        SyncData();
                    }
                    break;
                }
            case SFSAction.PVP_RANKING:
                {
                    GuiManager.Instance.ShowGuiWaiting(false);
                    if (errorCode == SFSErrorCode.SUCCESS)
                    {
                        GameObject GO = GuiManager.Instance.AddGui<PopupTopArena>("Prefap/PopupTopArena");
                        PopupTopArena popup = GO.GetComponent<PopupTopArena>();
                        popup.NewFromSFSObject(packet.GetSFSArray("list"));
                    }
                    break;
                }
            case SFSAction.PVP_HISTORY:
                {
                    GuiManager.Instance.ShowGuiWaiting(false);
                    if (errorCode == SFSErrorCode.SUCCESS)
                    {
                        GameObject GO = GuiManager.Instance.AddGui<PopupArenaHistory>("Prefap/PopupArenaHistory");
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
        GuiManager.Instance.AddGui<PopupNotification>("Prefap/PopupArenaGuide");
    }
    public void ShowTop()
    {
        GuiManager.Instance.ShowGuiWaiting(true);
        SFSObject s = new SFSObject();
        s.PutInt("from", 0);
        s.PutInt("to", 10);
        NetworkController.Send(SFSAction.PVP_RANKING, s);
    }
    public void ShowHistory()
    {
        GuiManager.Instance.ShowGuiWaiting(true);
        NetworkController.Send(SFSAction.PVP_HISTORY);
    }
    private void OnDestroy()
    {
        NetworkController.RemoveServerActionListener(OnReceiveServerAction);

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
        GuiManager.Instance.ShowGuiWaiting(true);
        NetworkController.Send(SFSAction.PVP_PLAY);
    }
    private void ShowListSailors()
    {
        for (int i = 0; i < nodeSailors.Count; i++)
        {
            var model = PvPData.Instance.DefenseCrew.SailorAt(new CombatPosition(i % 3, i / 3));
            if (model == null) continue;
            Transform GO = Instantiate(GameUtils.GetSailorModelPrefab(model.config_stats.root_name), nodeSailors[i]).transform.FindDeepChild("model");
            GO.GetComponent<Renderer>().sortingOrder = 3;
            GO.Find("shadow").GetComponent<Renderer>().sortingOrder = 3;
        }
    }
    private void ShowAppearEffect() {
        var scale = new Vector3(0.6f, 0.6f, 0.6f);
        background.localScale += scale;
        background.DOScale(-scale, 0.8f).SetRelative().SetEase(Ease.OutCirc).SetTarget(background).SetLink(background.gameObject);
    }
    private void UpdateBattleFieldImage()
    {
        battleField.sprite = PvPData.Instance.GetAdvantageBackgroundSprite();
    }
}
