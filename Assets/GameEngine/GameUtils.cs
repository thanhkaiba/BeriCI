using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

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
            case "target":
                characterGO = Instantiate(Resources.Load<GameObject>("characters/Target/target"));
                character = characterGO.AddComponent<Target>() as Target;
                break;
            case "helti":
                characterGO = Instantiate(Resources.Load<GameObject>("characters/Helti/Helti"));
                character = characterGO.AddComponent<Helti>() as Helti;
                break;
            default:
                characterGO = Instantiate(Resources.Load<GameObject>("characters/sword_man"));
                character = characterGO.AddComponent<Sailor>() as Sailor;
                break;
        }
        
        return character;
    }
    public Item CreateItem(string itemId, int quality = 0)
    {
        Item item;
        using (StreamReader r = new StreamReader("Assets/Config/Items/" + itemId + ".json"))
        {
            string json = r.ReadToEnd();
            item = JsonConvert.DeserializeObject<Item>(json);
            item.quality = quality;
        }
        return item;
    }
}
 