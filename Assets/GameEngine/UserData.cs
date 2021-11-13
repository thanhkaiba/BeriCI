
using Sfs2X.Entities;
using UnityEngine;

public class UserData : Singleton<UserData>
{

    private class UserInforPropertiesKey
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
    }

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


    public void LoadExpConfig()
    {
        LevelConfig = Resources.Load<UserLevelConfig>("ScriptableObject/UserLevel/UserLevel");
        StaminaConfig = Resources.Load<UserStaminaConfig>("ScriptableObject/Stamina/Stamina");
    }

    public float GetExpProgress()
    {
        return (Exp * 1.0F) / LevelConfig.GetExpNeed(Level);
    }

    public void OnUserVariablesUpdate(User user)
    {

        Avatar = user.GetVariable(UserInforPropertiesKey.AVATAR).GetStringValue();
        Username = user.Name;
        Beri = (long)user.GetVariable(UserInforPropertiesKey.BERI).GetDoubleValue();
        Stamina = user.GetVariable(UserInforPropertiesKey.STAMINA).GetIntValue();
        Exp = (long)user.GetVariable(UserInforPropertiesKey.EXP).GetDoubleValue();
        Level = user.GetVariable(UserInforPropertiesKey.LEVEL).GetIntValue();
        LastCountStamina = (long)user.GetVariable(UserInforPropertiesKey.LAST_COUNT).GetDoubleValue();
        TimeBuyStaminaToday = user.GetVariable(UserInforPropertiesKey.TIME_BUY_STAMINA_TODAY).GetIntValue();
        GameEvent.UserDataChange.Invoke();
   
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
        return  $"{Stamina}/{StaminaConfig.max_stamina}";
    }
}
