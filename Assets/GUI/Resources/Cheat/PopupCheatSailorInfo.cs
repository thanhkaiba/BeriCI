using Piratera.GUI;
using Piratera.Network;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.UI;

#if PIRATERA_DEV || PIRATERA_QC

namespace Piratera.Cheat
{
    public class PopupCheatSailorInfo : MonoBehaviour
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

        [SerializeField]
        private InputField textCheatSailor_fight;

        public string sailorId;

        protected void Start()
        {
            SailorModel sailor = CrewData.Instance.GetSailorModel(sailorId);
            textCheatSailor_name.text = sailor.name;
            textCheatSailor_quality.text = sailor.quality.ToString();
            textCheatSailor_level.text = sailor.level.ToString();
            textCheatSailor_exp.text = sailor.exp.ToString();
            textCheatSailor_star.text = sailor.star.ToString();
            textCheatSailor_fight.text = sailor.pve_count.ToString();
            NetworkController.Listen(OnReceiveServerAction);
        }
        public void SendCheatSailor()
        {
            int quantity = int.Parse(textCheatSailor_quality.text);
            int level = int.Parse(textCheatSailor_level.text);
            long exp = long.Parse(textCheatSailor_exp.text);
            byte star = byte.Parse(textCheatSailor_star.text);
            int fight = int.Parse(textCheatSailor_fight.text);
            SceneTransition.Instance.ShowWaiting(true);
            CheatMgr.CheatSailorQuantity(sailorId, quantity, level, exp, star, fight);
        }
        private void OnDestroy()
        {
            NetworkController.RemoveListener(OnReceiveServerAction);
        }
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
        private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
        {
            SceneTransition.Instance.ShowWaiting(false);
            if (action == SFSAction.CHEAT_SAILOR_QUANTITY)
            {
                SceneTransition.Instance.ShowWaiting(false);
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
                    sailor.star = packet.GetByte("star");
                    sailor.pve_count = packet.GetInt("pve_count");

                    GameEvent.SailorInfoChanged.Invoke(sailor);
                }
            }
        }
    }
}

#endif