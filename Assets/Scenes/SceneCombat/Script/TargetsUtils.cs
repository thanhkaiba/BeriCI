using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using Random = System.Random;

public class TargetsUtils
{
    public static CombatSailor Melee(CombatSailor actor, List<CombatSailor> l)
    {
        CombatSailor result = null;
        int myRow = actor.cs.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        l.ForEach(delegate (CombatSailor character)
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
    public static CombatSailor Range(CombatSailor actor, List<CombatSailor> l)
    {
        CombatSailor result = null;
        int myRow = actor.cs.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        l.ForEach(delegate (CombatSailor character)
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
    public static CombatSailor Backstab(CombatSailor actor, List<CombatSailor> l)
    {
        CombatSailor result = null;
        int myRow = actor.cs.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        l.ForEach(delegate (CombatSailor character)
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
    public static CombatSailor Behind(CombatSailor actor, List<CombatSailor> l)
    {
        CombatSailor result = null;
        CombatPosition p = actor.cs.position;
        result = l.Find(cha => (cha.cs.position.x == actor.cs.position.x + 1 && cha.cs.position.y == actor.cs.position.y));
        return result;
    }
    public static List<CombatSailor> AllBehind(CombatSailor actor, List<CombatSailor> l)
    {
        List<CombatSailor> result = new List<CombatSailor>();
        CombatPosition p = actor.cs.position;
        l.ForEach(cha =>
        {
            if ((cha.cs.position.x < actor.cs.position.x)
                && cha.cs.position.y == actor.cs.position.y) result.Add(cha);
        });
        return result;
    }
    public static List<CombatSailor> Around(CombatSailor actor, List<CombatSailor> l, bool includeActor = false)
    {
        List<CombatSailor> result = new List<CombatSailor>();
        CombatPosition p = actor.cs.position;
        l.ForEach(cha =>
        {
            if (Math.Abs(cha.cs.position.x - actor.cs.position.x) <= 1 && Math.Abs(cha.cs.position.y - actor.cs.position.y) <= 1)
            {
                if (actor != cha || includeActor) result.Add(cha);
            }
        });
        return result;
    }
    public static List<CombatSailor> Random(List<CombatSailor> l, int number = 1)
    {
        if (number > l.Count) number = l.Count;
        var clone = new List<CombatSailor>();
        var result = new List<CombatSailor>();
        l.ForEach(item => clone.Add(item));
        for (int i = 0; i < number; i++)
        {
            int r = UnityEngine.Random.Range(0, clone.Count);
            result.Add(clone[r]);
            clone.RemoveAt(r);
        }
        return result;
    }
    public static CombatSailor LowestPercentHealth(List<CombatSailor> l)
    {
        CombatSailor result = null;
        float curPercent = 1.0f;
        l.ForEach(character =>
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
    public static List<CombatSailor> SameRow(CombatSailor actor, List<CombatSailor> l)
    {
        int row = actor.cs.position.y;
        List<CombatSailor> result = new List<CombatSailor>();
        l.ForEach(delegate (CombatSailor character)
        {
            if (character.cs.position.y == row) result.Add(character);
        });
        return result;
    }
    public static CombatSailor LowestHealth(List<CombatSailor> l)
    {
        CombatSailor result = null;
        float health = 0.0f;
        l.ForEach(character =>
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
    public static CombatSailor FurthestMaxFury(List<CombatSailor> l)
    {
        CombatSailor result = null;
        int distance = 0;
        l.ForEach(character =>
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

    public static List<CombatSailor> NearestColumn(List<CombatSailor> l)
    {
        var result = new List<CombatSailor>();
        short x = 9999;
        l.ForEach(s =>
        {
            if (s.cs.position.x < x) x = s.cs.position.x;
        });
        l.ForEach(s =>
        {
            if (s.cs.position.x == x) result.Add(s);
        });
        return result;
    }

    public static CombatSailor Random(List<CombatSailor> l)
    {
        Random rnd = new Random();
        int idx = rnd.Next(0, l.Count);
        return l[idx];
    }

    public static Team OppositeTeam(Team t)
    {
        return (t == Team.A ? Team.B : Team.A);
    }
}
