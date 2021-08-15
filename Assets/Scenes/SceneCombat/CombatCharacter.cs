using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

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

    public float base_power;
    public float current_power;

    public float max_health;
    public float current_health;

    public float base_armor;
    public float current_armor;

    public float base_magic_resist;
    public float current_magic_resist;

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

    public CharacterModel model;

    public CharacterAnimatorCtrl display;
    public CharBarControl bar;
    public GameObject characterBar;
    private void Start()
    {
        display = GetComponent<CharacterAnimatorCtrl>();
        bar = CreateStatusBar();
        
        InitDisplayStatus();
    }
    CharBarControl CreateStatusBar ()
    {
        var barPrefab = Resources.Load<GameObject>("characters/Bar/Bar2");
        var barGO = Instantiate<GameObject>(
            barPrefab,
            transform.Find("nodeBar"));
        barGO.transform.localScale = new Vector3(0.018f, 0.018f, 0.018f);
        barGO.transform.localPosition = new Vector3(0, 0, 0);
        return barGO.transform.GetComponent<CharBarControl>();
    }
    public void SetData(Character data, Position p, Team t)
    {
        charName = data.name;
        base_power = data.GetPower();
        current_power = data.GetPower();
        max_health = data.GetHealth();
        current_health = data.GetHealth();
        base_armor = data.GetArmor();
        current_armor = data.GetArmor();
        base_magic_resist = data.GetMagicResist();
        current_magic_resist = data.GetMagicResist();
        max_speed = data.GetSpeed();
        current_speed = 0;
        level = data.level;
        type = data.TYPE;
        skill = data.skill;
        position = p;
        team = t;
        if (skill != null)
        {
            max_fury = skill.MAX_FURY;
            current_fury = skill.START_FURY;
            current_max_fury = skill.MAX_FURY;
        }
        model = data.model;
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
        float delay = 0;
        if (target != null)
        {
            delay += display.BaseAttack(target);
            StartCoroutine(DealBaseAttackDamageDelay(target, current_power, delay));
        }
        return delay + 0.2f;
    }
    float Immobile ()
    {
        current_speed -= max_speed;
        bar.SetSpeedBar(max_speed, current_speed);
        CountdownStatusRemain();
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition("Immobile", transform.position);
        Sequence seq = DOTween.Sequence();
        
        display.DisplayStatus(listStatus);
        return display.Immobile() + 0.2f;
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
    void DealBaseAttackDamage(CombatCharacter target, float physicsDamage)
    {
        target.TakeDamage(physicsDamage, 0, 0);
    }
    IEnumerator DealBaseAttackDamageDelay(CombatCharacter target, float current_power, float delay)
    {
        yield return new WaitForSeconds(delay);
        DealBaseAttackDamage(target, current_power);
    }
    public float TakeDamage(float physicsDamage, float magicDamage = 0, float trueDamage = 0)
    {
        float physicTake = physicsDamage * 100 / (100 + current_armor);
        float magicTake = magicDamage * 100 / (100 + current_magic_resist);
        float totalDamage = physicTake + magicTake + trueDamage;
        LoseHealth(totalDamage);
        GainFury(4);
        return totalDamage;
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
        bar.SetHealthBar(max_health, current_health);
        bar.SetSpeedBar(max_speed, current_speed);
        bar.SetFuryBar(current_max_fury, current_fury);
        bar.SetIconType(type);
        bar.SetIconSkill(skill);
        bar.SetName(charName);
        display.SetFaceDirection(team == Team.A ? -1 : 1);
    }
    public void GainHealth(float health)
    {
        current_health += health;
        if (current_health > max_health) current_health = max_health;
        bar.SetHealthBar(max_health, current_health);
    }
    public void LoseHealth(float health)
    {
        current_health -= health;
        if (current_health <= 0)
        {
            current_health = 0;
            AddStatus(new CombatCharacterStatus(CombatCharacterStatusName.DEATH));
            if (IsDeath()) display.Death();
        }
        bar.SetHealthBar(max_health, current_health);
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition("-" + (int)health, transform.position);
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
