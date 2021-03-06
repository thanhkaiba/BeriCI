using Piratera.Config;
using Sfs2X.Entities.Data;

public class PirateWheelData : Singleton<PirateWheelData>
{
 
    private long LastCountRoll { get; set; }
    public string Reward { get; set; }

    public long TimeToHaveNewRoll()
    {
      
        long now = GameTimeMgr.GetCurrentTime();
        long delta = now - LastCountRoll;
        int recoveringTime = GlobalConfigs.PirateWheelConfig.TimeCycle * 1000;
        long remain = recoveringTime - delta % recoveringTime;

        return remain;
    }




    public bool IsWaiting()
    {
        long now = GameTimeMgr.GetCurrentTime();
        long delta = now - LastCountRoll;
        int recoveringTime = GlobalConfigs.PirateWheelConfig.TimeCycle * 1000;

        return delta < recoveringTime;
    }

    public void NewFromSFSObject(ISFSObject packet)
    {
        LastCountRoll = packet.GetLong("last_roll");
    }

    internal void ReceiveGiftPack(ISFSObject packet)
    {
        LastCountRoll = packet.GetLong("reward_epoch");
        Reward = packet.GetUtfString("reward");
        
    }
}
