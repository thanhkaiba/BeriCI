
using Piratera.Config;
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
        GuiManager.Instance.AddGui<PopupDefenseLineUpGuide>("prefap/PopupCongratJoinArena");
        NetworkController.AddServerActionListener(OnReceiveServerAction);
    }

    void Awake()
    {
        Input.multiTouchEnabled = false;
    }

    private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {

        if (action == SFSAction.BUY_SLOT)
        {
         
        }
    }

    private void OnDestroy()
    {
        NetworkController.RemoveServerActionListener(OnReceiveServerAction);

    }



    public void OnBackToLobby()
    {
        CrewData.Instance.OnConfirmSquad();
        SceneManager.LoadScene("SceneLobby");
    }

    public void OpenSceneLineUp()
    {
        CrewData.Instance.OnConfirmSquad();
        SceneManager.LoadScene("SceneDefenseLineUP");
    }
 
}
