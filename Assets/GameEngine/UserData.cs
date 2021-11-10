
using UnityEngine;

public class UserData : Singleton<UserData>
{
    protected override void OnAwake()
    {
        LoadExpConfig();
    }
    private UserLevelConfig LevelConfig;
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


    public void LoadExpConfig()
    {
        LevelConfig = Resources.Load<UserLevelConfig>("ScriptableObject/UserLevel/UserLevel");
    }

    public float GetExpProgress()
    {
        return (Exp * 1.0F) / LevelConfig.GetExpNeed(Level);
    }

}
