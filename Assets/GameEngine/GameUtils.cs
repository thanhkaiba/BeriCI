using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Piratera.GUI;
using Piratera.Lib;

public class GameUtils : UnityEngine.Object
{
    // test only
    public static CombatSailor CreateCombatSailor(string name)
    {
        SailorModel model = new SailorModel(RandomId(), name);
        SailorConfig config_stats = model.config_stats;
        //if (config_stats == null) config_stats = Resources.Load<SailorConfig>("ScriptableObject/Sailors/Target");
        GameObject characterGO = Instantiate(config_stats.model);
        CombatSailor sailor = characterGO.AddComponent(Type.GetType(name)) as CombatSailor;
        sailor.Model = model;

        Debug.Log(">>>>>>>>>>>>>>>>>_id " + model.id);
        //if (config_stats.skillConfig)
        //{
        //    sailor.skill = Activator.CreateInstance(Type.GetType(config_stats.skillConfig.skillName.Replace(" ", string.Empty))) as Skill;
        //    sailor.skill.UpdateData(config_stats.skillConfig);
        //}

        //sailor.gameObject.AddComponent<Billboard>();

        //var shadow = Instantiate(Resources.Load<GameObject>("GameComponents/shadow/shadow"));
        //shadow.GetComponent<CharacterShadow>().SetCharacter(sailor.gameObject);
        //shadow.transform.Find("silhouette_shadow").gameObject.layer = 7;

        return sailor;
    }

    public static GameObject AddSailorImage(string name, Transform parent, out SailorConfig config_stats)
    {
        SailorModel model = new SailorModel(RandomId(), name);
        config_stats = model.config_stats;
        if (config_stats != null && config_stats.model != null)
        {
            GameObject sailor = Instantiate(config_stats.model, parent);
            sailor.transform.localPosition = Vector3.zero;
            return sailor;

        }
        return null;
    }
    public static CombatSailor CreateCombatSailor(SailorModel model)
    {
        SailorConfig config_stats = model.config_stats;
        GameObject characterGO = Instantiate(config_stats.model);
        CombatSailor sailor = characterGO.AddComponent(Type.GetType(model.config_stats.root_name)) as CombatSailor;
        sailor.Model = model;

        return sailor;
    }
    public static Sailor CreateSailor(string id)
    {
        SailorModel sailorModel = CrewData.Instance.GetSailorModel(id);
        if (sailorModel == null)
        {
            Debug.LogError("Sailor id = " + id + " Not found");
        }
        return CreateSailor(sailorModel);
    }

    public static GameObject CreateFakeSailorObject(string name)
    {
        SailorModel model = new SailorModel(RandomId(), name);
        SailorConfig config_stats = model.config_stats;
        return Instantiate(config_stats.model);

    }

    public static Sailor CreateSailor(SailorModel sailorModel)
    {
        if (sailorModel.config_stats.model == null)
        {
            GuiManager.Instance.ShowPopupNotification($"{sailorModel.config_stats.root_name} is not have a model (Chua Code o Client)");
            return null;
        }
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
    public static List<ClassBonusItem> CalculateClassBonus(List<SailorModel> t)
    {
        List<ClassBonusItem> result = new List<ClassBonusItem>();
        List<int> typeCount = new List<int>();
        int assassinCount = 0;
        for (int i = 0; i < Enum.GetNames(typeof(SailorClass)).Length; i++)
        {
            typeCount.Add(0);
        }

        foreach (SailorClass type in (SailorClass[])Enum.GetValues(typeof(SailorClass)))
        {
            List<string> countedSailor = new List<string>();
            t.ForEach(model =>
            {
                string sailorName = model.config_stats.root_name;
                // moi loai sailor chi tinh 1 lan
                if (model.config_stats.HaveType(type) && !countedSailor.Contains(sailorName))
                {
                    typeCount[(int)type] += 1;
                    countedSailor.Add(sailorName);
                }
            });
        }

        t.ForEach(model =>
        {
            // Dem so assassin, neu >= 2 thi khong kich hoat
            if (model.config_stats.HaveType(SailorClass.ASSASSIN)) assassinCount++;
        });

        foreach (SailorClass type in Enum.GetValues(typeof(SailorClass)))
        {
            // nhieu hon 1 assassin thi skip tinh assassin
            if (type == SailorClass.ASSASSIN && assassinCount > 1) continue;

            ContainerClassBonus config = GlobalConfigs.ClassBonus;
            if (!config.HaveBonus(type)) continue;
            List<int> milestones = config.GetMilestones(type);
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
    private static System.Random rd = new System.Random();
    public static string RandomId(int stringLength = 6)
    {
        const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
        char[] chars = new char[stringLength];

        for (int i = 0; i < stringLength; i++)
        {
            chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
        }
        return new string(chars);
    }

    public static void ShowPopupPacketError(SFSErrorCode errorCode)
    {
        string description = EnumHelper.GetDescription(errorCode);
        GuiManager.Instance.ShowPopupNotification(description);
    }
    public static IEnumerator WaitAndDo(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }
}
 