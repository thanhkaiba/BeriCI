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
    public float power_base_step = 0.25f;
    public float power_plv = 1;
    public float power_plv_step = 0.001f;

    public float crit = 0;
    public float crit_damage = 0;

    public float health_base = 250;
    public float health_base_step = 0.5f;
    public float health_plv = 3;
    public float health_plv_step = 0.003f;

    public float speed_base = 100;
    public float speed_step = 0.05f;

    public float armor = 10;
    public float magic_resist = 10;


    public AttackType attack_type = AttackType.MELEE;
    public List<SailorClass> classes;

    public string skill_name = "";
    public int start_fury = 0;
    public int max_fury = 20;
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
