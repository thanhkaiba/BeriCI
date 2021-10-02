﻿using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class Sailor: MonoBehaviour
{
    protected string config_url; // must define in extended
    public ConfigStats config_stats;
    public CombatStats cs;

    public string charName;
    public Skill skill = null;
    public int level;
    public int quality;

    public CharBarControl bar;
    public virtual void Awake()
    {
        using (StreamReader r = new StreamReader(config_url))
        {
            string json = r.ReadToEnd();
            config_stats = JsonConvert.DeserializeObject<ConfigStats>(json);
        }
    }
    CharBarControl CreateStatusBar()
    {
        var barPrefab = Resources.Load<GameObject>("characters/Bar/Bar2");
        var barGO = Instantiate<GameObject>(
            barPrefab,
            transform.Find("nodeBar"));
        barGO.transform.localScale = new Vector3(0.024f, 0.024f, 1f);
        barGO.transform.localPosition = new Vector3(0, 0, 0);
        return barGO.transform.GetComponent<CharBarControl>();
    }
    public void InitCombatData(int level, int quality, CombatPosition p, Team t)
    {
        cs = new CombatStats()
        {
            BasePower = config_stats.GetPower(level, quality),
            MaxHealth = config_stats.GetHealth(level, quality),
            CurHealth = config_stats.GetHealth(level, quality),
            BaseArmor = config_stats.GetArmor(),
            BaseMagicResist = config_stats.GetMagicResist(),
            DisplaySpeed = config_stats.GetSpeed(level, quality),
            CurrentSpeed = 0,
            position = p,
            team = t,
        };
        foreach (SailorType type in config_stats.types)
        {
            cs.types.Add(type);
        }

        charName = config_stats.root_name;

        this.level = level;
        this.quality = quality;

        if (skill != null)
        {
            cs.BaseFury = skill.MAX_FURY;
            cs.Fury = skill.START_FURY;
        }
        bar = CreateStatusBar();
        InitDisplayStatus();
    }
    public void UpdateCombatData(List<PassiveType> ownTeam, List<PassiveType> oppTeam)
    {
        ownTeam.ForEach(p =>
        {
            switch (p.type)
            {
                case SailorType.SWORD_MAN:
                    if (cs.HaveType(SailorType.SWORD_MAN))
                    {
                        if (p.level == 1) cs.DisplaySpeed += 12;
                        if (p.level == 2) cs.DisplaySpeed += 20;
                    }
                    break;
                case SailorType.SUPPORT:
                    if (p.level == 1) cs.Fury += 10;
                    if (p.level == 2) cs.Fury += 15;
                    break;
                case SailorType.ASSASSIN:
                    if (cs.HaveType(SailorType.ASSASSIN))
                    {
                        if (p.level == 1) cs.BasePower *= 1.15f;
                        if (p.level == 2) cs.BasePower *= 1.20f;
                        if (p.level == 2) cs.BasePower *= 1.25f;
                        if (p.level == 2) cs.BasePower *= 1.35f;
                    }
                    break;
                default:
                    break;
            }
        });

        oppTeam.ForEach(p =>
        {
            switch (p.type)
            {
                case SailorType.HORROR:
                    if (p.level == 1) cs.BaseArmor -= 12;
                    if (p.level == 2) cs.BaseArmor -= 20;
                    break;
                default:
                    break;
            }
        });
    }
    public bool HaveStatus(SailorStatusType name)
    {
        return cs.listStatus.Find(x => x.name == name) != null;
    }
    public int GetStatusRemainTurn(SailorStatusType name)
    {
        SailorStatus status = cs.listStatus.Find(x => x.name == name);
        if (status == null) return 0;
        return status.remainTurn;
    }
    public void CountdownStatusRemain()
    {
        SailorStatusType[] listName = new SailorStatusType[]
        {
            SailorStatusType.FROZEN,
        };
        foreach (SailorStatusType statusName in listName)
        {
            SailorStatus status = cs.listStatus.Find(status => status.name == statusName);
            if (status != null) status.remainTurn -= 1;
        }
        cs.listStatus.RemoveAll(status => status.remainTurn <= 0);
    }
    public bool IsDeath()
    {
        return HaveStatus(SailorStatusType.DEATH);
    }

    public float DoCombatAction(CombatState combatState)
    {
        bool useSkillCondition = UseSkillCondition(combatState);
        if (HaveStatus(SailorStatusType.FROZEN)) return Immobile();
        else if(useSkillCondition) return UseSkill(combatState);
        else return BaseAttack(combatState);
    }
    bool UseSkillCondition (CombatState combatState)
    {
        return skill != null && cs.Fury >= cs.MaxFury && skill.CanActive(this, combatState);
    }
    float UseSkill (CombatState combatState)
    {
        GameEvents.instance.castSkill.Invoke(this, skill);
        cs.CurrentSpeed -= cs.MaxSpeed;
        cs.Fury = 0;
        Debug.Log("Use skill now " + skill.name);
        bar.SetSpeedBar(cs.MaxSpeed, cs.CurrentSpeed);
        bar.SetFuryBar(cs.MaxFury, cs.Fury);
        return skill.CastSkill(this, combatState);
    }
    float BaseAttack (CombatState combatState)
    {
        cs.CurrentSpeed -= cs.MaxSpeed;
        bar.SetSpeedBar(cs.MaxSpeed, cs.CurrentSpeed);
        GainFury(10);
        Sailor target = GetBaseAttackTarget(combatState);
        float delay = 0;
        if (target != null)
        {
            delay += RunBaseAttack(target);
            StartCoroutine(DealBaseAttackDamageDelay(target, cs.Power, delay));
        }
        GameEvents.instance.attackOneTarget.Invoke(this, target);
        return delay + 0.5f;
    }
    float Immobile ()
    {
        cs.CurrentSpeed -= cs.MaxSpeed;
        bar.SetSpeedBar(cs.MaxSpeed, cs.CurrentSpeed);
        CountdownStatusRemain();
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition("Immobile", transform.position);

        DisplayStatus(cs.listStatus);
        return RunImmobile() + 0.2f;
    }
    Sailor GetBaseAttackTarget(CombatState combatState)
    {
        switch (config_stats.attack_type)
        {
            case AttackType.RANGE:
                return GetNearestInRowTarget(
                cs.team == Team.A
                    ? combatState.GetAllTeamAliveCharacter(Team.B)
                    : combatState.GetAllTeamAliveCharacter(Team.A));
            case AttackType.ASSASSINATE:
                return GetFurthestTarget(
                    cs.team == Team.A
                    ? combatState.GetAllTeamAliveCharacter(Team.B)
                    : combatState.GetAllTeamAliveCharacter(Team.A));
            default:
                return GetNearestTarget(
                cs.team == Team.A
                    ? combatState.GetAllTeamAliveCharacter(Team.B)
                    : combatState.GetAllTeamAliveCharacter(Team.A));
        }
    }
    void DealBaseAttackDamage(Sailor target, float physicsDamage)
    {

        target.TakeDamage(physicsDamage, 0, 0);
    }
    IEnumerator DealBaseAttackDamageDelay(Sailor target, float current_power, float delay)
    {
        yield return new WaitForSeconds(delay);
        DealBaseAttackDamage(target, current_power);
    }
    public virtual float TakeDamage(float physicsDamage = 0, float magicDamage = 0, float trueDamage = 0)
    {
        float physicTake, magicTake;
        if (cs.Armor > 0) physicTake = physicsDamage * 100 / (100 + cs.Armor);
        else physicTake = physicsDamage * (2 - 100 / (100 - cs.Armor));
        if (cs.MagicResist > 0) magicTake = magicDamage * 100 / (100 + cs.MagicResist);
        else magicTake = magicDamage * (2 - 100 / (100 - cs.MagicResist));
        float totalDamage = physicTake + magicTake + trueDamage;
        LoseHealth(totalDamage);
        GainFury(4);
        return totalDamage;
    }
    public void AddStatus (SailorStatus status)
    {
        SailorStatus existStatus = cs.listStatus.Find(x => x.name == status.name);
        if (existStatus != null)
        {
            if (status.remainTurn > existStatus.remainTurn)
            {
                existStatus.remainTurn = status.remainTurn;
            }
        }
        else
        {
            cs.listStatus.Add(status);
        }
        DisplayStatus(cs.listStatus);
    }
    public void InitDisplayStatus()
    {
        UnityEngine.Vector3 p = GameObject.Find(cs.team == Team.A ? "FieldA" : "FieldB").transform.Find("slot_A" + cs.position.x + cs.position.y).transform.position;
        transform.position = p;
        Debug.Log("max_health " + cs.MaxHealth + " " + cs.CurHealth);
        bar.SetHealthBar(cs.MaxHealth, cs.CurHealth);
        bar.SetSpeedBar(cs.MaxSpeed, cs.CurrentSpeed);
        bar.SetFuryBar(cs.MaxFury, cs.Fury);
        bar.SetIconType(config_stats.attack_type);
        bar.SetIconSkill(skill);
        bar.SetName(charName);
        SetFaceDirection();
    }
    public void GainHealth(float health)
    {
        cs.CurHealth += health;
        if (cs.CurHealth > cs.MaxHealth) cs.CurHealth = cs.MaxHealth;
        bar.SetHealthBar(cs.MaxHealth, cs.CurHealth);
    }
    public void LoseHealth(float health)
    {
        cs.CurHealth -= health;
        if (cs.CurHealth <= 0)
        {
            cs.CurHealth = 0;
            AddStatus(new SailorStatus(SailorStatusType.DEATH));
            if (IsDeath()) RunDeath();
        }
        bar.SetHealthBar(cs.MaxHealth, cs.CurHealth);
        GameEvents.instance.takeDamage.Invoke(this, health);
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition("-" + (int)health, transform.position);
    }
    public void GainFury(int value)
    {
        if (skill != null)
        {
            cs.Fury += value;
            if (cs.Fury > cs.MaxFury) cs.Fury = cs.MaxFury;
            bar.SetFuryBar(cs.MaxFury, cs.Fury);
        }
    }
    public int GetSpeedNeeded()
    {
        return cs.MaxSpeed - cs.CurrentSpeed;
    }
    public virtual void AddSpeed(int speedAdd)
    {
        cs.CurrentSpeed += speedAdd;
        bar.SetSpeedBar(cs.MaxSpeed, cs.CurrentSpeed);
    }
    public void IncDisplaySpeed(float spInc)
    {
        cs.DisplaySpeed += spInc;

        bar.SetSpeedBar(cs.MaxSpeed, cs.CurrentSpeed);
    }
    public bool IsEnoughSpeed()
    {
        return GetSpeedNeeded() <= 0;
    }
    // target
    Sailor GetNearestTarget(List<Sailor> listTarget)
    {
        Sailor result = null;
        int myRow = cs.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        listTarget.ForEach(delegate (Sailor character)
        {
            int col = character.cs.position.x;
            int row = character.cs.position.y;
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

    Sailor GetNearestInRowTarget(List<Sailor> listTarget)
    {
        Sailor result = null;
        int myRow = cs.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        listTarget.ForEach(delegate (Sailor character)
        {
            int col = character.cs.position.x;
            int row = character.cs.position.y;
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
    Sailor GetFurthestTarget(List<Sailor> listTarget)
    {
        Sailor result = null;
        int myRow = cs.position.y;
        int NR_col = 9999;
        int NR_row = 9999;
        listTarget.ForEach(delegate (Sailor character)
        {
            int col = character.cs.position.x;
            int row = character.cs.position.y;
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

    // animation
    public GameObject modelObject;

    private GameObject iceBlock = null;
    public void TriggerAnimation(string trigger)
    {
        modelObject.GetComponent<Animator>().SetTrigger(trigger);
    }
    public virtual float RunImmobile()
    {
        float oriX = transform.position.x;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveX(oriX - 0.2f, 0.05f));
        seq.Append(transform.DOMoveX(oriX + 0.2f, 0.05f));
        seq.Append(transform.DOMoveX(oriX, 0.05f));
        return 0.15f;
    }
    public virtual float RunBaseAttack(Sailor target) { return 0f; }
    public virtual float RunSkill(Sailor target) { return 0f; }
    public virtual float RunSkill(List<Sailor> targets) { return 0f; }
    public virtual float RunDeath()
    {
        TriggerAnimation("Death");
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.6f);
        seq.AppendCallback(() => { gameObject.SetActive(false); });
        return 0f;
    }
    public void DisplayStatus(List<SailorStatus> listStatus)
    {
        ShowInIce(listStatus.Find(x => x.name == SailorStatusType.FROZEN) != null);
    }
    public void ShowInIce(bool b)
    {
        if (b)
            if (iceBlock == null)
            {
                iceBlock = Instantiate(Resources.Load<GameObject>("GameComponents/IceBlock"), transform);
            }
            else iceBlock.SetActive(b);
        else if (iceBlock != null) iceBlock.SetActive(b);
    }
    public virtual void SetFaceDirection()
    {
        int scaleX = cs.team == Team.A ? -1 : 1;
        float scale = modelObject.transform.localScale.x;
        modelObject.transform.localScale = new Vector3(scale * scaleX, scale, scale);
    }
};