using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConfigs : MonoBehaviour
{
    public static GlobalConfigs Instance { get; private set; }
    public void Awake()
    {
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

    public ContainerClassBonus ClassBonus;
}
