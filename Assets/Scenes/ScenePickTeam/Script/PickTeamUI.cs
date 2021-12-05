
using UnityEngine;
using UnityEngine.UI;
using Piratera.Utils;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Piratera.GUI;
using Sfs2X.Entities.Data;

public class PickTeamUI : MonoBehaviour
{
    [SerializeField]
    private Image title;
    [SerializeField]
    private Image backgroundSailorList;
    private Canvas canvas;

    [Header("Slot Capacity")]
    [SerializeField]
    private Text TextMaxCapacity;
    [SerializeField]
    private Text TextNewSlotCost;
    [SerializeField]
    private Button ButtonBuySlot;
    [SerializeField]
    private LineUpSlot lineUpSlotConfig;

    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        RunAppearAction();
        SquadContainer.Draging = false;
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
        TextMaxCapacity.text = UserData.Instance.NumSlot.ToString();
        if (UserData.Instance.NumSlot < lineUpSlotConfig.max)
        {
            ButtonBuySlot.gameObject.SetActive(true);
            TextNewSlotCost.text = lineUpSlotConfig.costs[UserData.Instance.NumSlot - 1].ToString();
        }
        else
        {
            ButtonBuySlot.gameObject.SetActive(false);
        }

    }

    private void RunAppearAction()
    {
        float scale = canvas.transform.localScale.x;
        float titleHeight = ((RectTransform)title.transform).sizeDelta.y * scale;
        DoTweenUtils.FadeAppearY(title, titleHeight + 20, 0.5f, Ease.OutFlash);

        float slotHeight = ((RectTransform)backgroundSailorList.transform).sizeDelta.y * 2 * scale;
        DoTweenUtils.FadeAppearY(backgroundSailorList, -slotHeight * 1.5f, 0.5f, Ease.OutFlash);
    }

    public void OnBackToLobby()
    {
        SceneManager.LoadScene("SceneLobby");
    }

    public void OnBuySlot()
    {
        if (UserData.Instance.NumSlot >= lineUpSlotConfig.max)
        {
            GuiManager.Instance.ShowPopupNotification("Can't buy more slot");
            return;
        }
        int cost = lineUpSlotConfig.costs[UserData.Instance.NumSlot - 1];
        if (UserData.Instance.IsEnoughBeri(cost))
        {
            GuiManager.Instance.ShowGuiWaiting(true);
            NetworkController.Send(SFSAction.BUY_SLOT);
        }
        else
        {
            GuiManager.Instance.ShowPopupNotification("Not enough Beri");
        }

    }
}
