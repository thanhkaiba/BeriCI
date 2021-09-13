using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils : MonoBehaviour
{
    public static GameUtils Instance { get; private set; }
    public void Awake()
    {
        Debug.Log("FIRST TIME APPEAR");
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
    public Sailor CreateSailor(string name)
    {
        GameObject characterGO;
        Sailor character;
        switch (name)
        {
            case "demo":
                characterGO = Instantiate(Resources.Load<GameObject>("characters/sword_man"));
                character = characterGO.AddComponent<DemoSailor>() as Sailor;
                break;
            case "demo2":
                characterGO = Instantiate(Resources.Load<GameObject>("characters/goblin_archer"));
                character = characterGO.AddComponent<DemoSailor2>() as DemoSailor2;
                break;
            default:
                characterGO = Instantiate(Resources.Load<GameObject>("characters/sword_man"));
                character = characterGO.AddComponent<Sailor>() as Sailor;
                break;
        }

        return character;
    }
}
 