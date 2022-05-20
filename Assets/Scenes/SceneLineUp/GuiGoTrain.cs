using DG.Tweening;
using Piratera.Config;
using Piratera.GUI;
using Piratera.Network;
using Piratera.Sound;
using Piratera.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GuiGoTrain : MonoBehaviour
{
    private int train_level;
    [SerializeField]
    private Text textYourBeri;
    [SerializeField]
    private Text textDescription;
    [SerializeField]
    private Transform background;
    [SerializeField]
    private Text beriCostText;
    [SerializeField]
    private GameObject beriMinus;

    protected void Start()
    {
        NetworkController.Listen(onReceiveServerAction);
        Appear();
    }
    public void SetLevel(int level)
    {
        train_level = level;
        var config = GlobalConfigs.Training;
        beriCostText.text = "" + config.cost[train_level];
        textDescription.text = "All sailors in your line-up will receive <color=#695f01>" + config.exp_receive[level].ToString("N0") + "</color> EXP";
        textYourBeri.text = UserData.Instance.Beri.ToString("N0");
    }
    private void onReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        if (action == SFSAction.TRAIN_SAILORS)
        {
            SceneTransition.Instance.ShowWaiting(false);
            if (errorCode != SFSErrorCode.SUCCESS)
            {
                GameUtils.ShowPopupPacketError(errorCode);
            }
            else
            {
                SceneTransition.Instance.ShowWaiting(false);
                SceneTransition.Instance.LoadScene("SceneCombat2D", TransitionType.FADE);
            }
        }
    }
    public void OnStartTrain()
    {
        int cost = GlobalConfigs.Training.cost[train_level];
        UserData.Instance.Beri -= cost;
        SceneTransition.Instance.ShowWaiting(true);
        TempCombatData.Instance.trainingGameLevel = train_level;
        SFSObject sfsObject = new SFSObject();
        sfsObject.PutInt("level", train_level);
        NetworkController.Send(SFSAction.TRAIN_SAILORS, sfsObject);
    }
    public void OnClose()
    {
        ClosePopup();
    }
    private void OnDestroy()
    {
        NetworkController.RemoveListener(onReceiveServerAction);
    }
    private void Appear()
    {
        Sequence s = DOTween.Sequence();
        var canvasGroup = background.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.DOFade(1, 0.2f);
        s.AppendCallback(() => canvasGroup.interactable = true);

        background.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        background.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.OutBack).SetLink(background.gameObject).SetTarget(background.transform);

        var fog = GetComponent<HaveFog>();
        if (fog) fog.FadeIn(0.3f);
    }
    private void ClosePopup()
    {
        background.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.1f).SetEase(Ease.OutSine);
        var canvasGroup = background.GetComponent<CanvasGroup>();
        Sequence s = DOTween.Sequence();
        s.Append(canvasGroup.DOFade(0, 0.1f));
        s.AppendCallback(() => Destroy(gameObject));

        var fog = GetComponent<HaveFog>();
        if (fog) fog.FadeOut(0.1f);
    }
}
