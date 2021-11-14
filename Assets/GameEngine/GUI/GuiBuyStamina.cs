using System;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class GuiBuyStamina : MonoBehaviour
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


        public void InitPackData()
        {
            UserStaminaConfig staminaConfig = UserData.Instance.StaminaConfig;
            textStaminaValue.text = "" + staminaConfig.statmina_buy_value;
            if (UserData.Instance.TimeBuyStaminaToday < staminaConfig.costs.Length)
            {
                textBeriCost.text = "" + staminaConfig.costs[UserData.Instance.TimeBuyStaminaToday];
                EnableButtonBuy(true);

            }
            else
            {
                EnableButtonBuy(false);
            }

            UpdateCurrentStamina();
            GameEvent.UserDataChange.AddListener(UpdateCurrentStamina);
        }

        public void UpdateCurrentStamina()
        {
            textCurrentStamina.text = UserData.Instance.GetCurrentStaminaFormat();
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

        private void Start()
        {
            InitPackData();
        }

        public void OnClose()
        {
            Destroy(gameObject);
        }

        public void OnBuyStamina()
        {
            UserStaminaConfig staminaConfig = UserData.Instance.StaminaConfig;
            

            if (UserData.Instance.IsEnoughBeri(staminaConfig.costs[UserData.Instance.TimeBuyStaminaToday]))
            {
                NetworkController.Send(SFSAction.BUY_STAMINA);
                OnClose();
            }
            else
            {
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
