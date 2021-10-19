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
    public SailorType type;
    public int level;
    public int current = 0;
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
