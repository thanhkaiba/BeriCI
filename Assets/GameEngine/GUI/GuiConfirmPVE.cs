using DG.Tweening;
using Piratera.Network;
using Piratera.Sound;
using Piratera.Utils;
using Sfs2X.Entities.Data;
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
        private int priceStamina;
        [SerializeField]
        private GameObject fight, find, findBtn, backBtn;
        [SerializeField]
        private Text staminaCost;
        [SerializeField]
        private GameObject staminaMinus;
        [SerializeField]
        private GameObject iconFind, findTarget;
        public LobbyUI lobby;
        private Vector3 v;

        protected override void Start()
        {
            base.Start();
            SoundMgr.PlayFindMatchSound();
            priceStamina = GlobalConfigs.PvE.stamina_cost;
            staminaCost.text = "" + priceStamina;
          
            UpdateCurrentStamina();
            GameEvent.UserStaminaChanged.AddListener(UpdateCurrentStamina);
            NetworkController.AddServerActionListener(onReceiveServerAction);
            Appear();
            v = iconFind.transform.position - findTarget.transform.position;
        }

        private void onReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
        {
            if ((action == SFSAction.PVE_PLAY || action == SFSAction.COMBAT_PREPARE) && errorCode != SFSErrorCode.SUCCESS)
            {
                lobby.OnStaminaChanged(StaminaData.Instance.Stamina, StaminaData.Instance.Stamina);
                OnClose();
            }
        }

        private void UpdateCurrentStamina(int arg0, int arg1)
        {
            UpdateCurrentStamina();
        }


        private void UpdateCurrentStamina()
        {
            textCurrentStamina.text = StringUtils.ShortNumber(StaminaData.Instance.Stamina, 6);
        }

        public void OnStartFindGame()
        {
            if (StaminaData.Instance.Stamina < priceStamina)
            {
                GuiManager.Instance.ShowGuiWaiting(true);
                NetworkController.Send(SFSAction.GET_STAMINA_PACK);
            }
            else
            {
              
                backBtn.GetComponent<Button>().enabled = false;
                findBtn.GetComponent<Button>().enabled = false;
                textCurrentStamina.text = StringUtils.ShortNumber(StaminaData.Instance.Stamina - priceStamina, 6);
                lobby.OnStaminaChanged(StaminaData.Instance.Stamina, (StaminaData.Instance.Stamina - priceStamina));

                GameObject staminaMinusGO = Instantiate(staminaMinus, textCurrentStamina.transform);
                staminaMinusGO.GetComponent<Text>().text = "-" + priceStamina;
                var trs = staminaMinusGO.transform as RectTransform;
                
                
                Sequence seq = DOTween.Sequence();
                seq.Append(trs.DOAnchorPosY(100, 0.8f).SetRelative().SetEase(Ease.OutQuint));
                seq.Join(trs.GetComponent<CanvasGroup>().DOFade(0, 2f));
                seq.AppendCallback(() => Destroy(staminaMinusGO));
                seq.SetLink(gameObject).SetTarget(transform);
             

                Sequence s = DOTween.Sequence();
                s.AppendInterval(0.6f);
                s.AppendCallback(() =>
                {
                    fight.SetActive(false);
                    find.SetActive(true);
             

                });
                s.AppendInterval(1.5f);
                s.AppendCallback(() => NetworkController.Send(SFSAction.PVE_PLAY));
                s.SetLink(gameObject).SetTarget(transform);

                
            }

        }
        public void OnClose()
        {
            GameEvent.UserStaminaChanged.RemoveListener(UpdateCurrentStamina);
            NetworkController.RemoveServerActionListener(onReceiveServerAction);
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

        private void Update()
        {
            v = Quaternion.AngleAxis(Time.deltaTime * 120, Vector3.forward) * v;
            iconFind.transform.position = findTarget.transform.position + v;
        }
    }
}
