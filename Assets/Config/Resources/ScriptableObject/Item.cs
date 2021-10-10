using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemPosition
{
    HAT,
    NECKLACE,
    ARMOR,
    WEAPON,
    BELT,
    SHOES,
}

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObjectPro
{
    public string item_name = "";

    public float rank = 0;
    public ItemPosition pos = ItemPosition.NECKLACE;
    public float power_base = 50;
    public float power_step = 0;

    public float health_base = 0;
    public float health_step = 0;

    public float speed_base = 0;
    public float speed_step = 0;

    public float crit_base = 0;
    public float crit_step = 0;

    public float armor_base = 0;
    public float armor_step = 0;
    public float magic_resist_base = 0;
    public float magic_resist_step = 0;

    public SailorType type_buff = SailorType.BERSERK;

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
