using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combat config", menuName = "config/combat")]
public class CombatConfig : ScriptableObjectPro
{
    public int fury_per_base_attack = 10;
    public int fury_per_take_damage = 2;
}