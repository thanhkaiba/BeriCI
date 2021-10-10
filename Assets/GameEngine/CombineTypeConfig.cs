using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CombineTypeConfig
{
    private class PassiveConfig
    {
        public SailorType type;
        public IEnumerable<Level> levels;
    }
    private class Level
    {
        public int pop;
        public IEnumerable<float> _params;
    }

    public static CombineTypeConfig Instance;
    private IEnumerable<PassiveConfig> config;
    public CombineTypeConfig()
    {
        Instance = this;
        using (StreamReader r = new StreamReader("Assets/Config/PassiveType/PassiveType.json"))
        {
            string json = r.ReadToEnd();
            config = JsonConvert.DeserializeObject< IEnumerable<PassiveConfig> >(json);
        }
    }
    private PassiveConfig GetPassiveConfig(SailorType type)
    {
        foreach (PassiveConfig item in config)
        {
            if (item.type == type) return item;
        }
        return null;
    }
    public bool HaveCombine(SailorType type)
    {
        var config = GetPassiveConfig(type);
        return config != null;
    }
    public List<int> GetMilestones (SailorType type)
    {
        var config = GetPassiveConfig(type);
        List<int> result = new List<int>();
        if (config != null)
        {
            foreach (Level l in config.levels)
            {
                result.Add(l.pop);
            }
        }
        return result;
    }
    public int GetMaxPopNeed(SailorType type)
    {
        var config = GetPassiveConfig(type);
        int result = 0;
        if (config != null)
        {
            foreach (Level l in config.levels)
            {
                if (l.pop > result) result = l.pop;
            }
        }
        return result;
    }
    public List<float> GetParams (SailorType type, int level)
    {
        var config = GetPassiveConfig(type);
        List<float> result = new List<float>();
        if (config != null)
        {
            Level l = config.levels.ElementAt(level);
            if (l != null) result = l._params.ToList();
        }
        return result;
    }
}
