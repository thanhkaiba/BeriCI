using Piratera.Sound;
using Piratera.Utils;
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
        private Image userAvatar;

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

        private LoadAvatarUtils loadAvatar;



        protected override void Start()
        {
            base.Start();
            loadAvatar = GetComponent<LoadAvatarUtils>();
            textName.text = UserData.Instance.Username;
            textUID.text = UserData.Instance.UID;
            loadAvatar.LoadAvatar(userAvatar, UserData.Instance.Avatar);
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
