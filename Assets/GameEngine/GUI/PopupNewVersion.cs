using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;

namespace Piratera.GUI
{
    public class PopupNewVersion : MonoBehaviour
    {
        [SerializeField]
        private Transform background;
        private Action OKFunc;
        protected void Start()
        {
            Appear();
        }
        public void OnOK()
        {
            if (OKFunc != null) OKFunc();
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

        public void SetData(Action okFunc)
        {
            OKFunc = okFunc;
        }
    }
}

