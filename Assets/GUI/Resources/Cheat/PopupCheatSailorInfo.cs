using Piratera.GUI;
using UnityEngine;
using UnityEngine.UI;


namespace Piratera.Cheat
{

    public class PopupCheatSailorInfo : BaseGui
    {
        [SerializeField]
        private Text textCheatSailor_name;
        [SerializeField]
        private Text textCheatSailor_quality;

        public string sailorId;

        protected override void Start()
        {
            SailorModel sailor = CrewData.Instance.GetSailorModel(sailorId);
            textCheatSailor_name.text = sailor.name;
        }

        public void SendCheatSailor()
        {
            CheatMgr.CheatSailorQuantity(sailorId, int.Parse(textCheatSailor_quality.text));
        }
    }
}
