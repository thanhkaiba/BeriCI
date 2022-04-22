using DG.Tweening;
using Piratera.Network;
using Piratera.Sound;
using Sfs2X.Entities.Data;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class GuiBuyStamina : BaseGui
    {
        [SerializeField]
        private Transform background;
        [SerializeField]
        private Text textBeriCost;
        [SerializeField]
        private Text textStaminaValue;
        [SerializeField]
        private Text textCountDownStamina;
        [SerializeField]
        private Text textCurrentStamina;
        [SerializeField]
        private Button buttonBuy;

        private long cost;

        protected override void Start()
        {
            Appear();
        }
        public void InitPackData(long cost, long quantity)
        {
            this.cost = cost;
            textStaminaValue.text = "+" + quantity;
            textBeriCost.text = "" + cost;
            EnableButtonBuy(cost >= 0);
            textCurrentStamina.text = StaminaData.Instance.GetCurrentStaminaFormat();
            GameEvent.UserStaminaChanged.AddListener(UpdateCurrentStamina);
            NetworkController.AddServerActionListener(OnReceiveServerAction);

        }


        private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
        {
            if (action == SFSAction.BUY_STAMINA)
            {
                SceneTransition.Instance.ShowWaiting(false);
                if (errorCode != SFSErrorCode.SUCCESS)
                {
                    GameUtils.ShowPopupPacketError(errorCode);
                }
                else
                {
                    GameEvent.FlyStamina.Invoke();
                    SoundMgr.PlaySound(PirateraSoundEffect.RECEIVE_GIFT);
                    OnClose();
                }
            }
        }
        private void UpdateCurrentStamina(int arg0, int arg1)
        {
            textCurrentStamina.text = StaminaData.Instance.GetCurrentStaminaFormat();

        }

        private void EnableButtonBuy(bool enabled)
        {
            buttonBuy.interactable = enabled;
            int children = buttonBuy.transform.childCount;
            for (int i = 0; i < children; ++i)
            {
                Transform child = buttonBuy.transform.GetChild(i);
                if (child.gameObject.name == "TextLimited")
                {
                    child.gameObject.SetActive(!enabled);
                }
                else
                {
                    child.gameObject.SetActive(enabled);
                }
            }
        }
        public void OnClose()
        {
            ClosePopup();
            GameEvent.UserStaminaChanged.RemoveListener(UpdateCurrentStamina);
            NetworkController.RemoveServerActionListener(OnReceiveServerAction);
        }
        public void OnBuyStamina()
        {
            if (UserData.Instance.IsEnoughBeri(cost))
            {
                SceneTransition.Instance.ShowWaiting(true);
                NetworkController.Send(SFSAction.BUY_STAMINA);
            }
            else
            {
                OnClose();
                GuiManager.Instance.ShowPopupNotification("Not Enough Beri!");
            }
        }
        void Update()
        {
            if (StaminaData.Instance.IsRecorveringStamina())
            {
                TimeSpan remaining = TimeSpan.FromMilliseconds(StaminaData.Instance.TimeToHaveNewStamina());
                textCountDownStamina.text = string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds);
            }
            else
            {
                textCountDownStamina.text = "";
            }
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
