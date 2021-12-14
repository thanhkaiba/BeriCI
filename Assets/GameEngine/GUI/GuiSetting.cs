using Piratera.Sound;
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

        [SerializeField]
        private Sprite[] iconSoundSprites;

        [SerializeField]
        private Sprite[] iconMusicSprites;

        [SerializeField]
        private Image iconSound;

        [SerializeField]
        private Image iconMusic;


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

            SoundMgr.SoundOn = !SoundMgr.SoundOn;
            iconSound.sprite = SoundMgr.SoundOn ? iconSoundSprites[1] : iconSoundSprites[0];
            iconMusic.SetNativeSize();
        }

        public void ToggleMusic()
        {
            SoundMgr.MusicOn = !SoundMgr.MusicOn;
            iconMusic.sprite = SoundMgr.MusicOn ? iconMusicSprites[1] : iconMusicSprites[0];
            iconMusic.SetNativeSize();
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
