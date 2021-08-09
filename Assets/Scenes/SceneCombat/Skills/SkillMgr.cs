using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum SkillRank
{
    B,
    A,
    S,
    SR,
}
enum TargetTeam
{
    A,
    B,
    BOTH,
    NONE,
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
    public string name = "Skill Base";
    public int MAX_FURY = 100;
    public int START_FURY = 50;

    public virtual float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        Debug.Log(">>>>>>>>>>>>>>>" + cChar.charName + " active " + "TakeRest");
        return 0.5f;
    }
}

