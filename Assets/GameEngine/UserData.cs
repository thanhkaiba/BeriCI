
using System.Collections.Generic;
using Sfs2X.Entities;
using UnityEngine;

public class UserInfoPropertiesKey
{

    public const string UID = "uid";
    public const string USERNAME = "username";
    public const string BERI = "beri";
    public const string STAMINA = "stamina";
    public const string LAST_COUNT = "last_count";
    public const string EXP = "exp";
    public const string LEVEL = "level";
    public const string AVATAR = "avatar";
    public const string TIME_BUY_STAMINA_TODAY = "time_buy_to_day";
    public const string NUMBER_OF_POSITIONS = "number_of_positions";
}

public class UserData : Singleton<UserData>
{

   

    protected override void OnAwake()
    {
        LoadExpConfig();
    }
    public UserLevelConfig LevelConfig;
    public UserStaminaConfig StaminaConfig;
    public string UID { get;  set; }
    public string Username { get;  set; }
    public string Avatar { get;  set; }
    public long Beri { get;  set; }
    public int Stamina { get;  set; }
    public long Exp { get;  set; }

    /// <summary>
    /// Level of User, start from 1
    /// </summary>
    public int Level { get;  set; } 
    public long LastCountStamina { get;  set; }

    public int TimeBuyStaminaToday { get; set; }

    public int NumSlot { get; set; }


    public void LoadExpConfig()
    {
        LevelConfig = Resources.Load<UserLevelConfig>("ScriptableObject/UserLevel/UserLevel");
        StaminaConfig = Resources.Load<UserStaminaConfig>("ScriptableObject/Stamina/Stamina");
    }

    public float GetExpProgress()
    {
        return (Exp * 1.0F) / LevelConfig.GetExpNeed(Level);
    }

    public void OnUserVariablesUpdate(User user, List<string> changedVars)
    {

        Avatar = user.GetVariable(UserInfoPropertiesKey.AVATAR).GetStringValue();
        Username = user.Name;
        Exp = (long)user.GetVariable(UserInfoPropertiesKey.EXP).GetDoubleValue();
        Level = user.GetVariable(UserInfoPropertiesKey.LEVEL).GetIntValue();
        LastCountStamina = (long)user.GetVariable(UserInfoPropertiesKey.LAST_COUNT).GetDoubleValue();
        TimeBuyStaminaToday = user.GetVariable(UserInfoPropertiesKey.TIME_BUY_STAMINA_TODAY).GetIntValue();
        NumSlot = user.GetVariable(UserInfoPropertiesKey.NUMBER_OF_POSITIONS).GetIntValue();
        GameEvent.UserDataChanged.Invoke(changedVars);

        long oldBeri = Beri;
        Beri = (long)user.GetVariable(UserInfoPropertiesKey.BERI).GetDoubleValue();

        if (oldBeri != Beri)
        {
            GameEvent.UserBeriChanged.Invoke(oldBeri, Beri);
        }

        int oldStamina = Stamina;
        Stamina = user.GetVariable(UserInfoPropertiesKey.STAMINA).GetIntValue();

        if (oldStamina != Stamina)
        {
            GameEvent.UserStaminaChanged.Invoke(oldStamina, Stamina);
        }

    }

    public void MinusStamina(int value)
    {
        GameEvent.UserStaminaChanged.Invoke(Stamina, Stamina - value);
    }

    public void OnUserVariablesUpdate(User user)
    {

        OnUserVariablesUpdate(user, new List<string>());

    }

    public bool IsRecorveringStamina()
    {
        return Stamina < StaminaConfig.max_stamina;
    }

    public long TimeToHaveNewStamina()
    {
        if (Stamina > StaminaConfig.max_stamina)
        {
            return -1;
        }

        long now = GameTimeMgr.GetCurrentTime();
        long delta = now - LastCountStamina;
        int recoveringTime = StaminaConfig.recovering_time * 1000;
        delta = recoveringTime - delta % recoveringTime;
        return delta;
    }

    public string GetCurrentStaminaFormat()
    {
        return  GetStaminaFormat(Stamina);
    }

    public string GetStaminaFormat(int stamina)
    {
        return $"{stamina}/{StaminaConfig.max_stamina}";
    }
    public string _GetStaminaFormat(int stamina)
    {
        return $"{stamina}";
    }
    public int GetStamina()
    {
        return Stamina;
    }
    public bool IsEnoughBeri(long beri)
    {
        return Beri >= beri;
    }
}
