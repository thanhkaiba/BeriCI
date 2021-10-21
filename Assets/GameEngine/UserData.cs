using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserData : Singleton<UserData>
{
    public string uid { get; private set; }
    public string username { get; private set; }
    public string avatar { get; private set; }
   
}
