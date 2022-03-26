using Piratera.Config;
using Piratera.GUI;
using Piratera.Lib;
using Piratera.Network;
using Piratera.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils : UnityEngine.Object
{
    public static GameObject GetSailorModelPrefab(string name)
    {
        return Resources.Load<GameObject>("Characters/" + name + "/" + name);
    }
    public static Sprite GetSailorAvt(string name)
    {
        return Resources.Load<Sprite>("Icons/IconSailor/" + name);
    }
    // test only
    public static CombatSailor CreateCombatSailor(string name)
    {
        SailorModel model = new SailorModel(RandomId(), name);
        SailorConfig config_stats = model.config_stats;

        //if (config_stats == null) config_stats = Resources.Load<SailorConfig>("ScriptableObject/Sailors/Target");
        GameObject characterGO = Instantiate(GetSailorModelPrefab(config_stats.root_name));
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
        if (config_stats != null)
        {
            GameObject sailor = Instantiate(GetSailorModelPrefab(config_stats.root_name), parent);
            return sailor;

        }
        return null;
    }
    public static CombatSailor CreateCombatSailor(SailorModel model)
    {
        SailorConfig config_stats = model.config_stats;
        GameObject characterGO = Instantiate(GetSailorModelPrefab(config_stats.root_name));
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
        return Instantiate(GetSailorModelPrefab(config_stats.root_name));

    }

    public static Sailor CreateSailor(SailorModel sailorModel)
    {
        if (GetSailorModelPrefab(sailorModel.config_stats.root_name) == null)
        {
            GuiManager.Instance.ShowPopupNotification($"{sailorModel.config_stats.root_name} is not have a model (Chua Code o Client)");
            return null;
        }
        GameObject characterGO = Instantiate(GetSailorModelPrefab(sailorModel.config_stats.root_name));
        Sailor sailor = characterGO.AddComponent<Sailor>();
        sailor.Model = sailorModel;

        var barPrefab = Resources.Load<GameObject>("characters/Bar/GereralInfoBar");
        var barGO = Instantiate(
            barPrefab,
            sailor.transform.Find("nodeBar"));
        barGO.transform.localScale = new Vector3(0.024f, 0.024f, 1f);
        barGO.transform.localPosition = new Vector3(0, 0, 0);
        barGO.transform.GetComponent<GeneralInfoBar>().PresentData(sailorModel);

        return sailor;
    }
    public static Item CreateItem(string itemId, int quality = 0)
    {
        Item item = Resources.Load<Item>("ScriptableObject/Items/DemoItem");
        item.quality = quality;

        return item;
    }
    public static List<ClassBonusItem> CalculateClassBonus(List<SailorModel> t, bool countDeficient = false)
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

            SynergiesConfig config = GlobalConfigs.Synergies;
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
                if (typeCount[(int)type] > 0 && level == 0 && countDeficient)
                    result.Add(new ClassBonusItem() { type = type, level = -1, current = typeCount[(int)type] });
            }
        }
        result.Sort((a, b) => b.level - a.level);
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
    public static void SetTimeScale(float timescale)
    {
        Time.timeScale = timescale;
        if (timescale > 1)
        {
            SoundMgr.SetSoundFxSpeed(timescale * 0.1f + 1.3f);
            SoundMgr.SetSoundFxVolume(0.5f);
        }
        else
        {
            SoundMgr.SetSoundFxSpeed(timescale);
            SoundMgr.SetSoundFxVolume(1f);
        }
    }
    public static string GetTextDescription(string oriText, SailorModel model, CombatStats stats = null)
    {
        string result = oriText;
        List<int> start = new List<int>();
        List<int> end = new List<int>();
        for (int i = 0; i < oriText.Length; i++)
        {
            if (oriText[i] == '$') start.Add(i);
            if (oriText[i] == '@') end.Add(i);
        }
        for (int j = start.Count - 1; j >= 0; j--)
        {
            string subString = oriText.Substring(start[j], end[j] - start[j] + 1);

            string s = subString[1..^1];
            var split = s.Split("_");
            if (split[0] == "p")
            {
                int skillParamIndex = int.Parse(split[1]);
                float power;
                if (stats != null) power = stats.Power;
                else power = model.config_stats.GetPower(model.level, model.quality, model.star);
                float param = 0;
                if (skillParamIndex >= model.config_stats.skill_params.Count) param = 1;
                else param = model.config_stats.skill_params[skillParamIndex];
                oriText = oriText.Replace(subString, "" + Mathf.Round(power * param));
            }
            else if (split[0] == "per")
            {
                int skillParamIndex = int.Parse(split[1]);
                float param = 0;
                if (skillParamIndex >= model.config_stats.skill_params.Count) param = 1;
                else param = model.config_stats.skill_params[skillParamIndex];
                oriText = oriText.Replace(subString, "" + (param * 100) + "%");
            }
            else if (split[0] == "i")
            {
                int skillParamIndex = int.Parse(split[1]);
                float param = 0;
                if (skillParamIndex >= model.config_stats.skill_params.Count) param = 1;
                else param = model.config_stats.skill_params[skillParamIndex];
                oriText = oriText.Replace(subString, "" + param);
            }
            else if (split[0] == "perT")
            {
                int idx_base = int.Parse(split[1]);
                int idx_sub = int.Parse(split[2]);
                float param0 = model.config_stats.skill_params[idx_base];
                float param1 = model.config_stats.skill_params[idx_sub];
                float total = param0 + param1 * model.star;

                oriText = oriText.Replace(subString, "" + (total * 100) + "%");
            }
            else if (split[0] == "zekeDmg")
            {
                float param0 = model.config_stats.skill_params[0];
                float param1 = model.config_stats.skill_params[1];
                float total = param0 + param1 * model.star;
                oriText = oriText.Replace(subString, "" + (total * 100) + "%");
            }
            else if (split[0] == "kamijitaSpeedUp")
            {
                float baseSpeed = model.config_stats.skill_params[1];
                float speedPerStar = model.config_stats.skill_params[2];
                float total = baseSpeed + speedPerStar * model.star;
                oriText = oriText.Replace(subString, "" + (total * 100) + "%");
            }
            else if (split[0] == "galdalfSpeedUp")
            {
                float baseSpeed = model.config_stats.skill_params[1];
                float speedPerStar = model.config_stats.skill_params[2];
                float total = baseSpeed + speedPerStar * model.star;
                oriText = oriText.Replace(subString, "" + (total * 100) + "%");
            }
            else if (split[0] == "jenkinDmg")
            {
                float baseDmg = model.config_stats.skill_params[1];
                float dmgPerStar = model.config_stats.skill_params[2];
                Debug.Log("baseDmg: " + baseDmg);
                Debug.Log("dmgPerStar: " + dmgPerStar);
                Debug.Log("dmgPerStar * model.star: " + dmgPerStar * model.star);
                float total = baseDmg + dmgPerStar * model.star;
                oriText = oriText.Replace(subString, "" + (total * 100) + "%");
            }
        }
        return oriText;
    }
    public static List<ClassBonusItem> lineUpSynergy = null;
    public static List<ClassBonusItem> oppLineUpSynergy = null;
}
