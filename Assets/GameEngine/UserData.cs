using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserData : MonoBehaviour
{
    public static UserData Instance { get; private set; }
    public void Awake()
    {
        Debug.Log("Create UserData");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
