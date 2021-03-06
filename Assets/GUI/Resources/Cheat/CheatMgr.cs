#if PIRATERA_DEV || PIRATERA_QC
using Sfs2X.Entities.Data;

namespace Piratera.Cheat
{
    public static class CheatMgr
    {
        public static void CheatBotAndPlay(SFSObject sailorCheatData)
        {
            NetworkController.Send(Action.COMBAT_BOT, sailorCheatData);
        }

        public static void CheatSailor(string name, string quality = "0", string level = "0")
        {
            SFSObject data = new SFSObject();
            data.PutUtfString("cheat_text",
                  name + "|"
                + level + "|"
                + quality);
            NetworkController.Send(Action.CHEAT_SAILOR, data);
            NetworkController.Send(Action.LOAD_LIST_HERO_INFO);
        }

        public static void CheatResource(string resource, int quantity)
        {
            SFSObject data = new SFSObject();
            data.PutUtfString("resource", resource);
            data.PutInt("quantity", quantity);
            NetworkController.Send(Action.CHEAT_RESOURCE, data);
        }

        public static void CheatRank(int rank)
        {
            SFSObject data = new SFSObject();
            data.PutInt("rank", rank);
            NetworkController.Send(Action.CHEAT_RANK, data);
        }

        public static void CheatSailorQuantity(string sid, int quality, int level, long exp, byte star, int fight)
        {
            SFSObject data = new SFSObject();
            data.PutUtfString("sid", sid);
            data.PutInt("quality", quality);
            data.PutInt("level", level);
            data.PutLong("exp", exp);
            data.PutByte("star", star);
            data.PutInt("pve_count", fight);
            NetworkController.Send(Action.CHEAT_SAILOR_QUANTITY, data);
        }
    }
}
#endif
