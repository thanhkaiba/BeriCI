using System;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class GuiBuySlot : BaseGui
    {
        [SerializeField]
        private Text textBeriCost;
        [SerializeField]
        private Button buttonBuy;

        protected override void Start()
        {
            base.Start();
            InitPackData();
        }
        public void InitPackData()
        {
            EnableButtonBuy(true);
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
        }

        public void OnBuyStamina()
        {
            
            if (UserData.Instance.IsEnoughBeri(100))
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

 
    }
}
