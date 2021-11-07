using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CombatStatus
{
    PREPARING,
    STARTED,
    ENDED
};

public class CombatState : MonoBehaviour
{
    public static CombatState Instance;

    public CombatStatus status;
    public List<CombatSailor> sailorsTeamA = new List<CombatSailor>();
    public List<CombatSailor> sailorsTeamB = new List<CombatSailor>();
    public Team lastTeamAction;

    public List<ClassBonusItem> ClassBonusA = new List<ClassBonusItem>();
    public List<ClassBonusItem> classBonusB = new List<ClassBonusItem>();
    private void Awake()
    {
        Instance = this;
    }
    public CombatState()
    {
        status = CombatStatus.PREPARING;
        lastTeamAction = Team.B;
    }
    public void CreateTeamFromServer()
    {
        var listSailor = TempCombatData.Instance.listSailor;
        var fgl0 = TempCombatData.Instance.fgl0;
        var fgl1 = TempCombatData.Instance.fgl1;

        for (short x = 0; x < 3; x++)
        {
            for (short y = 0; y < 3; y++)
            {
                {
                    string sailorID = fgl0.SailorIdAt(x, y);
                    if (sailorID != "")
                    {
                        SailorModel sailor = listSailor.Find(sailor => sailor.id == sailorID);
                        CreateCombatSailor(sailor, new CombatPosition(x, y), Team.A);
                    }
                }
                {
                    string sailorID = fgl1.SailorIdAt(x, y);
                    if (sailorID != "")
                    {
                        SailorModel sailor = listSailor.Find(sailor => sailor.id == sailorID);
                        CreateCombatSailor(sailor, new CombatPosition(x, y), Team.B);
                    }
                }
            }
        }
    }
    public void CreateDemoTeam()
    {
        CreateRandomTeam(Team.A);
        CreateTargetTeam(Team.B);
        //CreateCombatSailor("Helti", new CombatPosition(0, 0), Team.A);
        //CreateCombatSailor("Helti", new CombatPosition(0, 2), Team.A);
        //CreateCombatSailor("Helti", new CombatPosition(1, 1), Team.A);
        //CreateCombatSailor("Helti", new CombatPosition(2, 0), Team.A);
        //CreateCombatSailor("Helti", new CombatPosition(2, 2), Team.A);

        //for (short x = 0; x < 3; x++)
        //{
        //    for (short y = 0; y < 3; y++)
        //    {
        //        SailorModel sA = SquadData.Instance.SailorAt(new CombatPosition(x, y));
        //        if (sA != null)
        //        {
        //            CreateCombatSailor(sA, new CombatPosition(x, y), Team.A);
        //        }

        //        SailorModel sB = SquadBData.Instance.SailorAt(new CombatPosition(x, y));
        //        if (sB != null)
        //        {
        //            CreateCombatSailor(sB, new CombatPosition(x, y), Team.B);
        //        }
        //    }
        //}

        //CreateCombatSailor("Meechic", new CombatPosition(0, 0), Team.B);
        //CreateCombatSailor("Meechic", new CombatPosition(0, 2), Team.B);
        //CreateCombatSailor("Meechic", new CombatPosition(2, 1), Team.B);
        //CreateCombatSailor("Meechic", new CombatPosition(0, 1), Team.B);
        //CreateCombatSailor("Meechic", new CombatPosition(1, 1), Team.B);
    }
    void CreateRandomTeam(Team t)
    {
        //CreateCombatSailor("Helti", new CombatPosition(0, 0), t);
        //CreateCombatSailor("Helti", new CombatPosition(0, 2), t);
        //CreateCombatSailor("Target", new CombatPosition(1, 1), t);
        CreateCombatSailor("Meechic|DemoItem:20", new CombatPosition(2, 1), t);

        //CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(0, 1), t);
        //CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(1, 1), t);
        //CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(2, 1), t);
        //CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(0, 0), t);
        //CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(2, 0), t);
        //CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(0, 2), t);
        //CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(2, 2), t);
        //CreateCombatSailor("Helti", new CombatPosition(1, 2), t);
        //CreateCombatSailor("demo", new CombatPosition(1, 1), t);
        //CreateCombatSailor("demo2", new CombatPosition(2, 0), t);
        //CreateCombatSailor("demo2", new CombatPosition(2, 2), t);
        //CreateCombatSailor("demo", new CombatPosition(0, 2), t);
    }

    void CreateTargetTeam(Team t)
    {
        CreateCombatSailor("Target", new CombatPosition(0, 1), t);
        CreateCombatSailor("Target", new CombatPosition(1, 1), t);
        CreateCombatSailor("Target", new CombatPosition(2, 1), t);
        CreateCombatSailor("Target", new CombatPosition(0, 0), t);
        CreateCombatSailor("Target", new CombatPosition(1, 0), t);
        CreateCombatSailor("Target", new CombatPosition(2, 0), t);
        CreateCombatSailor("Target", new CombatPosition(0, 2), t);
        CreateCombatSailor("Target", new CombatPosition(1, 2), t);
        CreateCombatSailor("Target", new CombatPosition(2, 2), t);

        //CreateCombatSailor("Helti", new CombatPosition(1, 0), t);
        //CreateCombatSailor("Helti", new CombatPosition(1, 2), t);
    }

    CombatSailor CreateCombatSailor(string sailorString, CombatPosition pos, Team team)
    {
        string[] split = sailorString.Split(char.Parse("|"));
        string name = split[0];

        //Debug.Log("CreateCombatSailor>>> " + name);

        List<Item> listItem = new List<Item>();
        for (int i = 1; i < split.Length; i++)
        {
            string itemName = split[i].Split(char.Parse(":"))[0];
            string itemQuality = split[i].Split(char.Parse(":"))[1];
            listItem.Add(GameUtils.CreateItem(itemName, Int32.Parse(itemQuality)));
        }

        CombatSailor sailor = GameUtils.CreateCombatSailor(name);
        int quality = UnityEngine.Random.Range(1, 100 + 1);
        int level = UnityEngine.Random.Range(1, 10 + 1);

        sailor.SetEquipItems(listItem);
        sailor.InitCombatData(level, quality, pos, team);
        sailor.CreateStatusBar();
        sailor.InitDisplayStatus();
        if (team == Team.A) sailorsTeamA.Add(sailor);
        else sailorsTeamB.Add(sailor);

        return sailor;
    }

    CombatSailor CreateCombatSailor(SailorModel s, CombatPosition pos, Team team)
    {
        string name = s.config_stats.root_name;
        List<Item> listItem = s.items;

        CombatSailor sailor = GameUtils.CreateCombatSailor(s);

        sailor.SetEquipItems(listItem);
        sailor.InitCombatData(s.level, s.quality, pos, team);
        sailor.CreateStatusBar();
        sailor.InitDisplayStatus();
        if (team == Team.A) sailorsTeamA.Add(sailor);
        else sailorsTeamB.Add(sailor);

        return sailor;
    }

    public List<CombatSailor> GetAllCombatCharacters()
    {
        List<CombatSailor> result = new List<CombatSailor>();
        sailorsTeamA.ForEach(delegate (CombatSailor character)
        {
            result.Add(character);
        });
        sailorsTeamB.ForEach(delegate (CombatSailor character)
        {
            result.Add(character);
        });
        return result;
    }
    public List<CombatSailor> GetAllAliveCombatSailors()
    {
        List<CombatSailor> result = new List<CombatSailor>();
        sailorsTeamA.ForEach(character => {
            if (!character.IsDeath()) result.Add(character);
        });
        sailorsTeamB.ForEach(character =>
        {
            if (!character.IsDeath()) result.Add(character);
        });
        return result;
    }
    public List<CombatSailor> GetAllTeamAliveSailors(Team t)
    {
        List<CombatSailor> result = new List<CombatSailor>();
        List<CombatSailor> CTeam = t == Team.A ? sailorsTeamA : sailorsTeamB;
        CTeam.ForEach(delegate (CombatSailor character)
        {
            if (!character.IsDeath()) result.Add(character);
        });
        return result;
    }
    public List<CombatSailor> GetAliveCharacterEnermy(Team t)
    {
        List<CombatSailor> result = new List<CombatSailor>();
        List<CombatSailor> CTeam = t == Team.A ? sailorsTeamB : sailorsTeamA;
        CTeam.ForEach(delegate (CombatSailor character)
        {
            if (!character.IsDeath()) result.Add(character);
        });
        return result;
    }
    public List<CombatSailor> GetQueueNextActionSailor()
    {
        List<CombatSailor> result = GetAllAliveCombatSailors();
        result.Sort((sailor1, sailor2) =>
        {
            int speedNeed_1 = sailor1.GetSpeedNeeded();
            int speedNeed_2 = sailor2.GetSpeedNeeded();
            if (speedNeed_1 < speedNeed_2) return -1;
            else return 1;
        });
        return result;
    }

    private List<ClassBonusItem> CalculateClassBonus(List<CombatSailor> t)
    {
        List<ClassBonusItem> result = new List<ClassBonusItem>();

        List<int> typeCount = new List<int>();
        for (int i = 0; i < Enum.GetNames(typeof(SailorClass)).Length; i++)
        {
            typeCount.Add(0);
        }
        t.ForEach(sailor =>
        {
            foreach (SailorClass type in (SailorClass[])Enum.GetValues(typeof(SailorClass)))
            {
                if (sailor.cs.HaveType(type)) typeCount[(int)type] += 1;
            }
        });

        foreach (SailorClass type in Enum.GetValues(typeof(SailorClass)))
        {
            ContainerClassBonus config = GlobalConfigs.ClassBonus;
            if (!config.HaveBonus(type)) continue;
            var milestones = config.GetMilestones(type);
            for (int level = milestones.Count - 1; level >= 0; level--)
            {
                int popNeed = milestones[level];
                if (typeCount[(int)type] >= popNeed)
                {
                    result.Add(new ClassBonusItem() { type = type, level = level, current = typeCount[(int)type] });
                    break;
                }
            }
        }
        return result;
    }
    public void CalculateClassBonus()
    {
        ClassBonusA = CalculateClassBonus(sailorsTeamA);
        classBonusB = CalculateClassBonus(sailorsTeamB);
        //passiveTypeA.ForEach(p => Debug.Log("passiveTypeA: " + p.type + " " + p.level));
    }

    public void UpdateGameWithClassBonus()
    {
        sailorsTeamA.ForEach(sailor => sailor.UpdateCombatData(ClassBonusA, classBonusB));
        sailorsTeamB.ForEach(sailor => sailor.UpdateCombatData(classBonusB, ClassBonusA));
    }
    public ClassBonusItem GetTeamClassBonus(Team team, SailorClass type)
    {
        List<ClassBonusItem> t = team == Team.A ? ClassBonusA : classBonusB;
        return t.FirstOrDefault(e => e.type == type);
    }

    public void RunEndAction(CombatSailor actor)
    {
        actor.CountdownStatusRemain();
        var listSailor = GetAllCombatCharacters();
        listSailor.ForEach(sailor =>
        {
            sailor.CheckDeath();
        });
    }
    public float GetTeamHealthRatio(Team t)
    {
        List<CombatSailor> list = t == Team.A ? sailorsTeamA : sailorsTeamB;
        float health = 0;
        float total_health = 0;
        list.ForEach(sailor =>
        {
            health += sailor.cs.CurHealth;
            total_health += sailor.cs.MaxHealth;
        });
        return health / total_health;
    }
    public CombatSailor GetSailor(Team t, string id)
    {
        var l = GetAllTeamAliveSailors(t);
        return l.Find(sailor => {
            return sailor.Model.id == id;
        });
    }
};
