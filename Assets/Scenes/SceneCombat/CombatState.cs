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
    public GameObject warriorPrefab;
    public GameObject goblinPrefab;

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
        // A
        CreateCombatCharacter(CharacterType.SHIPWRIGHT, new FrozenAxe(), new Position(0, 0), Team.A);
        CreateCombatCharacter(CharacterType.SWORD_MAN, new UltimateSlash(), new Position(1, 1), Team.A);
        CreateCombatCharacter(CharacterType.SNIPER, new HeadShot(), new Position(2, 0), Team.A);
        CreateCombatCharacter(CharacterType.SNIPER, new HeadShot(), new Position(2, 2), Team.A);
        CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 2), Team.A);
        // A2
        //CreateCombatCharacter(CharacterType.SWORD_MAN, new FightingPassion(), new Position(0, 1), Team.A);
        //CreateCombatCharacter(CharacterType.LOGISTICS, new Encourage(), new Position(1, 0), Team.A);
        //CreateCombatCharacter(CharacterType.LOGISTICS, new Encourage(), new Position(2, 0), Team.A);
        //CreateCombatCharacter(CharacterType.LOGISTICS, new Encourage(), new Position(2, 2), Team.A);
        //CreateCombatCharacter(CharacterType.LOGISTICS, new Encourage(), new Position(1, 2), Team.A);
        // B
        //CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 1), Team.B);
        //CreateCombatCharacter(CharacterType.LOGISTICS, new Encourage(), new Position(1, 1), Team.B);
        //CreateCombatCharacter(CharacterType.WIZARD, new NuclearBomb(), new Position(2, 1), Team.B);
        //CreateCombatCharacter(CharacterType.ARCHER, null, new Position(2, 2), Team.B);
        //CreateCombatCharacter(CharacterType.ASSASSIN, new Deceive(), new Position(2, 0), Team.B);
        // B2
        //CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 1), Team.B);
        //CreateCombatCharacter(CharacterType.LOGISTICS, new Encourage(), new Position(1, 1), Team.B);
        //CreateCombatCharacter(CharacterType.LOGISTICS, new Encourage(), new Position(2, 1), Team.B);
        //CreateCombatCharacter(CharacterType.LOGISTICS, new Encourage(), new Position(2, 2), Team.B);
        //CreateCombatCharacter(CharacterType.LOGISTICS, new Encourage(), new Position(2, 0), Team.B);
        // B3
        CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 0), Team.B);
        CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 2), Team.B);
        CreateCombatCharacter(CharacterType.ARCHER, new IceArrow(), new Position(2, 0), Team.B);
        CreateCombatCharacter(CharacterType.ARCHER, new IceArrow(), new Position(2, 2), Team.B);
        CreateCombatCharacter(CharacterType.ARCHER, new IceArrow(), new Position(1, 1), Team.B);
        // B4
        //CreateCombatCharacter(CharacterType.LOGISTICS, new HighNote(), new Position(0, 1), Team.B);
        //CreateCombatCharacter(CharacterType.WIZARD, new NuclearBomb(), new Position(2, 1), Team.B);
        //CreateCombatCharacter(CharacterType.LOGISTICS, new HighNote(), new Position(1, 1), Team.B);
        //CreateCombatCharacter(CharacterType.LOGISTICS, new HighNote(), new Position(0, 2), Team.B);
        //CreateCombatCharacter(CharacterType.LOGISTICS, new HighNote(), new Position(0, 0), Team.B);


        //CreateCombatCharacter(CharacterType.ARCHER, null, new Position(2, 1), Team.A);
        //CreateCombatCharacter(CharacterType.SHIPWRIGHT, null, new Position(1, 1), Team.A);
        //CreateCombatCharacter(CharacterType.ARCHER, null, new Position(2, 0), Team.B);
        //CreateCombatCharacter(CharacterType.ARCHER, null, new Position(2, 2), Team.B);

        // team tank
        //CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 1), Team.A);
        //CreateCombatCharacter(CharacterType.ASSASSIN, new Deceive(), new Position(2, 1), Team.A);
        //CreateCombatCharacter(CharacterType.ASSASSIN, new Deceive(), new Position(1, 1), Team.A);
        //CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 2), Team.A);
        //CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 0), Team.A);

        //CreateCombatCharacter(CharacterType.SHIPWRIGHT, new FrozenAxe(), new Position(0, 1), Team.B);
        //CreateCombatCharacter(CharacterType.ARCHER, new HeadShot(), new Position(2, 0), Team.B);
        //CreateCombatCharacter(CharacterType.ARCHER, new HeadShot(), new Position(2, 2), Team.B);
        //CreateCombatCharacter(CharacterType.SHIPWRIGHT, new FrozenAxe(), new Position(0, 2), Team.B);
        //CreateCombatCharacter(CharacterType.SHIPWRIGHT, new FrozenAxe(), new Position(0, 0), Team.B);
    }
    CombatCharacter CreateCombatCharacter(CharacterType type, Skill skill, Position pos, Team team) // test
    {
        Character data = new Character(type);
        data.SetRandomStats();
        data.level = 10;
        data.SetSkill(skill);
        return CreateCombatCharacterFromCharacterData(data, pos, team);
    }
    CombatCharacter CreateCombatCharacterFromCharacterData(Character data, Position pos, Team team)
    {
        GameObject characterGO = CreateCharacter(data.model);
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
    GameObject CreateCharacter(CharacterModel model)
    {
        GameObject c;
        switch (model)
        {
            case CharacterModel.WARRIOR:
                c = Instantiate(warriorPrefab);
                break;
            case CharacterModel.GOBLIN_ARCHER:
                c = Instantiate(goblinPrefab);
                break;
            default:
                c = Instantiate(warriorPrefab);
                break;
        }
        c.GetComponent<Billboard>().cam = MainCamera.transform;
        return c;
    }
};
