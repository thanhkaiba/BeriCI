using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    MELEE,
    RANGE,
    SNEAK,
};

public enum SailorStatusType
{
    DEATH,
    FROZEN,
    POISON,
    BURN,
};

public enum SailorType
{
    NONE,
    WILD,
    MIGHTY,
    SWORD_MAN,
    HORROR,
    BERSERK,
    DOCTOR,
    WIZARD,
    SUPPORT,
    SNIPER,
    ASSASSIN,
    KUNG_FU,
};

public class PassiveType
{
    public SailorType type;
    public int level;
}

public class SailorStatus
{
    public SailorStatusType name;
    public int remainTurn;
    public SailorStatus(SailorStatusType _name, int turn = 1)
    {
        name = _name;
        remainTurn = turn;
    }
};

public class CombatPosition
{
    public int x;
    public int y;
    public CombatPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class ConfigStats
{
    public string root_name;

    public float power_base;
    public float power_base_step;
    public float power_plv;
    public float power_plv_step;

    public float crit;

    public float health_base;
    public float health_base_step;
    public float health_plv;
    public float health_plv_step;

    public float speed_base;
    public float speed_step;

    public float armor;
    public float magic_resist;

    public AttackType attack_type;

    public IEnumerable<SailorType> types;
    public float GetPower(int level, int quality)
    {
        return power_base + quality * power_base_step + (power_plv + quality * power_plv_step) * level;
    }
    public float GetHealth(int level, int quality)
    {
        return health_base + quality * health_base_step + (health_plv + quality * health_plv_step) * level;
    }
    public float GetSpeed(int level, int quality)
    {
        return speed_base + speed_step * quality;
    }
    public float GetCrit()
    {
        return crit;
    }
    public float GetArmor()
    {
        return armor;
    }
    public float GetMagicResist()
    {
        return magic_resist;
    }
}

public class CombatStats
{
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

    public float DisplaySpeed;
    public int MaxSpeed
    {
        get { return (int)(10000f / DisplaySpeed); }
    }
    public int CurrentSpeed;

    public float Crit;

    public int BaseFury;
    public int MaxFury
    {
        get { return BaseFury; }
    }
    public int Fury;

    public List<SailorType> types = new List<SailorType>();

    public CombatPosition position;
    public List<SailorStatus> listStatus = new List<SailorStatus>();
    public Team team;

    public bool HaveType(SailorType type)
    {
        bool found = false;
        types.ForEach(_t =>
        {
            if (_t == type) found = true;
        });
        //Debug.Log("HaveType: " + found);
        return found;
    }
}
