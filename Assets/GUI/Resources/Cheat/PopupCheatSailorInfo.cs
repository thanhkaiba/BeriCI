#if PIRATERA_DEV || PIRATERA_QC
using Piratera.GUI;
using Piratera.Network;
using Sfs2X.Entities.Data;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace Piratera.Cheat
{

    public class PopupCheatSailorInfo : BaseGui
    {
        [SerializeField]
        private Text textCheatSailor_name;
        [SerializeField]
        private InputField textCheatSailor_quality;

        public string sailorId;

        protected override void Start()
        {
            SailorModel sailor = CrewData.Instance.GetSailorModel(sailorId);
            textCheatSailor_name.text = sailor.name;
            textCheatSailor_quality.text = sailor.quality.ToString();
            NetworkController.AddServerActionListener(OnReceiveServerAction);
        }


        public void SendCheatSailor()
        {
            GuiManager.Instance.ShowGuiWaiting(true);
            CheatMgr.CheatSailorQuantity(sailorId, int.Parse(textCheatSailor_quality.text));
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

                    GameEvent.SailorInfoChanged.Invoke(sailor);
                }
            }
        }
    }
}

#endif