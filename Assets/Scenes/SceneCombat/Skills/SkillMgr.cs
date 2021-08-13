using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public string name = "Skill Base";
    public int MAX_FURY = 100;
    public int START_FURY = 50;
    public SkillRank rank = SkillRank.A;

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
    public CombatCharacter GetNearestTarget(CombatCharacter mine, List<CombatCharacter> listTarget)
    {
        CombatCharacter result = null;
        int myRow = mine.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        listTarget.ForEach(delegate (CombatCharacter character)
        {
            int col = character.position.x;
            int row = character.position.y;
            if (
                (result == null)
                || (col < NR_col)
                || (col == NR_col && Math.Abs(myRow - row) < NR_row)
            )
            {
                result = character;
                NR_col = col;
                NR_row = Math.Abs(myRow - row);
            }
        });
        return result;
    }
    public CombatCharacter GetFurthestTarget(CombatCharacter mine, List<CombatCharacter> listTarget)
    {
        CombatCharacter result = null;
        int myRow = mine.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        listTarget.ForEach(delegate (CombatCharacter character)
        {
            int col = character.position.x;
            int row = character.position.y;
            if (
                (result == null)
                || (col > NR_col)
                || (col == NR_col && Math.Abs(myRow - row) < NR_row)
            )
            {
                result = character;
                NR_col = col;
                NR_row = Math.Abs(myRow - row);
            }
        });
        return result;
    }
    public Position GetRandomAvaiablePosition (List<CombatCharacter> teamChar)
    {
        List<Position> p = GetAvaiablePosition(teamChar);
        int index = UnityEngine.Random.Range(1, p.Count);
        return p.ElementAt(index);
    }
    public List<Position> GetAvaiablePosition(List<CombatCharacter> teamChar)
    {
        List<Position> lp = new List<Position>();
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                CombatCharacter character = teamChar.Find(character => character.position.x == x && character.position.y == y);
                if (character == null) lp.Add(new Position(x, y));
            }
        }
        return lp;
    }
    public CombatCharacter GetLowestPercentHealthTarget(List<CombatCharacter> teamChar)
    {
        CombatCharacter result = null;
        float curPercent = 1.0f;
        teamChar.ForEach(character =>
        {
            float percent = character.current_health / character.max_health;
            if (
                (result == null)
                || (percent < curPercent)
            )
            {
                result = character;
                curPercent = percent;
            }
        });
        return result;
    }
    public CombatCharacter GetLowestHealthTarget(List<CombatCharacter> teamChar)
    {
        CombatCharacter result = null;
        float health = 0.0f;
        teamChar.ForEach(character =>
        {
            if (
                (result == null)
                || (character.current_health < health)
            )
            {
                result = character;
                health = character.current_health;
            }
        });
        return result;
    }
    public CombatCharacter GetFurthestFuryMax (List<CombatCharacter> teamChar)
    {
        CombatCharacter result = null;
        int distance = 0;
        teamChar.ForEach(character =>
        {
            if (
                (result == null)
                || (character.max_fury - character.current_fury > distance)
            )
            {
                result = character;
                distance = character.max_fury - character.current_fury;
            }
        });
        return result;
    }
    public CombatCharacter GetBehind (CombatCharacter c, List<CombatCharacter> teamChar)
    {
        CombatCharacter result = null;
        Position p = c.position;
        result = teamChar.Find(cha => (cha.position.x == c.position.x + 1 && cha.position.y == c.position.y));
        return result;
    }
}

