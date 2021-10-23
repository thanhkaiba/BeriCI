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
        SailorModel model = new SailorModel("id_fake", name);
        SailorConfig config_stats = model.config_stats;
        //if (config_stats == null) config_stats = Resources.Load<SailorConfig>("ScriptableObject/Sailors/Target");
        GameObject characterGO = Instantiate(config_stats.model);
        CombatSailor sailor = characterGO.AddComponent(Type.GetType(name)) as CombatSailor;
        sailor.Model = model;
        if (config_stats.skillConfig)
        {
            sailor.skill = Activator.CreateInstance(Type.GetType(config_stats.skillConfig.skillName.Replace(" ", string.Empty))) as Skill;
            sailor.skill.UpdateData(config_stats.skillConfig);
        }

        sailor.gameObject.AddComponent<Billboard>();

        var shadow = Instantiate(Resources.Load<GameObject>("GameComponents/shadow/shadow"));
        shadow.GetComponent<CharacterShadow>().SetCharacter(sailor.gameObject);
        shadow.transform.Find("silhouette_shadow").gameObject.layer = 7;

        return sailor;
    }
    public static Sailor CreateSailor(string id)
    {
        SailorModel sailorModel = SquadData.Instance.GetSailorModel(id);
        if (sailorModel == null)
        {
            Debug.LogError("Sailor id = " + id + " not found");
        }
        return CreateSailor(sailorModel);
    }

    public static Sailor CreateSailor(SailorModel sailorModel)
    {

        GameObject characterGO = Instantiate(sailorModel.config_stats.model);
        Sailor sailor = characterGO.AddComponent<Sailor>();
        sailor.Model = sailorModel;
        sailor.gameObject.AddComponent<Billboard>();
        return sailor;

    }
    public static Item CreateItem(string itemId, int quality = 0)
    {
        Item item = Resources.Load<Item>("ScriptableObject/Items/DemoItem");
        item.quality = quality;

        return item;
    }
    public static List<ClassBonusItem> CalculateClassBonus(List<SailorModel> sailorModels)
    {
        List<ClassBonusItem> result = new List<ClassBonusItem>();

        List<int> typeCount = new List<int>();
        for (int i = 0; i < Enum.GetNames(typeof(SailorClass)).Length; i++)
        {
            typeCount.Add(0);
        }
        sailorModels.ForEach(model =>
        {
            foreach (SailorClass type in (SailorClass[])Enum.GetValues(typeof(SailorClass)))
            {
                if (model.HaveType(type)) typeCount[(int)type] += 1;
            }
        });

        foreach (SailorClass type in Enum.GetValues(typeof(SailorClass)))
        {
            ContainerClassBonus config = GlobalConfigs.ClassBonus;
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
 