using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sailor", menuName = "Sailor")]
public class SailorConfig : ScriptableObjectPro
{
    public string root_name = "Root";

    public float power_base = 0;
    public float power_base_step = 0;
    public float power_plv = 0;
    public float power_plv_step = 0;

    public float crit = 0;

    public float health_base = 0;
    public float health_base_step = 0;
    public float health_plv = 0;
    public float health_plv_step = 0;

    public float speed_base = 0;
    public float speed_step;

    public float armor = 0;
    public float magic_resist = 0;

    public AttackType attack_type = AttackType.MELEE;
    public SailorType[] types = new SailorType[0];

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
