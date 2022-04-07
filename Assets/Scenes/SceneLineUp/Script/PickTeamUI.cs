
using DG.Tweening;
using Piratera.Config;
using Piratera.GUI;
using Piratera.Network;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PickTeamUI : MonoBehaviour
{
    [Header("Slot Capacity")]
    [SerializeField]
    private Text TextMaxCapacity;
    [SerializeField]
    private Button ButtonBuySlot;
    [SerializeField]
    private Button[] TrainsButton;
    public RoyalCollectingController royal;
    void Start()
    {
        SquadContainer.Draging = false;
        UpdateSlotMaxCapacity();
        NetworkController.AddServerActionListener(OnReceiveServerAction);
        GameEvent.FlyBeri.AddListener(FlyBeri);
        if (TutorialMgr.Instance.CheckTutStartUp()) ShowTutBuildLineUp();
        if (TutorialMgr.Instance.CheckTutOpenSlot())
        {
            ShowNPCTutOpenSlot();
        }
    }
    void Awake()
    {
        Input.multiTouchEnabled = false;
        var config = GlobalConfigs.Training;
        var listTimes = UserData.Instance.TrainedToday;
        Debug.Log("UserData.Instance.PVECount: " + UserData.Instance.PVECount);
        for (int i = 0; i < TrainsButton.Length; i++)
        {
            Debug.Log("config.game_require[i]: " + config.game_require[i]);
            var btn = TrainsButton[i];
            btn.gameObject.SetActive(UserData.Instance.PVECount >= config.game_require[i]);
            btn.transform.Find("textPrice").GetComponent<Text>().text = "" + config.cost[i];
            btn.transform.Find("textEXP").GetComponent<Text>().text = "+" + config.exp_receive[i].ToString("N0") + "\nEXP";

            int times = 0;
            if (listTimes.Length > i) times = listTimes[i];
            btn.transform.Find("textTimes").GetComponent<Text>().text = "today: " + times + "/" + config.limit_day;
            btn.interactable = times < config.limit_day;
        }
    }
    private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        if (action == SFSAction.BUY_SLOT)
        {
            GuiManager.Instance.ShowGuiWaiting(false);
            if (errorCode != SFSErrorCode.SUCCESS)
            {
                GameUtils.ShowPopupPacketError(errorCode);
            }
            else
            {
                UpdateSlotMaxCapacity();
            }
        }
        if (action == SFSAction.TRAIN_SAILORS)
        {
            GuiManager.Instance.ShowGuiWaiting(false);
            if (errorCode != SFSErrorCode.SUCCESS)
            {
                GameUtils.ShowPopupPacketError(errorCode);
            }
            else
            {
                GuiManager.Instance.ShowGuiWaiting(false);
                SceneManager.LoadScene("SceneCombat2D");
            }
        }
    }
    private void OnDestroy()
    {
        NetworkController.RemoveServerActionListener(OnReceiveServerAction);
        GameEvent.FlyBeri.RemoveListener(FlyBeri);

    }
    void UpdateSlotMaxCapacity()
    {
        TextMaxCapacity.text = "" + UserData.Instance.NumSlot;

        if (UserData.Instance.NumSlot < GlobalConfigs.LineUp.max && UserData.Instance.NumSlot > 0)
        {
            ButtonBuySlot.gameObject.SetActive(true);
        }
        else
        {
            TextMaxCapacity.fontSize += 10;
            ButtonBuySlot.gameObject.SetActive(false);
        }

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
    public void FlyBeri()
    {
        royal.CollectItem(5, 1, () => { });
    }
    public void OnBuySlot()
    {
        if (UserData.Instance.NumSlot >= GlobalConfigs.LineUp.max)
        {
            GuiManager.Instance.ShowPopupNotification("Can't buy more slot");
            return;
        }
        GuiManager.Instance.ShowGuiWaiting(true);
        NetworkController.Send(SFSAction.GET_LINEUP_SLOT_PACK);
    }
    private void ShowTutBuildLineUp()
    {
        var go = Resources.Load<GameObject>("Prefap/Tuts/LineUpTut1");
        GameObject tut = Instantiate(go, GuiManager.Instance.GetLayer(LayerId.LOADING).transform);
        GameObject.Find("BgBuySlot").SetActive(false);
        GameObject.Find("ButtonCrew").SetActive(false);
    }
    public void ShowTutBackToLobby()
    {
        var go = Resources.Load<GameObject>("Prefap/Tuts/hand");
        GameObject hand = Instantiate(go, GuiManager.Instance.GetLayer(LayerId.LOADING).transform);
        var pos = GameObject.Find("ButtonBack").transform.position;
        hand.transform.position = new Vector3(pos.x, pos.y, pos.z);
    }
    public void ShowFocusOpenSlot()
    {
        TutorialMgr.Instance.CompleteOpenSlot();
        var go = Resources.Load<GameObject>("Prefap/Tuts/hand");
        GameObject hand = Instantiate(go, GuiManager.Instance.GetLayer(LayerId.LOADING).transform);
        var pos = GameObject.Find("ButtonOpenSlot").transform.position;
        hand.transform.position = new Vector3(pos.x, pos.y, pos.z);
        hand.name = "hand_open_slot";
    }
    private void ShowNPCTutOpenSlot()
    {
        var go = Resources.Load<GameObject>("Prefap/Tuts/NPCTutOpenSlot");
        GameObject hand = Instantiate(go, GuiManager.Instance.GetLayer(LayerId.LOADING).transform);
    }
    public void ClickOpenSlot()
    {
        var hand = GameObject.Find("hand_open_slot");
        if (hand) Destroy(hand);
    }
    public void Train(int level)
    {
        CrewData.Instance.OnConfirmSquad();
        int cost = GlobalConfigs.Training.cost[level];
        if (CrewData.Instance.FightingTeam.IsEmpty())
        {
            GuiManager.Instance.ShowPopupNotification("No sailor for trainning");
        }
        else if (UserData.Instance.Beri < cost)
        {
            GuiManager.Instance.ShowPopupNotification("You do not have enough BERI");
        }
        else
        {
            UserData.Instance.Beri -= cost;
            GuiManager.Instance.ShowGuiWaiting(true);
            TempCombatData.Instance.trainingGameLevel = level;
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("level", level);
            NetworkController.Send(SFSAction.TRAIN_SAILORS, sfsObject);
        }
    }
}
