using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SkillRank
{
    B,
    A,
    S,
    SR,
}

enum TargetType
{
    SELF,
    NEAREST,
    FURTHEST,
    SAME_ROW,
    SAME_COLUMN,
    NEAREST_COLUMN
}
public class Skill
{
    public string skill_name = "Skill Base";
    public int MAX_FURY = 100;
    public int START_FURY = 50;
    public SkillRank rank = SkillRank.A;

    public virtual void UpdateData(SkillConfig config)
    {
        skill_name = config.skillName;
        MAX_FURY = config.maxFury;
        START_FURY = config.startFury;
        rank = config.rank;
    }

    public virtual bool CanActive(CombatSailor cChar, CombatState cbState)
    {
        return true;
    }
    public virtual float CastSkill(CombatSailor cChar, CombatState cbState)
    {
        Debug.Log(">>>>>>>>>>>>>>>" + cChar.Model.config_stats.root_name + " active " + skill_name);
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition(skill_name, cChar.transform.position);
        return 0.5f;
    }
}

