using System;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class GuiBuyStamina : BaseGui
    {
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

        protected override void Start()
        {
            base.Start();
            InitPackData();
        }
        public void InitPackData()
        {
            UserStaminaConfig staminaConfig = UserData.Instance.StaminaConfig;
            textStaminaValue.text = "" + staminaConfig.statmina_buy_value;
            UpdateCurrentStamina();
            GameEvent.UserStaminaChanged.AddListener(UpdateCurrentStamina);
        }

        private void UpdateCurrentStamina(int arg0, int arg1)
        {
            UpdateCurrentStamina();
        }

        public void UpdateCurrentStamina()
        {
            GuiManager.Instance.ShowGuiWaiting(false);
            textCurrentStamina.text = UserData.Instance.GetCurrentStaminaFormat();

            UserStaminaConfig staminaConfig = UserData.Instance.StaminaConfig;
            if (UserData.Instance.TimeBuyStaminaToday < staminaConfig.costs.Length)
            {
                textBeriCost.text = "" + staminaConfig.costs[UserData.Instance.TimeBuyStaminaToday];
                EnableButtonBuy(true);

            }
            else
            {
                EnableButtonBuy(false);
            }

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
            RunDestroy();
            GameEvent.UserStaminaChanged.RemoveListener(UpdateCurrentStamina);
        }

        public void OnBuyStamina()
        {
            UserStaminaConfig staminaConfig = UserData.Instance.StaminaConfig;
            

            if (UserData.Instance.IsEnoughBeri(staminaConfig.costs[UserData.Instance.TimeBuyStaminaToday]))
            {
                GuiManager.Instance.ShowGuiWaiting(true);

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

            if (UserData.Instance.IsRecorveringStamina())
            {
                TimeSpan remaining = TimeSpan.FromMilliseconds(UserData.Instance.TimeToHaveNewStamina());
                textCountDownStamina.text = string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds);
            }
            else
            {
                textCountDownStamina.text = "";

            }
        }
    }
}
