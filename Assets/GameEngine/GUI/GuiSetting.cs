using Sfs2X.Entities.Data;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class GuiSetting : BaseGui
    {
        [SerializeField]
        private Text textName;
        [SerializeField]
        private Text textVersion;
        [SerializeField]
        private Text textUID;
        [SerializeField]
        private string whitePaperUrl = "https://piratera.io/";
        private string website = "https://piratera.io/";
 

        protected override void Start()
        {
            base.Start();
            textName.text = UserData.Instance.Username;
            textUID.text = UserData.Instance.UID;
        }

        public void Logout()
        {
            NetworkController.Logout();
        }

        public void Report()
        {

        }

        public void OnClose()
        {
            RunDestroy();
        }

        public void ToggleSound()
        {
            AudioListener.pause = !AudioListener.pause;
        }

        public void OpenWhitePaper()
        {
            Application.OpenURL(whitePaperUrl);
        }

        public void OpenWebsite()
        {
            Application.OpenURL(website);
        }

    }
}
