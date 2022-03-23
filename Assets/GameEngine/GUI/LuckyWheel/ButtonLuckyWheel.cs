using System;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class ButtonLuckyWheel: MonoBehaviour
    {
        [SerializeField]
        private Text luckyWheelCountDown;

        [SerializeField]
        private GameObject wheelAlert;

        [SerializeField]
        private Sprite[] sprites;

        private void Start()
        {
            if (CrewData.Instance.IsEmpty())
            {
                gameObject.SetActive(false);
            }
        }



        private void Update()
        {

            if (PirateWheelData.Instance.IsWaiting())
            {
                TimeSpan remaining = TimeSpan.FromMilliseconds(PirateWheelData.Instance.TimeToHaveNewRoll());
                luckyWheelCountDown.text = string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds);
                wheelAlert.SetActive(false);
                GetComponent<Button>().interactable = false;
                GetComponent<Image>().sprite = sprites[3];
            }
            else
            {
                luckyWheelCountDown.text = "Lucky Spin";
                wheelAlert.SetActive(true);

                GetComponent<Button>().interactable = true;
                GetComponent<Image>().sprite = sprites[2];


            }
        }

        public void ShowLuckyWheel()
        {

            GuiManager.Instance.AddGui<GuiLuckyWheel>("Prefap/GuiLuckyWheel");
        }
    }

    
}
