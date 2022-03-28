
using DG.Tweening;
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
    [SerializeField]
    private SpriteRenderer battlefield;
    void Start()
    {
        if (PvPData.Instance.ShowedTutorial < PvPData.PVP_TURORIAL_STEP.POPUP_DEFENSE_LINEUP)
        {
            GuiManager.Instance.AddGui<PopupDefenseLineUpGuide>("prefap/PopupDefenseLineUpGuide");
        }
     
        DefenseSquadContainer.Draging = false;
        TextMaxCapacity.text = "Max capacity: " + UserData.Instance.NumSlot;
        NetworkController.AddServerActionListener(OnReceiveServerAction);
        UpdateBattleFieldImage();
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
    public void OnConfirm()
    {
        if (PvPData.Instance.DefenseCrew.IsFightingLineEmpty())
        {
            GuiManager.Instance.ShowPopupNotification("You must select at least one fighter");
            return;
        }
        PvPData.Instance.DefenseCrew.OnConfirmSquad();
        SceneManager.LoadScene("SceneArena");
    }
    private void UpdateBattleFieldImage()
    {
        battlefield.sprite = PvPData.Instance.GetAdvantageBackgroundSprite();
    }
    [SerializeField]
    private GameObject popupSelectAdvantage;
    public void OpenPopupAdvantage()
    {
        if (!popupSelectAdvantage.activeSelf)
            popupSelectAdvantage.SetActive(true);
    }
    private void Awake()
    {
        Input.multiTouchEnabled = false;
        popupSelectAdvantage.SetActive(false);
    }
    public void ClosePopupAdvantage()
    {
        if (popupSelectAdvantage.activeSelf)
            popupSelectAdvantage.SetActive(false);
    }
}
