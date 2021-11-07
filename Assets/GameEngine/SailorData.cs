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
    POISON,
    BURN,
    EXCITED,
    BLEEDING,
};

public enum SailorClass
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
}
