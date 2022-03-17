
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
    void Start()
    {
        UpdateTicket();
        if (!PvPData.Instance.HaveJoin)
        {
            GuiManager.Instance.AddGui<PopupDefenseLineUpGuide>("prefap/PopupCongratJoinArena");
            PvPData.Instance.HaveJoin = true;
            NetworkController.Send(SFSAction.PVP_JOIN);
        }
        NetworkController.AddServerActionListener(OnReceiveServerAction);
    }

    private void UpdateTicket()
    {
        LbTicket.text = PvPData.Instance.Ticket + "/" + 5;
    }

    void Awake()
    {
        Input.multiTouchEnabled = false;
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
        }
       
    }

    public void ShowCommingSoon()
    {
        GuiManager.Instance.ShowPopupNotification("Coming Soon!");
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
