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
        lastTeamAction = Team.B;
    }
    public void CreateDemoTeam()
    {
        // test tao team
        /*for (int i = 0; i < 5; i++)
        {
            int x = i / 3;
            int y = i % 3;
            {
                GameObject characterGO = CreateCharacter();
                CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
                character.SetData("A " + i, Random.Range(40, 110), Random.Range(350, 700), Random.Range(60, 120), 10, CharacterType.SHIPWRIGHT, new Position(x, y), Team.A, i == 0 ? new FrozenAxe() : null);
                charactersTeamA.Add(character);
            }
            {
                GameObject characterGO = CreateCharacter();
                CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
                character.SetData("B " + i, Random.Range(40, 110), Random.Range(350, 700), Random.Range(60, 120), 10, CharacterType.SHIPWRIGHT, new Position(x, y), Team.B, i == 3 ? new NuclearBomb() : null);
                charactersTeamB.Add(character);
            }
        }*/
        // A
        {
            GameObject characterGO = CreateCharacter();
            CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
            character.SetData("A SuperTank", Random.Range(40, 60), Random.Range(1000, 1500), 100, 10, CharacterType.SHIPWRIGHT, new Position(0, 1), Team.A, new FrozenAxe());
            charactersTeamA.Add(character);
        }
        {
            GameObject characterGO = CreateCharacter();
            CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
            character.SetData("A SWORD MAN 1", Random.Range(70, 80), Random.Range(800, 1200), 80, 10, CharacterType.SWORD_MAN, new Position(1, 0), Team.A, new UltimateSlash());
            charactersTeamA.Add(character);
        }
        {
            GameObject characterGO = CreateCharacter();
            CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
            character.SetData("A SWORD MAN 2", Random.Range(70, 80), Random.Range(800, 1200), 60, 10, CharacterType.SWORD_MAN, new Position(1, 2), Team.A, null);
            charactersTeamA.Add(character);
        }
        {
            GameObject characterGO = CreateCharacter();
            CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
            character.SetData("A SNIPER", Random.Range(250, 300), Random.Range(250, 300), Random.Range(135, 150), 10, CharacterType.SNIPER, new Position(2, 1), Team.A, new LaserGun());
            charactersTeamA.Add(character);
        }
        {
            GameObject characterGO = CreateCharacter();
            CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
            character.SetData("A SNIPER", Random.Range(50, 60), Random.Range(350, 550), 90, 10, CharacterType.ENTERTAINER, new Position(1, 1), Team.A, new LifeRain());
            charactersTeamA.Add(character);
        }
        // B
        {
            GameObject characterGO = CreateCharacter();
            CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
            character.SetData("A SuperTank", Random.Range(40, 60), Random.Range(1000, 1500), 100, 10, CharacterType.SHIPWRIGHT, new Position(0, 1), Team.B, new TakeRest());
            charactersTeamB.Add(character);
        }
        {
            GameObject characterGO = CreateCharacter();
            CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
            character.SetData("A SWORD MAN 1", Random.Range(70, 80), Random.Range(800, 1200), 70, 10, CharacterType.SWORD_MAN, new Position(1, 1), Team.B, new Slash());
            charactersTeamB.Add(character);
        }
        {
            GameObject characterGO = CreateCharacter();
            CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
            character.SetData("A WIZARD", Random.Range(20, 25), Random.Range(800, 1200), 98, 10, CharacterType.WIZARD, new Position(2, 1), Team.B, new NuclearBomb());
            charactersTeamB.Add(character);
        }
        {
            GameObject characterGO = CreateCharacter();
            CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
            character.SetData("AN ARCHER ", Random.Range(45, 65), Random.Range(250, 350), 20, 10, CharacterType.ARCHER, new Position(2, 2), Team.B, null);
            charactersTeamB.Add(character);
        }
        {
            GameObject characterGO = CreateCharacter();
            CombatCharacter character = characterGO.AddComponent<CombatCharacter>() as CombatCharacter;
            character.SetData("A SNIPER", Random.Range(100, 135), Random.Range(350, 550), 115, 10, CharacterType.ASSASSIN, new Position(2, 0), Team.B, new Deceive());
            charactersTeamB.Add(character);
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
    GameObject CreateCharacter()
    {
        GameObject c = Instantiate(characterFrefab);
        c.GetComponent<Billboard>().cam = MainCamera.transform;
        return c;
    }
};
