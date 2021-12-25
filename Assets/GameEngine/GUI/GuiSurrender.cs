using DG.Tweening;
using Piratera.Network;
using Piratera.Utils;
using Sfs2X.Entities.Data;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class GuiSurrender : BaseGui
    {

        [SerializeField]
        GameObject beri;
        [SerializeField]
        private Transform background;
        [SerializeField]
        private Button btn;

        protected override void Start()
        {
            Appear();
        }

        private void Awake()
        {
            NetworkController.AddServerActionListener(OnReceiveServerAction);
        }

        private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
        {
            switch (action)
            {
                case SFSAction.PVE_SURRENDER:
                    if (errorCode == SFSErrorCode.SUCCESS)
                    {
                        GuiManager.Instance.ShowGuiWaiting(false);
                        float jumPower = 40;
                        Vector3 pos = beri.transform.position;
                        pos.y += jumPower;
                        beri.transform.DOJump(pos, jumPower, 1, .5f).OnComplete(() => SceneManager.LoadScene("SceneLobby"));
                    }
                    break;
            }
        }

        private void OnDestroy()
        {
            NetworkController.RemoveServerActionListener(OnReceiveServerAction);
        }

        public void ConfirmSur()
        {
            NetworkController.SendSurrenderPVEToSever();
            btn.enabled = false;
        }

        public void OnClose()
        {
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
            s.SetTarget(transform).SetLink(gameObject);
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
            s.SetTarget(transform).SetLink(gameObject);
            var fog = GetComponent<HaveFog>();
            if (fog) fog.FadeOut(0.1f);
        }
    }
}
