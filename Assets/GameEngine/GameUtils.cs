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
    public Character CreateRandomCharacter()
    {
        Array values = Enum.GetValues(typeof(CharacterType));
        System.Random random = new System.Random();
        CharacterType randomType = (CharacterType)values.GetValue(random.Next(values.Length));
        return CreateRandomCharacter(randomType);
    }
    public Character CreateRandomCharacter(CharacterType type)
    {
        Character c = new Character(type);
        c.SetRandomStats();
        return c;
    }
}
