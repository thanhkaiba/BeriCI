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
    public static CombatState instance;
    public Camera MainCamera;

    public CombatStatus status;
    public List<Sailor> sailorsTeamA = new List<Sailor>();
    public List<Sailor> sailorsTeamB = new List<Sailor>();
    public Team lastTeamAction;

    public List<PassiveType> passiveTypeA = new List<PassiveType>();
    public List<PassiveType> passiveTypeB = new List<PassiveType>();
    private void Awake()
    {
        instance = this;
    }
    public CombatState()
    {
        status = CombatStatus.PREPARING;
        lastTeamAction = Team.B;
    }
    public void CreateDemoTeam()
    {
        CreateRandomTeam(Team.A);
        CreateRandomTeam(Team.B);
    }
    void CreateRandomTeam(Team t)
    {
        CreateCombatSailor("helti", new CombatPosition(1, 0), t);
        CreateCombatSailor("helti", new CombatPosition(1, 2), t);
        //CreateCombatSailor("demo", new CombatPosition(1, 1), t);
        //CreateCombatSailor("demo2", new CombatPosition(2, 0), t);
        //CreateCombatSailor("demo2", new CombatPosition(2, 2), t);
        //CreateCombatSailor("demo", new CombatPosition(0, 2), t);
    }
    void CreateTargetTeam(Team t)
    {
        CreateCombatSailor("target", new CombatPosition(0, 0), t);
        CreateCombatSailor("target", new CombatPosition(1, 1), t);
        CreateCombatSailor("target", new CombatPosition(0, 2), t);
    }

    Sailor CreateCombatSailor(string name, CombatPosition pos, Team team)
    {
        int quality = UnityEngine.Random.Range(1, 100 + 1);
        int level = UnityEngine.Random.Range(1, 10 + 1);

        Sailor sailor = GameUtils.Instance.CreateSailor(name);

        Billboard billboard = sailor.gameObject.AddComponent<Billboard>() as Billboard;
        billboard.cam = MainCamera.transform;

        var shadow = Instantiate(Resources.Load<GameObject>("characters/shadow"));
        shadow.GetComponent<CharacterShadow>().SetCharacter(sailor.gameObject);

        sailor.SetEquipItems(new List<Item>());
        sailor.InitCombatData(level, quality, pos, team);
        if (team == Team.A) sailorsTeamA.Add(sailor);
        else sailorsTeamB.Add(sailor);

        return sailor;
    }
    public List<Sailor> GetAllCombatCharacters()
    {
        List<Sailor> result = new List<Sailor>();
        sailorsTeamA.ForEach(delegate (Sailor character)
        {
            result.Add(character);
        });
        sailorsTeamB.ForEach(delegate (Sailor character)
        {
            result.Add(character);
        });
        return result;
    }
    public List<Sailor> GetAllAliveCombatCharacters()
    {
        List<Sailor> result = new List<Sailor>();
        sailorsTeamA.ForEach(character => {
            if (!character.IsDeath()) result.Add(character);
        });
        sailorsTeamB.ForEach(character =>
        {
            if (!character.IsDeath()) result.Add(character);
        });
        return result;
    }
    public List<Sailor> GetAllTeamAliveCharacter(Team t)
    {
        List<Sailor> result = new List<Sailor>();
        List<Sailor> CTeam = t == Team.A ? sailorsTeamA : sailorsTeamB;
        CTeam.ForEach(delegate (Sailor character)
        {
            if (!character.IsDeath()) result.Add(character);
        });
        return result;
    }
    public List<Sailor> GetAliveCharacterEnermy(Team t)
    {
        List<Sailor> result = new List<Sailor>();
        List<Sailor> CTeam = t == Team.A ? sailorsTeamB : sailorsTeamA;
        CTeam.ForEach(delegate (Sailor character)
        {
            if (!character.IsDeath()) result.Add(character);
        });
        return result;
    }
    public List<Sailor> GetQueueNextActionSailor()
    {
        List<Sailor> result = GetAllAliveCombatCharacters();
        result.Sort(delegate (Sailor sailor1, Sailor sailor2)
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
        passiveTypeA = CalculateTypePassive(sailorsTeamA);
        passiveTypeB = CalculateTypePassive(sailorsTeamB);
        //passiveTypeA.ForEach(p => Debug.Log("passiveTypeA: " + p.type + " " + p.level));
    }

    private List<PassiveType> CalculateTypePassive(List<Sailor> t)
    {
        List<PassiveType> result = new List<PassiveType>();

        List<int> typeCount = new List<int>();
        for (int i = 0; i < Enum.GetNames(typeof(SailorType)).Length; i++) {
            typeCount.Add(0);
        }
        t.ForEach(sailor =>
        {
            foreach (SailorType type in (SailorType[])Enum.GetValues(typeof(SailorType)))
            {
                if (sailor.cs.HaveType(type)) typeCount[(int)type] += 1;
            }
        });
        // chuyen vao config sau
        if (typeCount[(int)SailorType.WILD] >= 3) result.Add(new PassiveType() { type = SailorType.WILD, level = 1 });

        if (typeCount[(int)SailorType.SWORD_MAN] >= 4) result.Add(new PassiveType() { type = SailorType.SWORD_MAN, level = 2 });
        else if (typeCount[(int)SailorType.SWORD_MAN] >= 2) result.Add(new PassiveType() { type = SailorType.SWORD_MAN, level = 1 });

        // do next here

        //
        return result;
    }

    public void UpdateGameWithPassive()
    {
        sailorsTeamA.ForEach(sailor => sailor.UpdateCombatData(passiveTypeA, passiveTypeB));
        sailorsTeamB.ForEach(sailor => sailor.UpdateCombatData(passiveTypeB, passiveTypeA));
    }
};
