using Piratera.GUI;
using Sfs2X.Entities.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Piratera.Cheat
{
#if PIRATERA_DEV || PIRATERA_QC

    public class PopupCheatGame : BaseGui
    {
        [SerializeField]
        private Text textCheatSailor_name;
        [SerializeField]
        private Text textCheatSailor_level;
        [SerializeField]
        private Text textCheatSailor_quality;
        [SerializeField]
        private InputField textCheatBeri_quality;
        [SerializeField]
        private InputField textCheatStamina_quality;
        [SerializeField]
        private InputField textCheatRank_quality;


        private void Awake()
        {
            GameEvent.UserDataChanged.AddListener(UpdatePVERank);
            textCheatRank_quality.text = UserData.Instance.PVERank.ToString();
        }

        private void UpdatePVERank(List<string> arg0)
        {
            textCheatRank_quality.text = UserData.Instance.PVERank.ToString();
        }

        private void OnDestroy()
        {
            GameEvent.UserDataChanged.RemoveListener(UpdatePVERank);
        }


        public void SendCheatSailor()
        {


            CheatMgr.CheatSailor(textCheatSailor_name.text, textCheatSailor_quality.text, textCheatSailor_level.text);


        }

        public void SendCheatStamina()
        {
            if (string.IsNullOrEmpty(textCheatStamina_quality.text))
            {
                return;
            }
            CheatMgr.CheatResource("stamina", int.Parse(textCheatStamina_quality.text));
        }

        public void SendCheatBeri()
        {
            if (string.IsNullOrEmpty(textCheatBeri_quality.text))
            {
                return;
            }
            CheatMgr.CheatResource("beri", int.Parse(textCheatBeri_quality.text));
        }

        public void SendCheatRank()
        {
            if (string.IsNullOrEmpty(textCheatRank_quality.text))
            {
                return;
            }
            CheatMgr.CheatRank(int.Parse(textCheatRank_quality.text));
        }

        public void CheatBot()
        {
            var fgl = CrewData.Instance.FightingTeam;

            SFSObject sailorCheatData = new SFSObject();
            ISFSArray sailorcheats = new SFSArray();

            for (short x = 0; x < 3; x++)
            {
                for (short y = 0; y < 3; y++)
                {
                    string sailorId = fgl.SailorIdAt(x, y);
                    SailorModel m = CrewData.Instance.GetSailorModel(sailorId);
                    if (m != null)
                    {
                        ISFSObject pos = new SFSObject();
                        pos.PutByte("x", (byte)x);
                        pos.PutByte("y", (byte)y);

                        ISFSObject sailorcheat = new SFSObject();
                        sailorcheat.PutUtfString("name", m.config_stats.root_name);
                        sailorcheat.PutSFSObject("pos", pos);

                        sailorcheats.AddSFSObject(sailorcheat);
                    }
                }
            }
            sailorCheatData.PutSFSArray("sailorcheats", sailorcheats);

            CheatMgr.CheatBotAndPlay(sailorCheatData);

        }

    }
#endif
}
