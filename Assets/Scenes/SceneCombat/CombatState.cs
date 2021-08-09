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
    public GameObject characterFrefab;
    public CombatState()
    {
        status = CombatStatus.PREPARING;
        //CreateDemoTeam();
        lastTeamAction = Team.B;
    }
    public void CreateDemoTeam()
    {
        // test tao team
        for (int i = 0; i < 5; i++)
        {
            int x = i / 3;
            int y = i % 3;
            {
                GameObject characterGO = CreateCharacter();
                CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
                character.SetData("A " + i, Random.Range(40, 110), Random.Range(350, 700), Random.Range(60, 120), 10, CharacterType.SHIPWRIGHT, new Position(x, y), Team.A, i == 0 ? new TakeRest() : null);
                charactersTeamA.Add(character);
            }
            {
                GameObject characterGO = CreateCharacter();
                CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
                character.SetData("B " + i, Random.Range(40, 110), Random.Range(350, 700), Random.Range(60, 120), 10, CharacterType.SHIPWRIGHT, new Position(x, y), Team.B, null);
                charactersTeamB.Add(character);
            }
        }
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
    GameObject CreateCharacter()
    {
        GameObject c = Instantiate(characterFrefab);
        c.GetComponent<Billboard>().cam = MainCamera.transform;
        return c;
    }
};
