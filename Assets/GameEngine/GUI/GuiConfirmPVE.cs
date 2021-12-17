using DG.Tweening;
using Piratera.Utils;
using Sfs2X.Entities.Data;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class GuiConfirmPVE : BaseGui
    {

        [SerializeField]
        private Text textCurrentStamina;
        [SerializeField]
        private Transform background;
        private int priceStamina = 5;
        [SerializeField]
        private GameObject fight, find, finding;
        public LobbyUI lobby;

        protected override void Start()
        {
            base.Start();
            UpdateCurrentStamina();
            GameEvent.UserStaminaChanged.AddListener(UpdateCurrentStamina);
            Appear();
        }

        private void UpdateCurrentStamina(int arg0, int arg1)
        {
            UpdateCurrentStamina();
        }


        private void UpdateCurrentStamina()
        {
            textCurrentStamina.text = UserData.Instance.GetStamina().ToString();
        }

        public void OnStartFindGame()
        {
            if (UserData.Instance.GetStamina() < priceStamina)
                GuiManager.Instance.AddGui<GuiBuyStamina>("Prefap/GuiBuyStamina", LayerId.GUI);
            else
            {

                Sequence s = DOTween.Sequence();
                DoTweenUtils.UpdateNumber(textCurrentStamina, UserData.Instance.GetStamina(), (UserData.Instance.GetStamina() - priceStamina), x => UserData.Instance._GetStaminaFormat((int)x));
                // UserData.Instance.MinusStamina(priceStamina);
                lobby.OnStaminaChanged(UserData.Instance.GetStamina(), (UserData.Instance.GetStamina() - priceStamina));
                s.AppendInterval(1.5f);
                s.AppendCallback(() => fight.SetActive(false));
                s.AppendCallback(() => find.SetActive(true));
                s.AppendInterval(1.5f);
                s.AppendCallback(() => NetworkController.Send(SFSAction.PVE_PLAY));

            }

        }
        public void OnClose()
        {
            ClosePopup();
            GameEvent.UserStaminaChanged.RemoveListener(UpdateCurrentStamina);
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
