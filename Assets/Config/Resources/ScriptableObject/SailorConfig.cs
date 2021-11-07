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

    public float power_base = 0;
    public float power_base_step = 0;
    public float power_plv = 0;
    public float power_plv_step = 0;

    public float crit = 0;
    public float crit_damage = 0;

    public float block_change = 0;

    public float health_base = 0;
    public float health_base_step = 0;
    public float health_plv = 0;
    public float health_plv_step = 0;

    public float speed_base = 0;
    public float speed_step;

    public float armor = 0;
    public float magic_resist = 0;


    public AttackType attack_type = AttackType.MELEE;
    public List<SailorClass> classes;

    public string skill_name = "";
    public int start_fury;
    public int max_fury;
    public List<float> skill_params;

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
    public float GetBlock()
    {
        return block_change;
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
