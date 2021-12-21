using Piratera.Network;
using Piratera.Sound;
using Piratera.Utils;
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
        private Text textCreateAt;

        [SerializeField]
        private UserAvatar userAvatar;

        [Header("Social")]
        [SerializeField]
        private string whitePaperUrl = "https://piratera.io/";
        [SerializeField]
        private string website = "https://piratera.io/";
        [SerializeField]
        private string facebookUrl = "https://piratera.io/";
        [SerializeField]
        private string telegramUrl = "https://piratera.io/";
        [SerializeField]
        private string twitterUrl = "https://piratera.io/";


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
#if PIRATERA_DEV
            textVersion.text = "DEV_" + Application.version;
#elif PIRATERA_QC
            textVersion.text = "QC_" + Application.version;
#else
            textVersion.text = "LIVE_" + Application.version;
#endif
            textName.text = UserData.Instance.Username;
            textUID.text = UserData.Instance.UID;
            userAvatar.LoadAvatar(UserData.Instance.Avatar);

            DateTime date = (new DateTime(1970, 1, 1)).AddMilliseconds(UserData.Instance.CreateAt);
            textCreateAt.text = date.ToString("MM/dd/yyyy");
            UpdateSoundIcon();
            UpdateMusicIcon();
            
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
            UpdateSoundIcon();
        }

        private void UpdateSoundIcon()
        {
            iconSound.sprite = SoundMgr.SoundOn ? iconSoundSprites[1] : iconSoundSprites[0];
            iconMusic.SetNativeSize();
        }

        public void ToggleMusic()
        {
            SoundMgr.MusicOn = !SoundMgr.MusicOn;
            UpdateMusicIcon();
        }

        private void UpdateMusicIcon()
        {
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


        public void OpenFacebook()
        {
            Application.OpenURL(facebookUrl);
        }

        public void OpenTwitter()
        {
            Application.OpenURL(twitterUrl);
        }

        public void OpenTelegram()
        {
            Application.OpenURL(telegramUrl);
        }
    }
}
