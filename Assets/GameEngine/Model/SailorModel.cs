using System;
using System.Collections.Generic;
using UnityEngine;

public class SailorModel : IEquatable<SailorModel>, IComparable<SailorModel>
{
    public SailorModel(string _id, string _name)
    {
        id = _id;
        name = _name;
        LoadConfig();
    }
    public SailorConfig config_stats { get; set; }
    public readonly string id;
    public readonly string name;
    public int quality { get; set; }
    public int level { get; set; }
    public int exp { get; set; }
    public List<Item> items { get; set; }

    public void LoadConfig()
    {
        config_stats = Resources.Load<SailorConfig>("ScriptableObject/Sailors/" + name);
    }

    public bool HaveType(SailorClass type)
    {
        bool found = false;
        GetListClasses().ForEach(_t =>
        {
            if (_t == type) found = true;
        });
        return found;
    }
    public List<SailorClass> GetListClasses()
    {
        List<SailorClass> result = new List<SailorClass>(config_stats.classes);
        if (items != null)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (item.class_buff != SailorClass.NONE) result.Add(item.class_buff);
            }
        }

        return result;
    }

    bool IEquatable<SailorModel>.Equals(SailorModel other)
    {
        if (other == null)
        {
            return false;
        }

        return name == other.name && quality == other.quality && level == other.level && exp == other.exp;

    }

    int IComparable<SailorModel>.CompareTo(SailorModel other)
    {
        if (config_stats.rank.Equals(other.config_stats.rank))
        {
            if (quality == other.quality)
            {
                if (level == other.level)
                {
                    return name.CompareTo(other.name);
                }
                else
                {
                    return level.CompareTo(other.level);
                }
            }
            else
            {
                return quality.CompareTo(other.quality);
            }
        }
        else
        {
            return config_stats.rank.CompareTo(other.config_stats.rank);
        }
    }
}
