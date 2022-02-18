using System.Collections.Generic;

public class SynergiesConfig
{
    public List<Synergies> classBonusList = new List<Synergies>();
    public Synergies GetPassiveConfig(SailorClass type)
    {
        foreach (Synergies item in classBonusList)
        {
            if (item.type == type) return item;
        }
        return null;
    }
    public bool HaveBonus(SailorClass type)
    {
        var config = GetPassiveConfig(type);
        return config != null;
    }
    public List<int> GetMilestones(SailorClass type)
    {
        var config = GetPassiveConfig(type);
        List<int> result = new List<int>();
        if (config != null)
        {
            foreach (SynergiesLevel l in config.levels)
            {
                result.Add(l.pop);
            }
        }
        return result;
    }
    public int GetMaxPopNeed(SailorClass type)
    {
        var config = GetPassiveConfig(type);
        int result = 0;
        if (config != null)
        {
            foreach (SynergiesLevel l in config.levels)
            {
                if (l.pop > result) result = l.pop;
            }
        }
        return result;
    }
    public int GetNextLevelPopNeed(SailorClass type, int level)
    {
        var config = GetPassiveConfig(type);
        if (config.levels.Count <= level + 1) return GetMaxPopNeed(type);
        else return config.levels[level + 1].pop;
    }
    public List<int> GetListPop(SailorClass type)
    {
        List<int> result = new List<int>();
        var config = GetPassiveConfig(type);
        if (config != null)
        {
            foreach (SynergiesLevel l in config.levels)
            {
                result.Add(l.pop);
            }
        }
        return result;
    }
    public int GetLevelCount(SailorClass type)
    {
        var config = GetPassiveConfig(type);
        return config.levels.Count;
    }
    public List<float> GetParams(SailorClass type, int level)
    {
        var config = GetPassiveConfig(type);
        List<float> result = new List<float>();
        if (config != null)
        {
            SynergiesLevel l = config.levels[level];
            if (l != null) result = l._params;
        }
        return result;
    }
}

public class Synergies
{
    public SailorClass type;
    public List<SynergiesLevel> levels;
}

public class SynergiesLevel
{
    public int pop;
    public List<float> _params;
}

