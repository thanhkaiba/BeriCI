using DG.Tweening;
using Piratera.Utils;
using Sfs2X.Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class GuiBuySlot : BaseGui
    {
        [SerializeField]
        private Transform background;
        [SerializeField]
        private Text textBeriCost;
        [SerializeField]
        private Text textCurrentBeri;
        [SerializeField]
        private Button buttonBuy;

        private long cost;
        protected override void Start()
        {
            Appear();
        }

        public void InitPackData(long cost)
        {
            this.cost = cost;
            textBeriCost.text = "" + cost;
            buttonBuy.interactable = cost >= 0;
            textCurrentBeri.text = StringUtils.ShortNumber(UserData.Instance.Beri);
            GameEvent.UserBeriChanged.AddListener(UpdateCurrentBeri);
            NetworkController.AddServerActionListener(OnReceiveServerAction);

        }

        private void UpdateCurrentBeri(long arg0, long arg1)
        {
            textCurrentBeri.text = StringUtils.ShortNumber(UserData.Instance.Beri);
        }

        public void OnClose()
        {
            ClosePopup();
            GameEvent.UserBeriChanged.RemoveListener(UpdateCurrentBeri);
            NetworkController.RemoveServerActionListener(OnReceiveServerAction);
        }
        public void OnBuySlot()
        {
            if (UserData.Instance.IsEnoughBeri(cost))
            {
                GuiManager.Instance.ShowGuiWaiting(true);
                NetworkController.Send(SFSAction.BUY_SLOT);
            }
            else
            {
                OnClose();
                GuiManager.Instance.ShowPopupNotification("Not Enough Beri!");
            }
        }

        private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
        {
            GuiManager.Instance.ShowGuiWaiting(false);
            if (action == SFSAction.BUY_SLOT)
            {
                GuiManager.Instance.ShowGuiWaiting(false);
                if (errorCode != SFSErrorCode.SUCCESS)
                {
                    GameUtils.ShowPopupPacketError(errorCode);
                }
                else
                {
                    OnClose();
                }
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

            background.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            background.DOScale(new Vector3(1f, 1f, 1f), 0.4f).SetEase(Ease.OutBack);

            var fog = GetComponent<HaveFog>();
            if (fog) fog.FadeIn(0.4f);
        }
        private void ClosePopup()
        {
            background.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f).SetEase(Ease.OutSine);
            var canvasGroup = background.GetComponent<CanvasGroup>();
            Sequence s = DOTween.Sequence();
            s.Append(canvasGroup.DOFade(0, 0.2f));
            s.AppendCallback(DestroySelf);

            var fog = GetComponent<HaveFog>();
            if (fog) fog.FadeOut(0.2f);
        }
    }
}


