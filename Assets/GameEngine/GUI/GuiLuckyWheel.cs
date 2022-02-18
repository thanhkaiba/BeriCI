using DG.Tweening;
using Piratera.Network;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class GuiLuckyWheel : BaseGui
    {
        [SerializeField]
        private Transform background;

        [SerializeField]
        private Text textCountDown;

        [SerializeField]
        private Button buttonSpin;


        protected override void Start()
        {
            Appear();
            NetworkController.AddServerActionListener(OnReceiveServerAction);
        }
        public void InitData()
        {



        }


        private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
        {
            if (action == SFSAction.BUY_STAMINA)
            {
                GuiManager.Instance.ShowGuiWaiting(false);

            }
        }



        public void OnClose()
        {
            ClosePopup();
            NetworkController.RemoveServerActionListener(OnReceiveServerAction);
        }
        public void SendSpin()
        {
            NetworkController.Send(SFSAction.PIRATE_WHEEL);
        }
        void Update()
        {
            /* if (StaminaData.Instance.IsRecorveringStamina())
             {
                 TimeSpan remaining = TimeSpan.FromMilliseconds(StaminaData.Instance.TimeToHaveNewStamina());
                 textCountDownStamina.text = string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds);
             }
             else
             {
                 textCountDownStamina.text = "";
             }*/
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
            s.AppendCallback(DestroySelf);

            var fog = GetComponent<HaveFog>();
            if (fog) fog.FadeOut(0.1f);
        }
    }
}
