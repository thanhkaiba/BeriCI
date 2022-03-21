
using Piratera.GUI;
using Piratera.Network;
using Sfs2X.Entities.Data;
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
    void Start()
    {

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
      
      
    }
    
    private void SyncData()
    {
        GuiManager.Instance.ShowGuiWaiting(true);
        NetworkController.Send(SFSAction.PVP_DATA);
        NetworkController.AddServerActionListener(OnReceiveServerAction);
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
}
