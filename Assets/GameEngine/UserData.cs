using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserData : Singleton<UserData>
{
    public string UID { get;  set; }
    public string Username { get;  set; }
    public string Avatar { get;  set; }
    public long Beri { get;  set; }
    public int Stamina { get;  set; }
    public long Exp { get;  set; }
    public int Level { get;  set; }
    public long LastCountStamina { get;  set; }

}
