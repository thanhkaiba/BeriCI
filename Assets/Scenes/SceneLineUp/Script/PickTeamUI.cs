
using UnityEngine;
using UnityEngine.UI;
using Piratera.Utils;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Piratera.GUI;
using Sfs2X.Entities.Data;
using Piratera.Network;

public class PickTeamUI : MonoBehaviour
{
    [Header("Slot Capacity")]
    [SerializeField]
    private Text TextMaxCapacity;
    [SerializeField]
    private Button ButtonBuySlot;
    [SerializeField]
    private LineUpSlot lineUpSlotConfig;

    void Start()
    {
        SquadContainer.Draging = false;
        SquadAContainer.Draging = false;
        UpdateSlotMaxCapacity();
        NetworkController.AddServerActionListener(OnReceiveServerAction);

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
    }



    void UpdateSlotMaxCapacity()
    {
        TextMaxCapacity.text = "" + UserData.Instance.NumSlot;

        if (UserData.Instance.NumSlot < lineUpSlotConfig.max && UserData.Instance.NumSlot > 0)
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
        SceneManager.LoadScene("SceneLobby");
    }

    public void OpenSceneCrew()
    {
        SceneManager.LoadScene("SceneCrew");
    }

    public void OnBuySlot()
    {
        if (UserData.Instance.NumSlot >= lineUpSlotConfig.max)
        {
            GuiManager.Instance.ShowPopupNotification("Can't buy more slot");
            return;
        }
        GuiManager.Instance.ShowGuiWaiting(true);
        NetworkController.Send(SFSAction.GET_LINEUP_SLOT_PACK);

    }
}
