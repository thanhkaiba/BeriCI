using System;
using System.Collections;
using System.Collections.Generic;
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

    public List<ClassBonusItem> passiveTypeA = new List<ClassBonusItem>();
    public List<ClassBonusItem> passiveTypeB = new List<ClassBonusItem>();
    private void Awake()
    {
        Instance = this;
    }
    public CombatState()
    {
        status = CombatStatus.PREPARING;
        lastTeamAction = Team.B;
    }
    public void CreateDemoTeam()
    {
        CreateRandomTeam(Team.A);
        CreateTargetTeam(Team.B);
    }
    void CreateRandomTeam(Team t)
    {
        //CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(0, 1), t);
        //CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(1, 1), t);
        //CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(2, 1), t);
        //CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(0, 0), t);
        CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(1, 0), t);
        //CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(2, 0), t);
        //CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(0, 2), t);
        CreateCombatSailor("Helti|DemoItem:20", new CombatPosition(1, 2), t);
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
    public List<CombatSailor> GetAllAliveCombatCharacters()
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
    public List<CombatSailor> GetAllTeamAliveCharacter(Team t)
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
        List<CombatSailor> result = GetAllAliveCombatCharacters();
        result.Sort(delegate (CombatSailor sailor1, CombatSailor sailor2)
        {
            int speedNeed_1 = sailor1.GetSpeedNeeded();
            int speedNeed_2 = sailor2.GetSpeedNeeded();
            if (speedNeed_1 < speedNeed_2) return -1;
            else return 1;
        });
        return result;
    }
    public void CalculateTypePassive()
    {
        passiveTypeA = GameUtils.CalculateClassBonus(sailorsTeamA);
        passiveTypeB = GameUtils.CalculateClassBonus(sailorsTeamB);
        //passiveTypeA.ForEach(p => Debug.Log("passiveTypeA: " + p.type + " " + p.level));
    }

    public void UpdateGameWithClassBonus()
    {
        sailorsTeamA.ForEach(sailor => sailor.UpdateCombatData(passiveTypeA, passiveTypeB));
        sailorsTeamB.ForEach(sailor => sailor.UpdateCombatData(passiveTypeB, passiveTypeA));
    }
    public ClassBonusItem GetTeamPassiveType(Team team, SailorType type)
    {
        List<ClassBonusItem> t = team == Team.A ? passiveTypeA : passiveTypeB;
        return t.Find(e => e.type == type);
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
};
