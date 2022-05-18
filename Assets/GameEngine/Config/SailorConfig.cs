using Piratera.Config;
using System.Collections.Generic;
using UnityEngine;

public class SailorConfig
{
    public string root_name = "Root";
    public SailorRank rank = SailorRank.S;

    public float power_base = 50;
    public float health_base = 250;
    public float speed_base = 100;
    public float armor = 10;
    public float magic_resist = 10;

    public float crit = 0;

    public AttackType attack_type = AttackType.MELEE;
    public List<SailorClass> classes;

    public int start_fury = 0;
    public int max_fury = 20;
    public List<float> skill_params;
    public float GetPower(int level, int quality, int star)
    {
        float power = power_base;
        power *= 1 + (float)quality / GlobalConfigs.SailorGeneral.MAX_QUALITY * GlobalConfigs.SailorGeneral.MAX_MIN_POWER_RATIO;
        power *= Mathf.Pow(1 + GlobalConfigs.SailorGeneral.POWER_PER_LEVEL_RATIO, level - 1);
        power *= Mathf.Pow(1 + GlobalConfigs.SailorGeneral.STAR_STAT_RATE, star - 1);
        return power;
    }
    public float GetHealth(int level, int quality, int star)
    {
        float health = health_base;
        health *= 1 + (float)quality / GlobalConfigs.SailorGeneral.MAX_QUALITY * GlobalConfigs.SailorGeneral.MAX_MIN_HEALTH_RATIO; 
        health *= Mathf.Pow(1 + GlobalConfigs.SailorGeneral.HEALTH_PER_LEVEL_RATIO, level - 1);
        health *= Mathf.Pow(1 + GlobalConfigs.SailorGeneral.STAR_STAT_RATE, star - 1);
        return health;
    }
    public float GetSpeed(int level, int quality)
    {
        float speed = speed_base;
        speed *= 1 + (float)quality / GlobalConfigs.SailorGeneral.MAX_QUALITY * GlobalConfigs.SailorGeneral.MAX_MIN_SPEED_RATIO;
        return speed;
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
    public bool HaveType(SailorClass type)
    {
        bool found = false;
        classes.ForEach(_t =>
        {
            if (_t == type) found = true;
        });
        return found;
    }
}
