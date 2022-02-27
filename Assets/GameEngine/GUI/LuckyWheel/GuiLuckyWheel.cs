using DG.Tweening;
using Piratera.Config;
using Piratera.Network;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class GuiLuckyWheel : BaseGui
    {
        [SerializeField]
        private Transform background;

    
        [SerializeField]
        private Button buttonSpin;

        [SerializeField]
        private GameObject[] slots;

        [SerializeField]
        private Sprite[] spriteGifts;


     


        protected override void Start()
        {
            base.Start();
            NetworkController.AddServerActionListener(OnReceiveServerAction);
            InitData();
        }
        public void InitData()
        {
            buttonSpin.interactable = !PirateWheelData.Instance.IsWaiting();

            string[] gifts = GlobalConfigs.PirateWheelConfig.ListItems;

            for(int i = 0; i < gifts.Length; i++)
            {
                string giftString = gifts[i];
                string[] data = giftString.Split(':');
                string gift = data[1];
                string quantity = data[0];

                slots[i].transform.Find("Quantity").GetComponent<Text>().text = quantity;
                Image icon = slots[i].transform.Find("Icon").GetComponent<Image>();

                switch (gift)
                {
                    case "beri":
                        {
                            icon.sprite = spriteGifts[0];
                            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(49.068f, 48.8367f);
                            break;
                        }
                    case "stamina":
                        {
                            icon.sprite = spriteGifts[1];
                            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(41.0958f, 57.1768f);
                            break;
                        }
                }
            

            }
       

        }


        private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
        {
            if (action == SFSAction.PIRATE_WHEEL)
            {
                GuiManager.Instance.ShowGuiWaiting(false);
                OnClose();

            }
        }



        public void OnClose()
        {
            RunDestroy();
            NetworkController.RemoveServerActionListener(OnReceiveServerAction);
        }
        public void SendSpin()
        {
            GuiManager.Instance.ShowGuiWaiting(true);
            NetworkController.Send(SFSAction.PIRATE_WHEEL);
        }
      
      
    }
}
