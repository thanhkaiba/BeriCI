using DG.Tweening;
using Piratera.Config;
using Piratera.Network;
using Piratera.Sound;
using Sfs2X.Entities.Data;
using System;
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
        private Button buttonClose;

        [SerializeField]
        private GameObject[] slots;

        [SerializeField]
        private Sprite[] spriteGifts;

        [SerializeField]
        private Image wheelCircle;

        [SerializeField]
        private Text textCountdown;
        [SerializeField]
        private GameObject textSpin;

        private bool UpdateCountDown = false;




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
                            text.color = new Color32(157, 205, 233, 255);
                            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(49.068f, 48.8367f);
                            break;
                        }
                    case "stamina":
                        {
                            icon.sprite = spriteGifts[1];
                            text.color = new Color32(247, 245, 36, 255);
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
                SceneTransition.Instance.ShowWaiting(false);
          
                if (errorCode != SFSErrorCode.SUCCESS)
                {
                    OnClose();
                } else
                {
                    DoSpin();
                }

            }
        }

        private void DoSpin()
        {
            SoundMgr.PlaySound(PirateraSoundEffect.WHEEL_RUNNING);
            float angle = GetAngleOfReward(PirateWheelData.Instance.Reward);
            wheelCircle.transform.DORotate(new Vector3(0, 0, 360*5 + angle), 5f + 3.0f / 360 * angle, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine).OnComplete(() => {
                buttonClose.gameObject.SetActive(true);
                FlyGift(PirateWheelData.Instance.Reward);
                PirateWheelData.Instance.Reward = null;
                UpdateCountDown = true;
            });

        }

        private void FlyGift(string giftString)
        {
            SoundMgr.PlaySound(PirateraSoundEffect.RECEIVE_GIFT);
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
            return 45 * index;
        }



        public void OnClose()
        {
            RunDestroy();
            NetworkController.RemoveServerActionListener(OnReceiveServerAction);
        }
        public void SendSpin()
        {
            buttonClose.gameObject.SetActive(false);
            SceneTransition.Instance.ShowWaiting(true);
            NetworkController.Send(SFSAction.PIRATE_WHEEL);
        }

        private void Update()
        {
            buttonSpin.interactable = !PirateWheelData.Instance.IsWaiting();
           
            textCountdown.gameObject.SetActive(PirateWheelData.Instance.IsWaiting());

            if (PirateWheelData.Instance.IsWaiting() && UpdateCountDown)
            {
                TimeSpan remaining = TimeSpan.FromMilliseconds(PirateWheelData.Instance.TimeToHaveNewRoll());
                textCountdown.text = string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds);
            }
            else
            {
                textCountdown.gameObject.SetActive(false);
            }

            textSpin.SetActive(!textCountdown.gameObject.activeSelf);
        }

    }
}
