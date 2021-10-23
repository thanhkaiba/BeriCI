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
    public List<CombatSailor> GetSameLineTarget(int row, List<CombatSailor> listChar)
    {
        List<CombatSailor> result = new List<CombatSailor>();
        listChar.ForEach(delegate (CombatSailor character)
        {
            if (character.cs.position.y == row) result.Add(character);
        });
        return result;
    }
    public CombatSailor GetRangeAttackTarget(CombatSailor mine, List<CombatSailor> listTarget)
    {
        CombatSailor result = null;
        int myRow = mine.cs.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        listTarget.ForEach(delegate (CombatSailor character)
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
    public CombatSailor GetMeleeTarget(CombatSailor mine, List<CombatSailor> listTarget)
    {
        CombatSailor result = null;
        int myRow = mine.cs.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        listTarget.ForEach(delegate (CombatSailor character)
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
    public CombatSailor GetFurthestTarget(CombatSailor mine, List<CombatSailor> listTarget)
    {
        CombatSailor result = null;
        int myRow = mine.cs.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        listTarget.ForEach(delegate (CombatSailor character)
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
    public CombatPosition GetRandomAvaiablePosition (List<CombatSailor> teamChar)
    {
        List<CombatPosition> p = GetAvaiablePosition(teamChar);
        int index = UnityEngine.Random.Range(1, p.Count);
        return p.ElementAt(index);
    }
    public List<CombatPosition> GetAvaiablePosition(List<CombatSailor> teamChar)
    {
        List<CombatPosition> lp = new List<CombatPosition>();
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                CombatSailor character = teamChar.Find(character => character.cs.position.x == x && character.cs.position.y == y);
                if (character == null) lp.Add(new CombatPosition(x, y));
            }
        }
        return lp;
    }
    public CombatSailor GetLowestPercentHealthTarget(List<CombatSailor> teamChar)
    {
        CombatSailor result = null;
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
    public CombatSailor GetLowestHealthTarget(List<CombatSailor> teamChar)
    {
        CombatSailor result = null;
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
    public CombatSailor GetFurthestFuryMax (List<CombatSailor> teamChar)
    {
        CombatSailor result = null;
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
    public CombatSailor GetBehind (CombatSailor c, List<CombatSailor> teamChar)
    {
        CombatSailor result = null;
        CombatPosition p = c.cs.position;
        result = teamChar.Find(cha => (cha.cs.position.x == c.cs.position.x + 1 && cha.cs.position.y == c.cs.position.y));
        return result;
    }
    public List<CombatSailor> GetAllBehind(CombatSailor c, List<CombatSailor> teamChar)
    {
        List<CombatSailor> result = new List<CombatSailor>();
        CombatPosition p = c.cs.position;
        teamChar.ForEach(cha =>
        {
            if ((cha.cs.position.x == c.cs.position.x + 1 || cha.cs.position.x == c.cs.position.x + 2)
                && cha.cs.position.y == c.cs.position.y) result.Add(cha);
        });
        return result;
    }
    public List<CombatSailor> GetAllAround(CombatSailor c, List<CombatSailor> teamChar)
    {
        List<CombatSailor> result = new List<CombatSailor>();
        CombatPosition p = c.cs.position;
        teamChar.ForEach(cha =>
        {
            if ( Math.Abs(cha.cs.position.x - c.cs.position.x) <= 1 && Math.Abs(cha.cs.position.y - c.cs.position.y) <= 1 && c != cha) result.Add(cha);
        });
        return result;
    }
    public CombatSailor GetNearestInRowTarget(CombatSailor c, List<CombatSailor> listTarget)
    {
        CombatSailor result = null;
        int myRow = c.cs.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        listTarget.ForEach(delegate (CombatSailor character)
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
    public List<CombatSailor> GetRandomTarget(List<CombatSailor> listTarget, int number = 1)
    {
        if (number > listTarget.Count) number = listTarget.Count;
        var clone = new List<CombatSailor>();
        var result = new List<CombatSailor>();
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

