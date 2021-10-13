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
        SailorConfig config_stats = Resources.Load<SailorConfig>("ScriptableObject/Sailors/" + name);
        if (config_stats == null) config_stats = Resources.Load<SailorConfig>("ScriptableObject/Sailors/Target");
        GameObject characterGO = Instantiate(config_stats.model);
        Sailor sailor = characterGO.AddComponent(Type.GetType(name)) as Sailor;
        sailor.config_stats = config_stats;
        if (config_stats.skillConfig)
        {
            sailor.skill = Activator.CreateInstance(Type.GetType(config_stats.skillConfig.skillName.Replace(" ", string.Empty))) as Skill;
            sailor.skill.UpdateData(config_stats.skillConfig);
        }

        sailor.gameObject.AddComponent<Billboard>();

        var shadow = Instantiate(Resources.Load<GameObject>("GameComponents/shadow/shadow"));
        shadow.GetComponent<CharacterShadow>().SetCharacter(sailor.gameObject);

        return sailor;
    }
    public Item CreateItem(string itemId, int quality = 0)
    {
        Debug.Log("ScriptableObject/Items/" + itemId);
        Item item = Resources.Load<Item>("ScriptableObject/Items/DemoItem");
        item.quality = quality;

        return item;
    }
}
 