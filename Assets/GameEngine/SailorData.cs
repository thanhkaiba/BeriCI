using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    MELEE,
    RANGE,
    BACKSTAB,
};

public enum SailorStatusType
{
    DEATH,
    STUN,
    FROZEN,
    POISON,
    BURN,
    EXCITED,
    BLEEDING,
};

public enum SailorClass
{
    NONE,
    MIGHTY,
    SWORD_MAN,
    MARKSMAN,
    MAGE,
    DEMON,
    ASSASSIN,
    WILD,
    SEA_CREATURE,
    CYBORG,
    SUPPORT,
    KNIGHT,
    GRAPPLER,
};

public enum SailorRank
{
    B,
    A,
    S,
    SR,
    SSR,
}

public class ClassBonusItem
{
    public SailorClass type;
    public int level;
    public int current = 0;
}

public class SailorStatus
{
    public SailorStatusType name;
    public int remainTurn;
    public List<float> _params;
    public SailorStatus(SailorStatusType _name, int turn = 1, List<float> _params = null)
    {
        name = _name;
        remainTurn = turn;
        this._params = _params;
    }
};

public class CombatPosition
{
    public short x;
    public short y;
    public CombatPosition(short x, short y)
    {
        this.x = x;
        this.y = y;
    }
    public CombatPosition(int x, int y)
    {
        this.x = (short)x;
        this.y = (short)y;
    }
}

public class CombatStats
{
    public CombatStats(SailorModel model)
    {
        int level = model.level;
        int quality = model.quality;
        BasePower = model.config_stats.GetPower(level, quality);
        MaxHealth = model.config_stats.GetHealth(level, quality);
        CurHealth = model.config_stats.GetHealth(level, quality);
        BaseArmor = model.config_stats.GetArmor();
        BaseMagicResist = model.config_stats.GetMagicResist();
        Speed = model.config_stats.GetSpeed(level, quality);
        CurrentSpeed = 0;
        Crit = model.config_stats.GetCrit();
        CritDamage = GlobalConfigs.Combat.base_crit_damage;
        BaseFury = model.config_stats.max_fury;
        Fury = model.config_stats.start_fury;

        foreach (SailorClass type in model.config_stats.classes)
        {
            types.Add(type);
        }

        if (model.items != null) model.items.ForEach(item =>
        {
            BasePower += item.Power;
            MaxHealth += item.Health;
            BaseArmor += item.Armor;
            BaseMagicResist += item.MagicResist;
            Speed += item.Speed;
            Crit += item.Crit;
            Fury += item.Fury;
            if (item.class_buff != SailorClass.NONE) types.Add(item.class_buff);
        });
    }
    public float BasePower;
    public float Power
    {
        get { return BasePower; }
    }

    public float MaxHealth;
    public float CurHealth;

    public float BaseArmor;
    public float Armor {
        get { return BaseArmor; }
    }

    public float BaseMagicResist;
    public float MagicResist {
        get {
            // tinh toan dua tren status
            return BaseMagicResist;
        }
    }

    public float Speed;
    public int SpeedNeed
    {
        get { return (int)(1000000f / Speed); }
    }
    public int CurrentSpeed;

    public float Crit;

    public float CritDamage;

    public float Block;

    public int BaseFury;
    public int MaxFury
    {
        get { return BaseFury; }
    }
    public int Fury;

    public List<SailorClass> types = new List<SailorClass>();

    public CombatPosition position;
    public List<SailorStatus> listStatus = new List<SailorStatus>();
    public Team team;

    public bool HaveType(SailorClass type)
    {
        bool found = false;
        types.ForEach(_t =>
        {
            if (_t == type) found = true;
        });
        //Debug.Log("HaveType: " + found);
        return found;
    }
    public float GetCurrentHealthRatio()
    {
        return CurHealth / MaxHealth;
    }
}
