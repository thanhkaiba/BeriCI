

using Sfs2X.Entities.Data;

namespace Piratera.Cheat
{
    public static class CheatMgr
    {
        public static void CheatBotAndPlay(SFSObject sailorCheatData)
        {
            NetworkController.Send(SFSAction.COMBAT_BOT, sailorCheatData);
        }

        public static void CheatSailor(string name,  string quality = "0", string level = "0")
        {
            SFSObject data = new SFSObject();
            data.PutUtfString("cheat_text",
                  name + "|"
                + level + "|"
                + quality);
            NetworkController.Send(SFSAction.CHEAT_SAILOR, data);
            NetworkController.Send(SFSAction.LOAD_LIST_HERO_INFO);
        }
    }
}
