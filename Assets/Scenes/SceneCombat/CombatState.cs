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
    public Camera MainCamera;

    public CombatStatus status;
    public List<Sailor> charactersTeamA = new List<Sailor>();
    public List<Sailor> charactersTeamB = new List<Sailor>();
    public Team lastTeamAction;
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
        CreateCombatSailor("demo", new CombatPosition(0, 0), t);
        CreateCombatSailor("demo", new CombatPosition(1, 1), t);
        CreateCombatSailor("demo2", new CombatPosition(2, 0), t);
        CreateCombatSailor("demo2", new CombatPosition(2, 2), t);
        CreateCombatSailor("demo", new CombatPosition(0, 2), t);
    }
    void CreateTargetTeam(Team t)
    {
        CreateCombatSailor("target", new CombatPosition(0, 0), t);
        CreateCombatSailor("target", new CombatPosition(1, 1), t);
        CreateCombatSailor("target", new CombatPosition(0, 2), t);
        CreateCombatSailor("target", new CombatPosition(2, 0), t);
        CreateCombatSailor("target", new CombatPosition(2, 2), t);
    }

    Sailor CreateCombatSailor(string name, CombatPosition pos, Team team)
    {
        int quality = Random.Range(1, 100 + 1);
        int level = Random.Range(1, 10 + 1);

        Sailor sailor = GameUtils.Instance.CreateSailor(name);

        Billboard billboard = sailor.gameObject.AddComponent<Billboard>() as Billboard;
        billboard.cam = MainCamera.transform;

        var shadow = Instantiate(Resources.Load<GameObject>("characters/shadow"));
        shadow.GetComponent<CharacterShadow>().SetCharacter(sailor.gameObject);

        sailor.InitCombatData(level, quality, pos, team);
        if (team == Team.A) charactersTeamA.Add(sailor);
        else charactersTeamB.Add(sailor);
        return sailor;
    }
    public List<Sailor> GetAllCombatCharacters()
    {
        List<Sailor> result = new List<Sailor>();
        charactersTeamA.ForEach(delegate (Sailor character)
        {
            result.Add(character);
        });
        charactersTeamB.ForEach(delegate (Sailor character)
        {
            result.Add(character);
        });
        return result;
    }
    public List<Sailor> GetAllAliveCombatCharacters()
    {
        List<Sailor> result = new List<Sailor>();
        charactersTeamA.ForEach(character => {
            if (!character.IsDeath()) result.Add(character);
        });
        charactersTeamB.ForEach(character =>
        {
            if (!character.IsDeath()) result.Add(character);
        });
        return result;
    }
    public List<Sailor> GetAllTeamAliveCharacter(Team t)
    {
        List<Sailor> result = new List<Sailor>();
        List<Sailor> CTeam = t == Team.A ? charactersTeamA : charactersTeamB;
        CTeam.ForEach(delegate (Sailor character)
        {
            if (!character.IsDeath()) result.Add(character);
        });
        return result;
    }
    public List<Sailor> GetAliveCharacterEnermy(Team t)
    {
        List<Sailor> result = new List<Sailor>();
        List<Sailor> CTeam = t == Team.A ? charactersTeamB : charactersTeamA;
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
};
