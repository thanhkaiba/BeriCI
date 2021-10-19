using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class GameUtils : UnityEngine.Object
{
    public static CombatSailor CreateCombatSailor(string name)
    {
        SailorConfig config_stats = Resources.Load<SailorConfig>("ScriptableObject/Sailors/" + name);
        if (config_stats == null) config_stats = Resources.Load<SailorConfig>("ScriptableObject/Sailors/Target");
        GameObject characterGO = Instantiate(config_stats.model);
        CombatSailor sailor = characterGO.AddComponent(Type.GetType(name)) as CombatSailor;
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
    public static Sailor CreateSailor(string name)
    {
        SailorConfig config_stats = Resources.Load<SailorConfig>("ScriptableObject/Sailors/" + name);
        if (config_stats == null) config_stats = Resources.Load<SailorConfig>("ScriptableObject/Sailors/Target");
        GameObject characterGO = Instantiate(config_stats.model);
        Sailor sailor = characterGO.AddComponent<Sailor>() as Sailor;
        sailor.config_stats = config_stats;

        sailor.gameObject.AddComponent<Billboard>();
        return sailor;
    }
    public static Item CreateItem(string itemId, int quality = 0)
    {
        Item item = Resources.Load<Item>("ScriptableObject/Items/DemoItem");
        item.quality = quality;

        return item;
    }
    public static List<ClassBonusItem> CalculateClassBonus(List<CombatSailor> t)
    {
        List<ClassBonusItem> result = new List<ClassBonusItem>();

        List<int> typeCount = new List<int>();
        for (int i = 0; i < Enum.GetNames(typeof(SailorType)).Length; i++)
        {
            typeCount.Add(0);
        }
        t.ForEach(sailor =>
        {
            foreach (SailorType type in (SailorType[])Enum.GetValues(typeof(SailorType)))
            {
                if (sailor.cs.HaveType(type)) typeCount[(int)type] += 1;
            }
        });

        foreach (SailorType type in Enum.GetValues(typeof(SailorType)))
        {
            ContainerClassBonus config = GlobalConfigs.Instance.ClassBonus;
            if (!config.HaveBonus(type)) continue;
            var milestones = config.GetMilestones(type);
            for (int level = milestones.Count - 1; level >= 0; level--)
            {
                int popNeed = milestones[level];
                if (typeCount[(int)type] >= popNeed)
                {
                    result.Add(new ClassBonusItem() { type = type, level = level, current = typeCount[(int)type] });
                    break;
                }
            }
        }
        return result;
    }
}
 