using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserData : Singleton<UserData>
{
    public string uid { get; private set; }

    // for sailors aren't in squad
    public List<SailorModel> sailors;

    public Dictionary<int, SailorModel> squad;

    protected override void OnAwake()
    {
        
    }
}
