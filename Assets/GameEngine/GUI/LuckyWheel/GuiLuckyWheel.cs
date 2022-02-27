using DG.Tweening;
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
        private Text textCountDown;

        [SerializeField]
        private Button buttonSpin;


        protected override void Start()
        {
            base.Start();
            NetworkController.AddServerActionListener(OnReceiveServerAction);
            InitData();
        }
        public void InitData()
        {
            buttonSpin.interactable = !PirateWheelData.Instance.IsWaiting();


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
