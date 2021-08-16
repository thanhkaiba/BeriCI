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
        CreateRandomTeam(Team.A);
        CreateRandomTeam(Team.B);
    }
    void CreateRandomTeam(Team t)
    {
        int r = Random.Range(0, 12);
        switch (r)
        {
            case 0:
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new FrozenAxe(), new Position(0, 0), t);
                CreateCombatCharacter(CharacterType.SWORD_MAN, new UltimateSlash(), new Position(1, 1), t);
                CreateCombatCharacter(CharacterType.SNIPER, new HeadShot(), new Position(2, 0), t);
                CreateCombatCharacter(CharacterType.SNIPER, new HeadShot(), new Position(2, 2), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 2), t);
                break;
            case 1:
                CreateCombatCharacter(CharacterType.SWORD_MAN, new FightingPassion(), new Position(0, 1), t);
                CreateCombatCharacter(CharacterType.LOGISTICS, new Encourage(), new Position(0, 0), t);
                CreateCombatCharacter(CharacterType.LOGISTICS, new Encourage(), new Position(0, 2), t);
                CreateCombatCharacter(CharacterType.LOGISTICS, new Encourage(), new Position(2, 0), t);
                CreateCombatCharacter(CharacterType.LOGISTICS, new Encourage(), new Position(2, 2), t);
                break;
            case 2:
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 0), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 2), t);
                CreateCombatCharacter(CharacterType.ARCHER, new IceArrow(), new Position(2, 0), t);
                CreateCombatCharacter(CharacterType.ARCHER, new IceArrow(), new Position(2, 2), t);
                CreateCombatCharacter(CharacterType.ARCHER, new IceArrow(), new Position(1, 1), t);
                break;
            case 3:
                CreateCombatCharacter(CharacterType.LOGISTICS, new HighNote(), new Position(0, 1), t);
                CreateCombatCharacter(CharacterType.WIZARD, new NuclearBomb(), new Position(2, 1), t);
                CreateCombatCharacter(CharacterType.LOGISTICS, new HighNote(), new Position(1, 1), t);
                CreateCombatCharacter(CharacterType.LOGISTICS, new HighNote(), new Position(0, 2), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 0), t);
                break;
            case 4:
                CreateCombatCharacter(CharacterType.ASSASSIN, new Deceive(), new Position(2, 0), t);
                CreateCombatCharacter(CharacterType.ASSASSIN, new Deceive(), new Position(2, 1), t);
                CreateCombatCharacter(CharacterType.ASSASSIN, new Deceive(), new Position(2, 2), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 2), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 0), t);
                break;
            case 5:
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new FrozenAxe(), new Position(0, 1), t);
                CreateCombatCharacter(CharacterType.ARCHER, new HeadShot(), new Position(2, 0), t);
                CreateCombatCharacter(CharacterType.ARCHER, new HeadShot(), new Position(2, 2), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new FrozenAxe(), new Position(0, 2), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new FrozenAxe(), new Position(0, 0), t);
                break;
            case 6:
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new FrozenAxe(), new Position(2, 1), t);
                CreateCombatCharacter(CharacterType.WIZARD, new NuclearBomb(), new Position(1, 1), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 0), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 1), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 2), t);
                break;
            case 7:
                CreateCombatCharacter(CharacterType.ARCHER, new IceArrow(), new Position(2, 2), t);
                CreateCombatCharacter(CharacterType.SNIPER, new HeadShot(), new Position(2, 0), t);
                CreateCombatCharacter(CharacterType.SWORD_MAN, new FightingPassion(), new Position(1, 1), t);
                CreateCombatCharacter(CharacterType.SWORD_MAN, new UltimateSlash(), new Position(0, 2), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new FrozenAxe(), new Position(0, 0), t);
                break;
            case 8:
                CreateCombatCharacter(CharacterType.SWORD_MAN, new Slash(), new Position(2, 2), t);
                CreateCombatCharacter(CharacterType.SWORD_MAN, new Slash(), new Position(2, 0), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new FrozenAxe(), new Position(0, 1), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new FrozenAxe(), new Position(0, 2), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new FrozenAxe(), new Position(0, 0), t);
                break;
            case 9:
                CreateCombatCharacter(CharacterType.LOGISTICS, new Encourage(), new Position(2, 2), t);
                CreateCombatCharacter(CharacterType.LOGISTICS, new HighNote(), new Position(2, 0), t);
                CreateCombatCharacter(CharacterType.SWORD_MAN, new FrozenAxe(), new Position(0, 1), t);
                CreateCombatCharacter(CharacterType.SWORD_MAN, new Slash(), new Position(0, 2), t);
                CreateCombatCharacter(CharacterType.SWORD_MAN, new UltimateSlash(), new Position(0, 0), t);
                break;
            case 10:
                CreateCombatCharacter(CharacterType.LOGISTICS, new HighNote(), new Position(2, 2), t);
                CreateCombatCharacter(CharacterType.LOGISTICS, new HighNote(), new Position(2, 0), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 0), t);
                CreateCombatCharacter(CharacterType.SHIPWRIGHT, new TakeRest(), new Position(0, 2), t);
                CreateCombatCharacter(CharacterType.SWORD_MAN, new UltimateSlash(), new Position(0, 1), t);
                break;
            case 11:
                CreateCombatCharacter(CharacterType.SWORD_MAN, new FightingPassion(), new Position(2, 2), t);
                CreateCombatCharacter(CharacterType.SWORD_MAN, new FightingPassion(), new Position(2, 0), t);
                CreateCombatCharacter(CharacterType.SWORD_MAN, new FightingPassion(), new Position(0, 0), t);
                CreateCombatCharacter(CharacterType.SWORD_MAN, new FightingPassion(), new Position(0, 2), t);
                CreateCombatCharacter(CharacterType.SWORD_MAN, new FightingPassion(), new Position(0, 1), t);
                break;
        }
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
                c = Instantiate(Resources.Load<GameObject>("characters/sword_man"));
                break;
            case CharacterModel.GOBLIN_ARCHER:
                c = Instantiate(Resources.Load<GameObject>("characters/goblin_archer"));
                break;
            default:
                c = Instantiate(Resources.Load<GameObject>("characters/sword_man"));
                break;
        }
        c.GetComponent<Billboard>().cam = MainCamera.transform;
        var shadow = Instantiate(Resources.Load<GameObject>("characters/shadow"));
        shadow.GetComponent<CharacterShadow>().SetCharacter(c);
        return c;
    }
};
