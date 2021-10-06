using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum ItemRank
{
    NONE,
    B,
    A,
    S,
    SR,
    SSR,
}

public enum ItemPosition
{
    HEAD,
    NECKLACE,
    BODY,
    HANDS,
    LEGS,
    BELT,
}

public class Item
{
    public string id;
    public string name;
    public ItemRank rank;
    public ItemPosition pos;

    public float power_base;
    public float health_base;
    public float speed_base;
    public float crit_base;
    public float armor_base;
    public float magic_resist_base;

    public SailorType type_buff;

    public float power_step;
    public float health_step;
    public float speed_step;
    public float crit_step;
    public float armor_step;
    public float magic_resist_step;

    public int quality = 0;

    public float Power
    {
        get { return power_base + quality * power_step; }
    }
    public float Health
    {
        get { return health_base + quality * health_step; }
    }
    public float Speed
    {
        get { return speed_base + quality * speed_step; }
    }
    public float Crit
    {
        get { return crit_base + quality * crit_step; }
    }
    public float Armor
    {
        get { return armor_base + quality * armor_step; }
    }
    public float MagicResist
    {
        get { return magic_resist_base + quality * magic_resist_step; }
    }
}
