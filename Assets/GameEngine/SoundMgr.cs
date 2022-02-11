using UnityEngine;

namespace Piratera.Sound
{
    public enum PirateraMusic
    {
        LOBBY,
        LOGIN,
        COMBAT
    }

    public enum PirateraSoundEffect
    {
        WIN,
        LOSE,
        DRAW,
        RECEIVE_GIFT,
        OPEN_SLOT,
        SUMMON,
    }

    class SoundMgr : MonoBehaviour
    {

        private static string MUSIC_TOGGLE_KEY = "music-toggle";
        private static string SOUND_FX_TOGGLE_KEY = "sound-fx-toggle";

        [Header("Sound Effect")]
        [SerializeField]
        private AudioClip buttonTapSound;
        [SerializeField]
        private AudioClip win;
        [SerializeField]
        private AudioClip lose;
        [SerializeField]
        private AudioClip draw;

        [SerializeField]
        private AudioClip receiveGift;

        [SerializeField]
        private AudioClip openSlot;
        [SerializeField]
        private AudioClip summon;

        [Header("Voice")]
        [SerializeField]
        private AudioClip[] findMatchVoice;


        [Header("Background Music")]
        [SerializeField]
        public AudioClip LobbyMusic;

        [SerializeField]
        public AudioClip CombatMusic;

        [Header("Sound Sailor")]
        [SerializeField]
        private AudioClip[] skills;
        [SerializeField]               // sắp xếp theo thứ tự trong bảng chữ cái Alex , Galdalf , Helti .......
        private AudioClip[] attacks;

        [Header("Volume")]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float soundVolume = 1;
        public float SoundVolume
        {
            get
            {
                return soundVolume;
            }
            set
            {
                soundEfectPlayer.volume = value;
                soundVolume = value;
            }
        }
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float musicVolume = 1;

        public float MusicVolume
        {
            get
            {
                return musicVolume;
            }
            set
            {
                musicPlayer.volume = value;
                musicVolume = value;
            }
        }


        [Header("Toggle")]
        [SerializeField]
        private bool soundOn;
        public static bool SoundOn
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.soundOn;
                }
                return false;
            }
            set
            {
                if (Instance != null)
                {
                    PlayerPrefs.SetInt(SOUND_FX_TOGGLE_KEY, value ? 1 : 0);
                    Instance.soundOn = value;
                    if (value)
                    {
                        Instance.soundEfectPlayer.UnPause();
                    }
                    else
                    {
                        Instance.soundEfectPlayer.Pause();
                    }
                }

            }
        }

        [SerializeField]
        private bool musicOn;
        public static bool MusicOn
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.musicOn;
                }
                return false;
            }
            set
            {
                if (Instance != null)
                {
                    PlayerPrefs.SetInt(MUSIC_TOGGLE_KEY, value ? 1 : 0);
                    Instance.musicOn = value;
                    if (value)
                    {
                        Instance.musicPlayer.UnPause();
                    }
                    else
                    {
                        Instance.musicPlayer.Pause();
                    }
                }

            }
        }


        [SerializeField]
        private AudioSource soundEfectPlayer;
        [SerializeField]
        private AudioSource musicPlayer;
        public static SoundMgr Instance;


        void Awake()
        {

            if (Instance == null)
            {

#if UNITY_WEBGL
                // if sound play in start game, some browser won't load game 
                musicOn = false;
                soundOn = false;
#else
                musicOn = PlayerPrefs.GetInt(MUSIC_TOGGLE_KEY, 1) == 1;
                soundOn = PlayerPrefs.GetInt(SOUND_FX_TOGGLE_KEY, 1) == 1;
#endif
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);


        }

        public static void SetSoundFxSpeed(float pitch)
        {
            if (Instance != null)
            {
                Instance.soundEfectPlayer.pitch = pitch;
            }
        }

        public static void SetSoundFxVolume(float volume)
        {
            if (Instance != null)
            {
                Instance.SoundVolume = volume;
            }
        }

        public static void PlayTabSound()
        {
            if (Instance != null)
            {
                Instance.PlaySoundEffect(Instance.buttonTapSound);
            }
        }

        public static void PlaySound(PirateraSoundEffect sound)
        {
            if (Instance != null)
            {
                switch (sound)
                {
                    case PirateraSoundEffect.DRAW:
                        Instance.PlaySoundEffect(Instance.draw);
                        break;
                    case PirateraSoundEffect.WIN:
                        Instance.PlaySoundEffect(Instance.win);
                        break;
                    case PirateraSoundEffect.LOSE:
                        Instance.PlaySoundEffect(Instance.lose);
                        break;
                    case PirateraSoundEffect.RECEIVE_GIFT:
                        Instance.PlaySoundEffect(Instance.receiveGift);
                        break;
                    case PirateraSoundEffect.OPEN_SLOT:
                        Instance.PlaySoundEffect(Instance.openSlot);
                        break;
                    case PirateraSoundEffect.SUMMON:
                        Instance.PlaySoundEffect(Instance.summon);
                        break;
                }

            }
        }

        public static void PlaySound(AudioClip audioClip)
        {
            if (Instance != null)
            {
                Instance.PlaySoundEffect(audioClip);
            }
        }

        private void PlaySoundEffect(AudioClip audioClip)
        {
            if (soundOn)
            {
                
                soundEfectPlayer.PlayOneShot(audioClip, soundVolume);
            }
        }
        public static void PlaySoundSkillSailor(int index)
        {
            if (Instance != null)
            {
                Instance.PlaySoundEffect(Instance.skills[index]);
            }
        }
        public static void PlaySoundAttackSailor(int index)
        {
            if (Instance != null)
            {
                Instance.PlaySoundEffect(Instance.attacks[index]);
            }
        }
        private void PlayBackgroundMusic(AudioClip music)
        {
            if (music == musicPlayer.clip)
            {
                return;
            }
            musicPlayer.clip = music;
            musicPlayer.Stop();
            musicPlayer.loop = true;
            musicPlayer.Play();
            if (!MusicOn)
            {

                musicPlayer.Pause();

            }
        }

        public static void PlayFindMatchSound()
        {
            if (Instance != null)
            {
                Instance.PlaySoundEffect(Instance.findMatchVoice[Random.Range(0, Instance.findMatchVoice.Length)]);
            }
        }

        private void PlayBackgroundMusic(AudioClip music, float volume)
        {
            MusicVolume = volume;
            PlayBackgroundMusic(music);
        }

        public static void PlayBGMusic(PirateraMusic music)
        {
            if (Instance != null)
            {
                switch (music)
                {
                    case PirateraMusic.LOBBY:
                        Instance.PlayBackgroundMusic(Instance.LobbyMusic, 0.15f);
                        break;
                    case PirateraMusic.LOGIN:
                        Instance.PlayBackgroundMusic(Instance.LobbyMusic, 0.3f);
                        break;
                    case PirateraMusic.COMBAT:
                        Instance.PlayBackgroundMusic(Instance.CombatMusic, 0.25f);
                        break;

                }
            }
        }
    }
}
