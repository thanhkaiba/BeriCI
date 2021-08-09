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

    public virtual bool CanActive(CombatCharacter cChar, CombatState cbState)
    {
        return true;
    }
    public virtual float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        Debug.Log(">>>>>>>>>>>>>>>" + cChar.charName + " active " + name);
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition(name, cChar.transform.position);
        return 0.5f;
    }
    public List<CombatCharacter> GetSameLineTarget(int row, List<CombatCharacter> listChar)
    {
        List<CombatCharacter> result = new List<CombatCharacter>();
        listChar.ForEach(delegate (CombatCharacter character)
        {
            if (character.position.y == row) result.Add(character);
        });
        return result;
    }
}

