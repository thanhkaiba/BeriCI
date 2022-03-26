using Piratera.Config;
using Sfs2X.Entities.Data;
using System;
using System.Collections.Generic;
using Piratera.Constance;

public class SailorModel : IEquatable<SailorModel>, IComparable<SailorModel>
{
    public SailorModel(string _id, string _name)
    {
        id = _id;
        name = _name;
        config_stats = GlobalConfigs.GetSailorConfig(_name);
    }

    public SailorModel(ISFSObject obj) : this(obj.GetUtfString("id"), obj.GetUtfString("name"))
    {
        quality = obj.GetInt("quality");
        level = obj.GetInt("level");
        exp = obj.GetLong("exp");
        lastTrade = obj.GetLong("last_trade") * 1000;
        star = obj.GetByte("star");
        pve_count = obj.GetInt("pve_count");
    }

    public SailorConfig config_stats { get; set; }
    public readonly string id;
    public readonly string name;
    public int quality { get; set; }
    public int level { get; set; }
    public long exp { get; set; }
    public byte star { get; set; }
    public int pve_count { get; set; }

    public long lastTrade { get; set; }
    public List<Item> items { get; set; }

    public bool HaveType(SailorClass type)
    {
        bool found = false;
        GetListClasses().ForEach(_t =>
        {
            if (_t == type) found = true;
        });
        return found;
    }

    public bool IsTrial()
    {
        return id.StartsWith("trial-");
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

    public bool IsAvaiable()
    {
        return lastTrade < GameTimeMgr.GetCurrentUTCTime() - GameConst.TIME_LOCK_SAILOR;
    }

    public long GetRemainingLockTime()
    {
        return lastTrade + GameConst.TIME_LOCK_SAILOR - GameTimeMgr.GetCurrentUTCTime();
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
        //if (IsTrial()) return 1;
        if (config_stats.rank.Equals(other.config_stats.rank))
        {
            if (star == other.star)
            {
                if (level == other.level)
                {
                    if (name.Equals(other.name))
                    {
                        return quality.CompareTo(other.quality);
                    }
                    else
                    {
                        return name.CompareTo(other.name);
                    }
                }
                else
                {
                    return level.CompareTo(other.level);
                }
            }
            else
            {
                return star.CompareTo(other.star);
            }
        }
        else
        {
            return config_stats.rank.CompareTo(other.config_stats.rank);
        }
    }
}
