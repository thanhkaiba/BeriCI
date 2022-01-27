using Piratera.GUI;
using Piratera.Network;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.UI;

#if PIRATERA_DEV || PIRATERA_QC

namespace Piratera.Cheat
{

    public class PopupCheatSailorInfo : BaseGui
    {
        [SerializeField]
        private Text textCheatSailor_name;
        [SerializeField]
        private InputField textCheatSailor_quality;

        [SerializeField]
        private InputField textCheatSailor_level;

        [SerializeField]
        private InputField textCheatSailor_exp;

        [SerializeField]
        private InputField textCheatSailor_star;

        public string sailorId;

        protected override void Start()
        {
            SailorModel sailor = CrewData.Instance.GetSailorModel(sailorId);
            textCheatSailor_name.text = sailor.name;
            textCheatSailor_quality.text = sailor.quality.ToString();
            textCheatSailor_level.text = sailor.level.ToString();
            textCheatSailor_exp.text = sailor.exp.ToString();
            textCheatSailor_star.text = sailor.star.ToString();
            NetworkController.AddServerActionListener(OnReceiveServerAction);
        }


        public void SendCheatSailor()
        {
            int quantity = int.Parse(textCheatSailor_quality.text);
            int level = int.Parse(textCheatSailor_level.text);
            long exp = long.Parse(textCheatSailor_exp.text);
            byte star = byte.Parse(textCheatSailor_star.text);
            GuiManager.Instance.ShowGuiWaiting(true);
            CheatMgr.CheatSailorQuantity(sailorId, quantity, level, exp, star);
        }

        private void OnDestroy()
        {
            NetworkController.RemoveServerActionListener(OnReceiveServerAction);
        }

        private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
        {
            GuiManager.Instance.ShowGuiWaiting(false);
            if (action == SFSAction.CHEAT_SAILOR_QUANTITY)
            {
                GuiManager.Instance.ShowGuiWaiting(false);
                if (errorCode != SFSErrorCode.SUCCESS)
                {
                    GameUtils.ShowPopupPacketError(errorCode);
                }
                else
                {
                    SailorModel sailor = CrewData.Instance.GetSailorModel(packet.GetUtfString("sid"));
                    sailor.quality = packet.GetInt("quality");
                    sailor.level = packet.GetInt("level");
                    sailor.exp = packet.GetLong("exp");
                    sailor.exp = packet.GetByte("star");

                    GameEvent.SailorInfoChanged.Invoke(sailor);
                }
            }
        }
    }
}

#endif