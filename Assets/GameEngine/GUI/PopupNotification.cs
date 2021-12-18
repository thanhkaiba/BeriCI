using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class PopupNotification : BaseGui
    {
        [SerializeField]
        private Text textNotification;
        [SerializeField]
        private Transform background;
        private Action OKFunc;
        protected override void Start()
        {
            Appear();
        }
        public void OnOK()
        {
            if (OKFunc != null)
            {
                OKFunc();
            }
            ClosePopup();
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
            background.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.OutBack);

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
        public void SetData(string text)
        {
            textNotification.text = text;
        }
        public void SetData(string text, Action okFunc)
        {
            SetData(text);
            OKFunc = okFunc;
        }
    }
}


