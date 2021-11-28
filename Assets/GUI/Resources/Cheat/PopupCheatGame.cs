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
            SFSObject data = new SFSObject();
            data.PutUtfString("cheat_text",
                textCheatSailor_name.text + "|"
                + textCheatSailor_level.text + "|"
                + textCheatSailor_quality.text);
            NetworkController.Send(SFSAction.CHEAT_SAILOR, data);
            NetworkController.Send(SFSAction.LOAD_LIST_HERO_INFO);
        }
     
    }
}


