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

    public float GetPower ()
    {
        return power_base + power_base_step;
    }
}
