#if PIRATERA_DEV 
using Piratera.Network;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Piratera.Cheat
{
    public static class CheatMgr
    {
        public static void CheatBotAndPlay(SFSObject sailorCheatData)
        {
            NetworkController.Send(SFSAction.COMBAT_BOT, sailorCheatData);
        }

        public static void CheatSailor(string name, string quality = "0", string level = "0")
        {
            SFSObject data = new SFSObject();
            data.PutUtfString("cheat_text",
                  name + "|"
                + level + "|"
                + quality);
            NetworkController.Send(SFSAction.CHEAT_SAILOR, data);
            NetworkController.Send(SFSAction.LOAD_LIST_HERO_INFO);
        }

        public static void CheatResource(string resource, int quantity)
        {
            SFSObject data = new SFSObject();
            data.PutUtfString("resource", resource);
            data.PutInt("quantity", quantity);
            NetworkController.Send(SFSAction.CHEAT_RESOURCE, data);
        }

        public static void CheatRank(int rank)
        {
            SFSObject data = new SFSObject();
            data.PutInt("rank", rank);
            NetworkController.Send(SFSAction.CHEAT_RANK, data);
        }

        public static void CheatSailorQuantity(string sid, int quality)
        {
            Debug.Log($"Cheat SID: {sid} - quality {quality}");
            SFSObject data = new SFSObject();
            data.PutUtfString("sid", sid);
            data.PutInt("quality", quality);
            NetworkController.Send(SFSAction.CHEAT_SAILOR_QUANTITY, data);
        }
    }
}
#endif
