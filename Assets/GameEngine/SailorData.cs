using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    MELEE,
    RANGE,
    ASSASSINATE,
};

public enum SailorStatusType
{
    DEATH,
    FROZEN,
    POISON,
    BURN,
};

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

    public float health_base;
    public float health_base_step;
    public float health_plv;
    public float health_plv_step;

    public float speed_base;
    public float speed_step;

    public float armor;
    public float magic_resist;

    public AttackType attack_type;
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
    public float base_power;
    public float current_power;

    public float max_health;
    public float current_health;

    public float base_armor;
    public float current_armor;

    public float base_magic_resist;
    public float current_magic_resist;

    public float display_speed;
    public int max_speed;
    public int current_speed;

    public int max_fury;
    public int current_fury;
    public int current_max_fury = 0;

    public CombatPosition position;
    public List<SailorStatus> listStatus = new List<SailorStatus>();
    public Team team;
}
