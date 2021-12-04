using Piratera.Cheat;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.UI;


namespace Piratera.GUI
{
    public class PopupCheatGame : BaseGui
    {
        [SerializeField]
        private Text textCheatSailor_name;
        [SerializeField]
        private Text textCheatSailor_level;
        [SerializeField]
        private Text textCheatSailor_quality;


        public void SendCheatSailor()
        {
            

            CheatMgr.CheatSailor(textCheatSailor_name.text, textCheatSailor_quality.text, textCheatSailor_level.text);


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


}


