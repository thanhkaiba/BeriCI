
using Piratera.GUI;
using Piratera.Network;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLineUpDefenseUI : MonoBehaviour
{
    [SerializeField]
    private Text TextMaxCapacity;

    void Start()
    {
        if (!PvPData.Instance.HaveJoin)
        {
            GuiManager.Instance.AddGui<PopupDefenseLineUpGuide>("prefap/PopupDefenseLineUpGuide");
        }
     
        DefenseSquadContainer.Draging = false;
        TextMaxCapacity.text = "Max capacity: " + UserData.Instance.NumSlot;
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

    public void OpenSceneCrew()
    {
        CrewData.Instance.OnConfirmSquad();
        SceneManager.LoadScene("SceneCrew");
    }
   
    public void OnConfirm()
    {
        SceneManager.LoadScene("SceneArena");
    }
}
