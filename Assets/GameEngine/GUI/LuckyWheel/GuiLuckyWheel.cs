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

        [SerializeField]
        private Image wheelCircle;

        [SerializeField]
        private GameObject iconLock;




        protected override void Start()
        {
            base.Start();
            NetworkController.AddServerActionListener(OnReceiveServerAction);
            InitData();
        }
        public void InitData()
        {
           
            string[] gifts = GlobalConfigs.PirateWheelConfig.ListItems;

            for(int i = 0; i < gifts.Length; i++)
            {
                string giftString = gifts[i];
                string[] data = giftString.Split(':');
                string gift = data[1];
                string quantity = data[0];

                Text text = slots[i].transform.Find("Quantity").GetComponent<Text>();
                text.text = quantity;
                Image icon = slots[i].transform.Find("Icon").GetComponent<Image>();

                switch (gift)
                {
                    case "beri":
                        {
                            icon.sprite = spriteGifts[0];
                            text.color = new Color32(45, 134, 188, 255);
                            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(49.068f, 48.8367f);
                            break;
                        }
                    case "stamina":
                        {
                            icon.sprite = spriteGifts[1];
                            text.color = new Color32(247, 185, 36, 255);
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
             

                if (errorCode != SFSErrorCode.SUCCESS)
                {
                    OnClose();
                }

            }
        }

        private void DoSpin()
        {
            Sequence seq = DOTween.Sequence();
            seq.SetLink(wheelCircle.gameObject);
            seq.SetTarget(wheelCircle.transform);
            seq.Append(wheelCircle.transform.DORotate(Vector3.forward * 360, 1f, RotateMode.FastBeyond360));

            seq.OnComplete(() => {
            if (PirateWheelData.Instance.Reward != null)
            {
                seq.Kill();
                float angle = GetAngleOfReward(PirateWheelData.Instance.Reward);
                wheelCircle.transform.DORotate(new Vector3(0, 0, 360 + angle), 1f + 1.2f/360 * angle, RotateMode.FastBeyond360).OnComplete(() => {
                    FlyGift(PirateWheelData.Instance.Reward);
                    PirateWheelData.Instance.Reward = null;
                });
            }
            else
                {
                    seq.Restart();

                }
            });
             
        }

        private void FlyGift(string giftString)
        {
            string gift = giftString.Split(':')[1];
            int quantity = int.Parse(giftString.Split(':')[0]);

            switch (gift)
            {
                case "stamina":
                    {
                        StaminaData.Instance.AddStamina(quantity);
                        GameEvent.FlyStamina.Invoke();
                        break;
                    }
                case "beri":
                    {
                        UserData.Instance.AddBeri(quantity);
                        GameEvent.FlyBeri.Invoke();
                        break;
                    }
            }
           
        }

        private float GetAngleOfReward(string reward)
        {
            int index = System.Array.IndexOf(GlobalConfigs.PirateWheelConfig.ListItems, reward);
            return 8 + 46 * index;
        }



        public void OnClose()
        {
            RunDestroy();
            NetworkController.RemoveServerActionListener(OnReceiveServerAction);
        }
        public void SendSpin()
        {
            DoSpin();
            GuiManager.Instance.ShowGuiWaiting(true);
            NetworkController.Send(SFSAction.PIRATE_WHEEL);
        }

        private void Update()
        {
            buttonSpin.interactable = !PirateWheelData.Instance.IsWaiting();
            iconLock.SetActive(PirateWheelData.Instance.IsWaiting());
        }

    }
}
