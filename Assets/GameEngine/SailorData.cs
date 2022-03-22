using Piratera.Config;
using System.Collections.Generic;

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
    EXCITED,
    EPIDEMIC,
    SUMMONED,
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
    GUNPOWDER,
    SHIPWRIGHT,
    EPIDEMIC,
    CRIMINAL,
    SUMMONER,
    PIONEER,
    ELF,
    SAMURAI,
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
    public int stack;
    public SailorStatus(SailorStatusType _name, int _stack = 1)
    {
        name = _name;
        stack = _stack;
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
        int star = model.star;
        BasePower = model.config_stats.GetPower(level, quality, star);
        MaxHealth = model.config_stats.GetHealth(level, quality, star);
        CurHealth = model.config_stats.GetHealth(level, quality, star);
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
        get
        {
            float increase = 0;
            var excited = GetStatus(SailorStatusType.EXCITED);
            if (excited != null) increase += excited.stack * GlobalConfigs.SailorStatus.EXCITED * BasePower;
            return BasePower + increase;
        }
    }

    public float MaxHealth;
    public float CurHealth;

    public float BaseArmor;
    public float Armor
    {
        get { return BaseArmor; }
    }

    public float BaseMagicResist;
    public float MagicResist
    {
        get
        {
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
    public SailorStatus GetStatus(SailorStatusType name)
    {
        return listStatus.Find(x => x.name == name);
    }
    public void UpdateStatsWithSynergy(List<ClassBonusItem> ownTeam, List<ClassBonusItem> oppTeam = null)
    {
        SynergiesConfig config = GlobalConfigs.Synergies;
        if (ownTeam != null) ownTeam.ForEach(p =>
        {
            if (p.level < 0) return;
            switch (p.type)
            {
                case SailorClass.MIGHTY:
                    if (HaveType(SailorClass.MIGHTY))
                    {
                        MaxHealth *= 1 + config.GetParams(p.type, p.level)[0];
                        CurHealth = MaxHealth;
                    }
                    break;
                case SailorClass.SWORD_MAN:
                    if (HaveType(SailorClass.SWORD_MAN))
                    {
                        Speed += config.GetParams(p.type, p.level)[0];
                    }
                    break;
                case SailorClass.MAGE:
                    if (HaveType(SailorClass.MAGE))
                    {
                        BasePower *= 1 + config.GetParams(p.type, p.level)[0];
                    }
                    break;
                case SailorClass.ASSASSIN:
                    if (HaveType(SailorClass.ASSASSIN))
                    {
                        CritDamage += config.GetParams(p.type, p.level)[0];
                        Crit += config.GetParams(p.type, p.level)[0];
                    }
                    break;
                case SailorClass.SEA_CREATURE:
                    BaseMagicResist += config.GetParams(p.type, p.level)[0];
                    break;
                case SailorClass.SUPPORT:
                    Fury += (int)config.GetParams(p.type, p.level)[0];
                    if (Fury > MaxFury) Fury = MaxFury;
                    break;
                case SailorClass.KNIGHT:
                    if (HaveType(SailorClass.KNIGHT))
                    {
                        BaseArmor += (int)config.GetParams(p.type, p.level)[0];
                    }
                    break;
                case SailorClass.PIONEER:
                    if (HaveType(SailorClass.PIONEER))
                    {
                        CurrentSpeed += (int)(config.GetParams(p.type, p.level)[0] * SpeedNeed);
                    }
                    break;
                case SailorClass.CYBORG:
                    if (HaveType(SailorClass.CYBORG))
                    {
                        Speed += config.GetParams(p.type, p.level)[1];
                        BasePower += config.GetParams(p.type, p.level)[2];
                    }
                    break;
            }
        });

        if (oppTeam != null) oppTeam.ForEach(p =>
        {
            if (p.level < 0) return;
            switch (p.type)
            {
                case SailorClass.DEMON:
                    BaseArmor -= config.GetParams(p.type, p.level)[0];
                    BaseMagicResist -= config.GetParams(p.type, p.level)[0];
                    break;
            }
        });
    }
}
