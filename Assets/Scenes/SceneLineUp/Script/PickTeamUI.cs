
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
    public RoyalCollectingController royal;
    void Start()
    {
        SquadContainer.Draging = false;
        UpdateSlotMaxCapacity();
        NetworkController.AddServerActionListener(OnReceiveServerAction);
        GameEvent.FlyBeri.AddListener(FlyBeri);
        if (TutorialMgr.Instance.CheckTutStartUp()) ShowTutBuildLineUp();
    }

    void Awake()
    {
        Input.multiTouchEnabled = false;
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
        hand.transform.position = new Vector3(pos.x + 40, pos.y-80, pos.z);
    }
}
