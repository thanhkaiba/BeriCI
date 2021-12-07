using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sailor", menuName = "config/Sailor")]
public class SailorConfig : ScriptableObjectPro
{
    public GameObject model;
    public Sprite avatar;
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
    public float power_base_step
    {
        get { return power_base * GlobalConfigs.SailorGeneral.MAX_MIN_POWER_RATIO / GlobalConfigs.SailorGeneral.MAX_QUALITY; }
    }
    public float power_plv
    {
        get { return power_base * GlobalConfigs.SailorGeneral.POWER_PER_LEVEL_RATIO; }
    }    
    public float power_plv_step
    {
        get { return power_base_step * GlobalConfigs.SailorGeneral.POWER_PER_LEVEL_RATIO; }
    }

    public float health_base_step { 
        get { return health_base * GlobalConfigs.SailorGeneral.MAX_MIN_HEALTH_RATIO / GlobalConfigs.SailorGeneral.MAX_QUALITY; }
    }
    public float health_plv {
        get { return health_base * GlobalConfigs.SailorGeneral.HEALTH_PER_LEVEL_RATIO; }
    }
    public float health_plv_step {
        get { return health_base_step * GlobalConfigs.SailorGeneral.HEALTH_PER_LEVEL_RATIO; }
    }

    public float speed_step {
        get { return speed_base * GlobalConfigs.SailorGeneral.MAX_MIN_SPEED_RATIO / GlobalConfigs.SailorGeneral.MAX_QUALITY; }
    }

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
