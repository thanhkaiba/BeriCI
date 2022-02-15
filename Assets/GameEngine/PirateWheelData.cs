using Piratera.Config;
using Piratera.Utils;
using Sfs2X.Entities;
using UnityEngine;


public class PirateWheelData : Singleton<PirateWheelData>
{
    public int Roll;
    private long LastCountRoll;
    string listItems;

    public string GetRollFormat(string roll)
    {
        return $"roll";
    }
    public string getPrize()
    {
        string[] ps = GlobalConfigs.PirateWheelConfig.listItems;
        for (int i = 0; i < ps.Length; i++)
        {
            //Debug.Log(i);
            listItems += ps[i] + ":";
        }
        return listItems;
    }
    public int GetRoll()
    {
        return Roll;
    }

    public bool IsRecorveringRoll()
    {
        return Roll < 1;
    }
    public long TimeToHaveNewRoll()
    {
        if (Roll > 0)
        {
            return -1;
        }

        long now = GameTimeMgr.GetCurrentTime();

        long delta = now - LastCountRoll;
        int recoveringTime = GlobalConfigs.PirateWheelConfig.timeCycle * 1000;
        long remain = recoveringTime - delta % recoveringTime;


        return remain;
    }
    public void OnUserVariablesUpdate(User user)
    {
        LastCountRoll = (long)user.GetVariable(UserInfoPropertiesKey.LAST_COUNT).GetDoubleValue();
        int oldRoll= Roll;
        Roll = user.GetVariable(UserInfoPropertiesKey.ROLL).GetIntValue();

        if (oldRoll != Roll)
        {
            GameEvent.UserRollChanged.Invoke(oldRoll, Roll);
        }
    }
}
