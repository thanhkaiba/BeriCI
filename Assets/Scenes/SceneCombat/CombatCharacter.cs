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

    public int max_fury;
    public int current_fury;
    public int current_max_fury = 0;

    public int level;

    public CharacterType type;

    public Position position;

    public List<CombatCharacterStatus> listStatus = new List<CombatCharacterStatus>();

    public Skill skill = null;

    public Team team;

    public CharacterAnimatorCtrl display;
    private void Start()
    {
        display = GetComponent<CharacterAnimatorCtrl>();
        InitDisplayStatus();
    }
    public void SetData(string name, int power, int health, int speed, int level, CharacterType type, Position position, Team team, Skill skill)
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
        this.skill = skill;
        if (skill != null)
        {
            max_fury = skill.MAX_FURY;
            current_fury = skill.START_FURY;
            current_max_fury = skill.MAX_FURY;
        }
    }
    /*public void SetGameObject(GameObject GO)
    {
        gameObject = GO;
    }*/
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

    public float DoCombatAction(CombatState combatState)
    {
        bool useSkillCondition = UseSkillCondition();
        if (useSkillCondition) return UseSkill(combatState);
        else return BaseAttack(combatState);
    }
    bool UseSkillCondition ()
    {
        return skill != null && current_fury >= current_max_fury;
    }
    float UseSkill (CombatState combatState)
    {
        current_speed -= max_speed;
        current_fury = 0;
        Debug.Log("Use skill now " + skill.name);
        display.SetSpeedBar(max_speed, current_speed);
        display.SetFuryBar(current_max_fury, current_fury);
        return skill.CastSkill(this, combatState);
    }
    float BaseAttack(CombatState combatState)
    {
        current_speed -= max_speed;
        display.SetSpeedBar(max_speed, current_speed);
        GainFury(10);
        CombatCharacter target = GetBaseAttackTarget(combatState);
        if (target != null)
        {
            display.BaseAttack(target);
            StartCoroutine(DealDamageDelay(target, current_power, 0.4f));
        }
        return 0.8f;
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
        LoseHealth(physicsDamage);
        GainFury(4);
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
        display.SetHealthBar(max_health, current_health);
        display.SetSpeedBar(max_speed, current_speed);
        display.SetFuryBar(current_max_fury, current_fury);
        child.transform.localScale = new Vector3(team == Team.A ? -100f : 100f, 100f, 100f);
    }
    public void GainHealth (int health)
    {
        current_health += health;
        if (current_health > max_health) current_health = max_health;
        display.SetHealthBar(max_health, current_health);
    }
    public void LoseHealth(int health)
    {
        current_health -= health;
        if (current_health <= 0)
        {
            current_health = 0;
            AddStatus(new CombatCharacterStatus(CombatCharacterStatusName.DEATH));
            if (IsDeath()) display.Death();
        }
        display.SetHealthBar(max_health, current_health);
    }
    public void GainFury(int value)
    {
        if (skill != null)
        {
            current_fury += value;
            if (current_fury > current_max_fury) current_fury = current_max_fury;
            display.SetFuryBar(current_max_fury, current_fury);
        }
    }
    public int GetSpeedNeeded()
    {
        return max_speed - current_speed;
    }
    public void AddSpeed(int speedAdd)
    {
        current_speed += speedAdd;
        display.SetSpeedBar(max_speed, current_speed);
    }
    public bool IsEnoughSpeed()
    {
        return GetSpeedNeeded() <= 0;
    }
};
