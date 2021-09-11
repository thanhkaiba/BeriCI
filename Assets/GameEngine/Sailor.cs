using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    MELEE,
    RANGE,
    ASSASSINATE,
};

public class ConfigStats
{
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
}

public class Sailor
{
    public string name = "A Sailor";
    public AttackType attackType;
    public int quality = 0;

    public ConfigStats s;

    public int level = 1;
    public Skill skill;

    public string model_name = "sword_man";
    public Sailor()
    {

    }
    public float GetPower()
    {
        return s.power_base + quality * s.power_base_step + (s.power_plv + quality * s.power_plv_step) * level;
    }
    public float GetHealth()
    {
        return s.health_base + quality * s.health_base_step + (s.health_plv + quality * s.health_plv_step) * level;
    }
    public float GetSpeed()
    {
        return s.speed_base + s.speed_step * quality;
    }
    public float GetArmor()
    {
        return s.armor;
    }
    public float GetMagicResist()
    {
        return s.magic_resist;
    }
}
