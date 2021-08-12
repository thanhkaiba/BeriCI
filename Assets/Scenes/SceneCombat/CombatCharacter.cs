using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
    public CombatCharacterStatus(CombatCharacterStatusName _name, int turn = 1)
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
    public CharBarControl bar;
    public GameObject characterBar;
    private void Start()
    {
        display = GetComponent<CharacterAnimatorCtrl>();
        bar = transform.Find("CharacterBar").GetComponent<CharBarControl>();
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
    public bool HaveStatus(CombatCharacterStatusName name)
    {
        return listStatus.Find(x => x.name == name) != null;
    }
    public int GetStatusRemainTurn(CombatCharacterStatusName name)
    {
        CombatCharacterStatus status = listStatus.Find(x => x.name == name);
        if (status == null) return 0;
        return status.remainTurn;
    }
    public void CountdownStatusRemain()
    {
        CombatCharacterStatusName[] listName = new CombatCharacterStatusName[]
        {
            CombatCharacterStatusName.FROZEN,
        };
        foreach (CombatCharacterStatusName statusName in listName)
        {
            CombatCharacterStatus status = listStatus.Find(status => status.name == statusName);
            if (status != null) status.remainTurn -= 1;
        }
        listStatus.RemoveAll(status => status.remainTurn <= 0);
    }
    public bool IsDeath()
    {
        return HaveStatus(CombatCharacterStatusName.DEATH);
    }

    public float DoCombatAction(CombatState combatState)
    {
        bool useSkillCondition = UseSkillCondition(combatState);
        if (HaveStatus(CombatCharacterStatusName.FROZEN)) return Immobile();
        else if(useSkillCondition) return UseSkill(combatState);
        else return BaseAttack(combatState);
    }
    bool UseSkillCondition (CombatState combatState)
    {
        return skill != null && current_fury >= current_max_fury && skill.CanActive(this, combatState);
    }
    float UseSkill (CombatState combatState)
    {
        current_speed -= max_speed;
        current_fury = 0;
        Debug.Log("Use skill now " + skill.name);
        bar.SetSpeedBar(max_speed, current_speed);
        bar.SetFuryBar(current_max_fury, current_fury);
        return skill.CastSkill(this, combatState);
    }
    float BaseAttack (CombatState combatState)
    {
        current_speed -= max_speed;
        bar.SetSpeedBar(max_speed, current_speed);
        GainFury(10);
        CombatCharacter target = GetBaseAttackTarget(combatState);
        if (target != null)
        {
            display.BaseAttack(target);
            StartCoroutine(DealDamageDelay(target, current_power, 0.4f));
        }
        return 0.8f;
    }
    float Immobile ()
    {
        current_speed -= max_speed;
        bar.SetSpeedBar(max_speed, current_speed);
        CountdownStatusRemain();
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition("Immobile", transform.position);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {
            display.DisplayStatus(listStatus);
            display.Immobile();
        });
        return 0.6f;
    }
    CombatCharacter GetBaseAttackTarget(CombatState combatState)
    {
        switch (type)
        {
            case CharacterType.ARCHER:
            case CharacterType.SNIPER:
                return GetNearestInRowTarget(
                team == Team.A
                    ? combatState.GetAllTeamAliveCharacter(Team.B)
                    : combatState.GetAllTeamAliveCharacter(Team.A));
            case CharacterType.ASSASSIN:
                return GetFurthestTarget(
                    team == Team.A
                    ? combatState.GetAllTeamAliveCharacter(Team.B)
                    : combatState.GetAllTeamAliveCharacter(Team.A));
            default:
                return GetNearestTarget(
                team == Team.A
                    ? combatState.GetAllTeamAliveCharacter(Team.B)
                    : combatState.GetAllTeamAliveCharacter(Team.A));
        }
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
        GainFury(3);
    }
    public void AddStatus (CombatCharacterStatus status)
    {
        CombatCharacterStatus existStatus = listStatus.Find(x => x.name == status.name);
        if (existStatus != null)
        {
            if (status.remainTurn > existStatus.remainTurn)
            {
                existStatus.remainTurn = status.remainTurn;
            }
        }
        else
        {
            listStatus.Add(status);
        }
        display.DisplayStatus(listStatus);
    }
    public void InitDisplayStatus()
    {
        UnityEngine.Vector3 p = GameObject.Find("slot_" + (team == Team.A ? "A" : "B") + position.x + position.y).transform.position;
        transform.position = p;
        GameObject child = transform.Find("sword_man").gameObject;
        bar.SetHealthBar(max_health, current_health);
        bar.SetSpeedBar(max_speed, current_speed);
        bar.SetFuryBar(current_max_fury, current_fury);
        bar.SetIconType(type);
        child.transform.localScale = new Vector3(team == Team.A ? -100f : 100f, 100f, 100f);
    }
    public void GainHealth(int health)
    {
        current_health += health;
        if (current_health > max_health) current_health = max_health;
        bar.SetHealthBar(max_health, current_health);
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
        bar.SetHealthBar(max_health, current_health);
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition("-" + health, transform.position);
    }
    public void GainFury(int value)
    {
        if (skill != null)
        {
            current_fury += value;
            if (current_fury > current_max_fury) current_fury = current_max_fury;
            bar.SetFuryBar(current_max_fury, current_fury);
        }
    }
    public int GetSpeedNeeded()
    {
        return max_speed - current_speed;
    }
    public void AddSpeed(int speedAdd)
    {
        current_speed += speedAdd;
        bar.SetSpeedBar(max_speed, current_speed);
    }
    public bool IsEnoughSpeed()
    {
        return GetSpeedNeeded() <= 0;
    }
    // target
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

    CombatCharacter GetNearestInRowTarget(List<CombatCharacter> listTarget)
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
                || (Math.Abs(myRow - row) < NR_row)
                || (Math.Abs(myRow - row) == NR_row && col < NR_col)
            )
            {
                result = character;
                NR_col = col;
                NR_row = Math.Abs(myRow - row);
            }
        });
        return result;
    }
    CombatCharacter GetFurthestTarget(List<CombatCharacter> listTarget)
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
                || (col > NR_col)
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
};
