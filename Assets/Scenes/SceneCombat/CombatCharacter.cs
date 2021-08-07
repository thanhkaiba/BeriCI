using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public enum CombatCharacterStatusName
{
    DEATH,
    FROZEN,
    POISON,
    BURN,
};

public class CombatCharacterStatus
{
    public CombatCharacterStatusName name;
    public int remainTurn;
    public CombatCharacterStatus(CombatCharacterStatusName _name, int turn = 0)
    {
        name = _name;
        remainTurn = turn;
    }
};

public class Position
{
    public int x;
    public int y;
    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class CombatCharacter: MonoBehaviour
{
    public string charName;

    public int base_power;
    public int current_power;

    public int max_health;
    public int current_health;

    public int max_speed;
    public int current_speed;

    public int MAX_FURY;
    public int START_FURY;
    public int CURRENT_FURY;
    public int MAX_FURY_IN_COMBAT;

    public int level;

    public CharacterType type;

    public Position position;

    public List<CombatCharacterStatus> listStatus = new List<CombatCharacterStatus>();

    //public GameObject gameObject;

    public Team team;

    public void SetData(string name, int power, int health, int speed, int level, CharacterType type, Position position, Team team)
    {
        this.charName = name;
        this.base_power = power;
        this.current_power = power;
        this.max_health = health;
        this.current_health = health;
        this.max_speed = speed;
        this.current_speed = 0;
        this.level = level;
        this.type = type;
        this.position = position;
        this.team = team;
    }
    /*public void SetGameObject(GameObject GO)
    {
        gameObject = GO;
    }*/
    public int GetSpeedNeeded()
    {
        return max_speed - current_speed;
    }
    public void AddSpeed(int speedAdd)
    {
        current_speed += speedAdd;
    }
    public bool IsEnoughSpeed()
    {
        return GetSpeedNeeded() <= 0;
    }
    public bool HaveStatus(CombatCharacterStatusName name)
    {
        bool result = false;
        listStatus.ForEach(delegate (CombatCharacterStatus status)
        {
            if (status.name == name) result = true;
        });
        return result;
    }
    public bool IsDeath()
    {
        return HaveStatus(CombatCharacterStatusName.DEATH);
    }

    public void DoCombatAction(CombatState combatState)
    {
        bool useSkillCondition = false;
        if (useSkillCondition) UseSkill();
        else BaseAttack(combatState);
    }
    void UseSkill ()
    {

    }
    void BaseAttack(CombatState combatState)
    {
        current_speed -= max_speed;
        CombatCharacter target = GetBaseAttackTarget(combatState);
        if (target != null)
        {
            gameObject.GetComponent<CharacterAnimatorCtrl>().BaseAttack();
            StartCoroutine(DealDamageDelay(target, current_power, 0.4f));
        }
    }
    CombatCharacter GetBaseAttackTarget(CombatState combatState)
    {
        if (type == CharacterType.SHIPWRIGHT
            || true)
        {
            return GetNearestTarget(
                team == Team.A
                ? combatState.GetAllTeamAliveCharacter(Team.B)
                : combatState.GetAllTeamAliveCharacter(Team.A));
        }
        return null;
    }
    CombatCharacter GetNearestTarget(List<CombatCharacter> listTarget)
    {
        CombatCharacter result = null;
        int myRow = position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        listTarget.ForEach(delegate (CombatCharacter character)
        {
            int col = character.position.x;
            int row = character.position.y;
            if (
                (result == null)
                || (col < NR_col)
                || (col == NR_col && Math.Abs(myRow - row) < NR_row)
            )
            {
                result = character;
                NR_col = col;
                NR_row = Math.Abs(myRow - row);
            }
        });
        return result;
    }
    void DealDamage(CombatCharacter target, int physicsDamage)
    {
        target.TakeDamage(physicsDamage);
    }
    IEnumerator DealDamageDelay(CombatCharacter target, int current_power, float delay)
    {
        yield return new WaitForSeconds(delay);
        DealDamage(target, current_power);
    }
    public void TakeDamage(int physicsDamage)
    {
        Debug.Log("-------> Take Damage" + team + position + current_health);
        current_health -= physicsDamage;
        Debug.Log("-------> Take Damage" + team + position + current_health);
        if (current_health <= 0)
        {
            current_health = 0;
            AddStatus(new CombatCharacterStatus(CombatCharacterStatusName.DEATH));
        }
        UpdateGO();
    }
    void UpdateGO()
    {
        gameObject.GetComponent<CharacterAnimatorCtrl>().SetHealthBar(max_health, current_health);
        if (IsDeath()) gameObject.GetComponent<CharacterAnimatorCtrl>().Death();
    }
    public void AddStatus (CombatCharacterStatus status)
    {
        listStatus.Add(status);
    }
    public void InitDisplayStatus()
    {
        UnityEngine.Vector3 p = GameObject.Find("slot_" + (team == Team.A ? "A" : "B") + position.x + position.y).transform.position;
        transform.position = p;
        GameObject child = transform.Find("sword_man").gameObject;
        GetComponent<CharacterAnimatorCtrl>().SetHealthBar(max_health, current_health);
        child.transform.localScale = new Vector3(team == Team.A ? -100f : 100f, 100f, 100f);
    }
};
