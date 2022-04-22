
using DG.Tweening;
using Piratera.GUI;
using Piratera.Network;
using Sfs2X.Entities.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLineUpDefenseUI : MonoBehaviour
{
    [SerializeField]
    private Text TextMaxCapacity, textCurrentAdvantage, textDescAdvantage;
    [SerializeField]
    private Image iconCurrentAdvantage;
    [SerializeField]
    private SpriteRenderer battlefield;
    void Start()
    {
        if (PvPData.Instance.ShowedTutorial < PvPData.PVP_TURORIAL_STEP.POPUP_DEFENSE_LINEUP)
        {
            GuiManager.Instance.AddGui("prefap/PopupDefenseLineUpGuide");
        }
     
        DefenseSquadContainer.Draging = false;
        TextMaxCapacity.text = "Max capacity: " + UserData.Instance.NumSlot;
        NetworkController.Listen(OnReceiveServerAction);
        UpdateBattleFieldImage();
    }
    private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        if (action == SFSAction.PVP_OPEN_HOME_ADVANTAGE)
        {
            PresentListAdvantage();
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
        var curAdvantage = PvPData.Instance.SelectingAdvantage;
        textCurrentAdvantage.text = GameUtils.GetHomeAdvantageStr(curAdvantage);
        textDescAdvantage.text = GameUtils.GetHomeAdvantageDesc(curAdvantage);
        iconCurrentAdvantage.sprite = Resources.Load<Sprite>("UI/Arena/advantage/ad_" + curAdvantage.ToString());
    }
    [SerializeField]
    private GameObject popupSelectAdvantage;
    [SerializeField]
    private GameObject CellAdvantage;
    [SerializeField]
    private Transform ContentAdvantageView;
    [SerializeField]
    private GameObject popupConfirm;

    private List<CellAdvantage> listCell = new List<CellAdvantage>();
    private void Awake()
    {
        Input.multiTouchEnabled = false;
        popupSelectAdvantage.SetActive(false);
        CreateListAdvantage();
        PresentListAdvantage();
    }
    public void OpenPopupAdvantage()
    {
        if (!popupSelectAdvantage.activeSelf)
        {
            popupSelectAdvantage.SetActive(true);
            PresentListAdvantage();
        }
    }
    public void ClosePopupAdvantage()
    {
        if (popupSelectAdvantage.activeSelf)
            popupSelectAdvantage.SetActive(false);
    }
    private void CreateListAdvantage()
    {
        foreach (HomefieldAdvantage ele in (HomefieldAdvantage[]) System.Enum.GetValues(typeof(HomefieldAdvantage)))
        {
            var cell = Instantiate(CellAdvantage, ContentAdvantageView);
            listCell.Add(cell.GetComponent<CellAdvantage>());
            var btn = cell.AddComponent<Button>();
            btn.onClick.AddListener(() => SelectAdvantage(ele));
        }
    }
    private void PresentListAdvantage()
    {
        for (int i = 0; i < listCell.Count; i++)
        {
            var cell = listCell[i];
            cell.SetType((HomefieldAdvantage) i);
        }
    }
    private void SelectAdvantage(HomefieldAdvantage type)
    {
        Debug.Log("Select Advantage: " + type);
        // neu da mua thi chon, chua mua thi hien bo tien mua

        var data = PvPData.Instance.OpenedAdvantage;
        if (data.Contains(type))
        {
            PvPData.Instance.SelectingAdvantage = type;
            SFSObject sfsObj = new SFSObject();
            sfsObj.PutInt("advantage_idx", (int)type);
            NetworkController.Send(SFSAction.PVP_SELECT_HOME_ADVANTAGE, sfsObj);
            UpdateBattleFieldImage();
            PresentListAdvantage();
        }
        else
        {
            var gui = GuiManager.Instance.AddGui(popupConfirm);
            gui.GetComponent<PopupConfirmUnlockAdvantage>().SetType(type);
        }
    }
}
