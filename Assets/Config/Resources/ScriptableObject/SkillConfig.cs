using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "config/Skill")]
public class SkillConfig : ScriptableObjectPro
{
    public string skillName = "Wind Slash";

    public int maxFury;
    public int startFury;

    public List<float> _params;
}
