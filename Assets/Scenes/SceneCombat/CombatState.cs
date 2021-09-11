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
    public List<CombatCharacter> charactersTeamA = new List<CombatCharacter>();
    public List<CombatCharacter> charactersTeamB = new List<CombatCharacter>();
    public Team lastTeamAction;
    public CombatState()
    {
        status = CombatStatus.PREPARING;
        lastTeamAction = Team.B;
    }
    public void CreateDemoTeam()
    {
        new DemoSailor();
        CreateRandomTeam(Team.A);
        CreateRandomTeam(Team.B);
    }
    void CreateRandomTeam(Team t)
    {
        CreateCombatCharacter(new Position(0, 0), t);
        CreateCombatCharacter(new Position(1, 1), t);
        CreateCombatCharacter(new Position(2, 0), t);
        CreateCombatCharacter(new Position(2, 2), t);
        CreateCombatCharacter(new Position(0, 2), t);
    }
    CombatCharacter CreateCombatCharacter(Position pos, Team team) // test
    {
        Sailor data = new DemoSailor();
        data.quality = Random.Range(1, 10 + 1);
        data.level = Random.Range(1, 100 + 1);
        return CreateCombatCharacterFromSailorData(data, pos, team);
    }
    CombatCharacter CreateCombatCharacterFromSailorData(Sailor data, Position pos, Team team)
    {
        GameObject characterGO = CreateCharacter(data.model_name);
        CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
        character.SetData(data, pos, team);
        if (team == Team.A) charactersTeamA.Add(character);
        else charactersTeamB.Add(character);
        return character;
    }
    public List<CombatCharacter> GetAllCombatCharacters()
    {
        List<CombatCharacter> result = new List<CombatCharacter>();
        charactersTeamA.ForEach(delegate (CombatCharacter character)
        {
            result.Add(character);
        });
        charactersTeamB.ForEach(delegate (CombatCharacter character)
        {
            result.Add(character);
        });
        return result;
    }
    public List<CombatCharacter> GetAllAliveCombatCharacters()
    {
        List<CombatCharacter> result = new List<CombatCharacter>();
        charactersTeamA.ForEach(delegate (CombatCharacter character)
        {
            if (!character.IsDeath()) result.Add(character);
        });
        charactersTeamB.ForEach(delegate (CombatCharacter character)
        {
            if (!character.IsDeath()) result.Add(character);
        });
        return result;
    }
    public List<CombatCharacter> GetAllTeamAliveCharacter(Team t)
    {
        List<CombatCharacter> result = new List<CombatCharacter>();
        List<CombatCharacter> CTeam = t == Team.A ? charactersTeamA : charactersTeamB;
        CTeam.ForEach(delegate (CombatCharacter character)
        {
            if (!character.IsDeath()) result.Add(character);
        });
        return result;
    }
    public List<CombatCharacter> GetAliveCharacterEnermy(Team t)
    {
        List<CombatCharacter> result = new List<CombatCharacter>();
        List<CombatCharacter> CTeam = t == Team.A ? charactersTeamB : charactersTeamA;
        CTeam.ForEach(delegate (CombatCharacter character)
        {
            if (!character.IsDeath()) result.Add(character);
        });
        return result;
    }
    GameObject CreateCharacter(string model_name)
    {
        GameObject c = Instantiate(Resources.Load<GameObject>("characters/" + model_name));
        c.GetComponent<Billboard>().cam = MainCamera.transform;
        var shadow = Instantiate(Resources.Load<GameObject>("characters/shadow"));
        shadow.GetComponent<CharacterShadow>().SetCharacter(c);
        return c;
    }
};
