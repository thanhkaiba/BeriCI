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
    public string name = "Skill Base";
    public int MAX_FURY = 100;
    public int START_FURY = 50;
    public SkillRank rank = SkillRank.A;

    public virtual bool CanActive(Sailor cChar, CombatState cbState)
    {
        return true;
    }
    public virtual float CastSkill(Sailor cChar, CombatState cbState)
    {
        Debug.Log(">>>>>>>>>>>>>>>" + cChar.charName + " active " + name);
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition(name, cChar.transform.position);
        return 0.5f;
    }
    public List<Sailor> GetSameLineTarget(int row, List<Sailor> listChar)
    {
        List<Sailor> result = new List<Sailor>();
        listChar.ForEach(delegate (Sailor character)
        {
            if (character.cs.position.y == row) result.Add(character);
        });
        return result;
    }
    public Sailor GetNearestTarget(Sailor mine, List<Sailor> listTarget)
    {
        Sailor result = null;
        int myRow = mine.cs.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        listTarget.ForEach(delegate (Sailor character)
        {
            int col = character.cs.position.x;
            int row = character.cs.position.y;
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
    public Sailor GetFurthestTarget(Sailor mine, List<Sailor> listTarget)
    {
        Sailor result = null;
        int myRow = mine.cs.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        listTarget.ForEach(delegate (Sailor character)
        {
            int col = character.cs.position.x;
            int row = character.cs.position.y;
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
    public CombatPosition GetRandomAvaiablePosition (List<Sailor> teamChar)
    {
        List<CombatPosition> p = GetAvaiablePosition(teamChar);
        int index = UnityEngine.Random.Range(1, p.Count);
        return p.ElementAt(index);
    }
    public List<CombatPosition> GetAvaiablePosition(List<Sailor> teamChar)
    {
        List<CombatPosition> lp = new List<CombatPosition>();
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Sailor character = teamChar.Find(character => character.cs.position.x == x && character.cs.position.y == y);
                if (character == null) lp.Add(new CombatPosition(x, y));
            }
        }
        return lp;
    }
    public Sailor GetLowestPercentHealthTarget(List<Sailor> teamChar)
    {
        Sailor result = null;
        float curPercent = 1.0f;
        teamChar.ForEach(character =>
        {
            float percent = character.cs.CurHealth / character.cs.MaxHealth;
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
    public Sailor GetLowestHealthTarget(List<Sailor> teamChar)
    {
        Sailor result = null;
        float health = 0.0f;
        teamChar.ForEach(character =>
        {
            if (
                (result == null)
                || (character.cs.CurHealth < health)
            )
            {
                result = character;
                health = character.cs.CurHealth;
            }
        });
        return result;
    }
    public Sailor GetFurthestFuryMax (List<Sailor> teamChar)
    {
        Sailor result = null;
        int distance = 0;
        teamChar.ForEach(character =>
        {
            if (
                (result == null)
                || (character.cs.MaxFury - character.cs.Fury > distance)
            )
            {
                result = character;
                distance = character.cs.MaxFury - character.cs.MaxFury;
            }
        });
        return result;
    }
    public Sailor GetBehind (Sailor c, List<Sailor> teamChar)
    {
        Sailor result = null;
        CombatPosition p = c.cs.position;
        result = teamChar.Find(cha => (cha.cs.position.x == c.cs.position.x + 1 && cha.cs.position.y == c.cs.position.y));
        return result;
    }
    public Sailor GetNearestInRowTarget(Sailor c, List<Sailor> listTarget)
    {
        Sailor result = null;
        int myRow = c.cs.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        listTarget.ForEach(delegate (Sailor character)
        {
            int col = character.cs.position.x;
            int row = character.cs.position.y;
            if (
                (result == null)
                || (Math.Abs(myRow - row) < NR_row)
                || (Math.Abs(myRow - row) == NR_row && col < NR_col)
            )
            {
                result = character;
                NR_col = col;
                NR_row = Math.Abs(myRow - row);
            }
        });
        return result;
    }
    public List<Sailor> GetRandomTarget(List<Sailor> listTarget, int number = 1)
    {
        if (number > listTarget.Count) number = listTarget.Count;
        var clone = new List<Sailor>();
        var result = new List<Sailor>();
        listTarget.ForEach(item => clone.Add(item));
        for (int i = 0; i < number; i++)
        {
            int r = Random.Range(0, clone.Count);
            result.Add(clone[r]);
            clone.RemoveAt(r);
        }
        return result;
    }
    public Team GetOppositeTeam(Team t)
    {
        return (t == Team.A ? Team.B : Team.A);
    }
}

