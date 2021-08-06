using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatMgr : MonoBehaviour
{
    public Camera MainCamera;
    public CombatState combatState;
    private void Start()
    {
        /*for (int i = 0; i < 9; i++)
        {
            GameObject character2 = this.createCharacter(false, i);
        }*/
        combatState = new CombatState();
        combatState.charactersTeamA.ForEach(delegate (CombatCharacter character)
        {
            this.createCharacter(Team.A, character);
            this.createCharacter(Team.B, character);
        });
    }
    public GameObject characterGO;
    // create character on screen
    GameObject createCharacter(Team t, CombatCharacter character)
    {
        UnityEngine.Vector3 p = GameObject.Find("slot_" + (t == Team.A ? "l" : "r") + "_" + character.position.ToString()).transform.position;
        GameObject c = Instantiate(characterGO, p, Quaternion.identity);
        GameObject child = c.transform.Find("sword_man").gameObject;
        child.GetComponent<Billboard>().cam = MainCamera.transform;
        child.transform.localScale = new Vector3(t == Team.A ? -100f : 100f, 100f, 100f);
        return c;
    }
}

public enum Team
{
    A,
    B
};

public enum CombatStatus
{
    PREPARING,
    STARTED,
    ENDED
};

public class CombatState
{
    public CombatStatus status;
    public List<CombatCharacter> charactersTeamA = new List<CombatCharacter>();
    public List<CombatCharacter> charactersTeamB = new List<CombatCharacter>();
    public CombatState()
    {
        status = CombatStatus.PREPARING;
        this.createDemoTeam();
    }
    private void createDemoTeam ()
    {
        // test tao team
        for (int i = 0; i < 5; i++)
        {
            charactersTeamA.Add(new CombatCharacter("A " + i, 50, 500, 100, 10, CharacterType.SHIPWRIGHT, i));
            charactersTeamB.Add(new CombatCharacter("A " + i, 50, 500, 100, 10, CharacterType.SHIPWRIGHT, i));
        }
    }

};

public enum CharacterType
{
    SHIPWRIGHT,
    SNIPER,
    ARCHER,
    SWORD_MAN,
    DOCTOR,
    ENTERTAINER,
    WIZARD,
    ASSASSIN,
    PET
};

public class CombatCharacter
{
    public string Name;

    public int BASE_POWER;
    public int CURRENT_POWER;

    public int MAX_HEALTH;
    public int CURRENT_HEALTH;

    public int MAX_SPEED;
    public int CURRENT_SPEED;

    public int MAX_FURY;
    public int START_FURY;
    public int CURRENT_FURY;
    public int MAX_FURY_IN_COMBAT;

    public int LEVEL;

    public CharacterType TYPE;

    public int position;

    public CombatCharacter(string name, int power, int health, int speed, int level, CharacterType type, int position)
    {
        this.Name = name;
        this.BASE_POWER = power;
        this.CURRENT_POWER = power;
        this.MAX_HEALTH = health;
        this.CURRENT_HEALTH = health;
        this.MAX_SPEED = speed;
        this.CURRENT_SPEED = 0;
        this.LEVEL = level;
        this.TYPE = type;
        this.position = position;
    }

};