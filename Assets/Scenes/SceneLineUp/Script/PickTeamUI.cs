
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
    [SerializeField]
    private Image BgSlotCapacity;

    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        RunAppearAction();
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
        DoTweenUtils.UpdateNumber(TextMaxCapacity, int.Parse(TextMaxCapacity.text), UserData.Instance.NumSlot);

        if (UserData.Instance.NumSlot < lineUpSlotConfig.max && UserData.Instance.NumSlot > 0)
        {
            ButtonBuySlot.gameObject.SetActive(true);
            TextNewSlotCost.text = lineUpSlotConfig.costs[UserData.Instance.NumSlot - 1].ToString();
        }
        else
        {
            TextMaxCapacity.fontSize += 10;
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

        RectTransform slotCapacityRect = BgSlotCapacity.transform as RectTransform;
        float height = slotCapacityRect.rect.height;
        DOTween.To(x => slotCapacityRect.sizeDelta = new Vector2(slotCapacityRect.sizeDelta.x, x), 0, height, 1f).SetTarget(transform).SetLink(gameObject).SetEase(Ease.OutBack);
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
