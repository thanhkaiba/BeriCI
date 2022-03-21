using Sfs2X.Entities;
using System.Collections.Generic;
using UnityEngine;

public class UserInfoPropertiesKey
{
    public const string UID = "id";
    public const string USERNAME = "displayName";
    public const string BERI = "beri";
    public const string STAMINA = "stamina";
    public const string LAST_COUNT = "last_count";
    public const string EXP = "exp";
    public const string LEVEL = "level";
    public const string AVATAR = "avatar";
    public const string TIME_BUY_STAMINA_TODAY = "time_buy_to_day";
    public const string NUMBER_OF_POSITIONS = "number_of_positions";
    public const string CREATE_AT = "createdAt";
    public const string PVE_RANK = "pve_rank";
    public const string PVE_COUNT = "pve_count";
}

public class UserData : Singleton<UserData>
{

    public string UID { get; set; }
    public string Username { get; set; }
    public string Avatar { get; set; }
    public long Beri { get; set; }
    public long Exp { get; set; }

    public long PVERank { get; set; }

    /// <summary>
    /// Level of User, start from 1
    /// </summary>
    public int Level { get; set; }
    public int NumSlot { get; set; }
    public long CreateAt { get; set; }
    public int PVECount { get; set; }

    public void OnUserVariablesUpdate(User user, List<string> changedVars)
    {

        Avatar = user.GetVariable(UserInfoPropertiesKey.AVATAR).GetStringValue();
        UID = user.GetVariable(UserInfoPropertiesKey.UID).GetStringValue();
        try
        {
            Username = user.GetVariable(UserInfoPropertiesKey.USERNAME).GetStringValue();
        }
        catch
        {
            Username = user.Name;
        }
        Exp = (long)user.GetVariable(UserInfoPropertiesKey.EXP).GetDoubleValue();
        Level = user.GetVariable(UserInfoPropertiesKey.LEVEL).GetIntValue();
        NumSlot = user.GetVariable(UserInfoPropertiesKey.NUMBER_OF_POSITIONS).GetIntValue();
        PVECount = user.GetVariable(UserInfoPropertiesKey.PVE_COUNT).GetIntValue();
        CreateAt = (long)user.GetVariable(UserInfoPropertiesKey.CREATE_AT).GetDoubleValue();
        try
        {
            PVERank = user.GetVariable(UserInfoPropertiesKey.PVE_RANK).GetIntValue();
        }
        catch
        {

        }
        GameEvent.UserDataChanged.Invoke(changedVars);

        long oldBeri = Beri;
        Beri = (long)user.GetVariable(UserInfoPropertiesKey.BERI).GetDoubleValue();

        if (oldBeri != Beri)
        {
            GameEvent.UserBeriChanged.Invoke(oldBeri, Beri);
        }


    }

    public void AddBeri(long quantity)
    {
        long oldBeri = Beri;
        Beri += quantity;
        GameEvent.UserBeriChanged.Invoke(oldBeri, Beri);
    }

   

    public void OnUserVariablesUpdate(User user)
    {

        OnUserVariablesUpdate(user, new List<string>());

    }

    public bool IsEnoughBeri(long beri)
    {
        return Beri >= beri;
    }

    public void Reset()
    {
        UID = "";
        Username = "";
        Avatar = "";
        Beri = 0;
        NumSlot = 0;
    }
}
