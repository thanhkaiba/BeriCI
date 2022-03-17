
using Piratera.GUI;
using Piratera.Network;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneArenaUI : MonoBehaviour
{
   
    void Start()
    {
        if (!PvPData.Instance.HaveJoin)
        {
            GuiManager.Instance.AddGui<PopupDefenseLineUpGuide>("prefap/PopupCongratJoinArena");
            PvPData.Instance.HaveJoin = true;
            NetworkController.Send(SFSAction.PVP_JOIN);
        }
        NetworkController.AddServerActionListener(OnReceiveServerAction);
    }

    void Awake()
    {
        Input.multiTouchEnabled = false;
    }

    private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {

       
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
 
}
