using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using Random = UnityEngine.Random;

public class Damage
{
    public float physics_damage = 0;
    public float magic_damage = 0;
    public float true_damage = 0;
    public int fury_gain = 0;
    public bool isCrit = false;
    public bool isBlock = false;
    public float total
    {
        get { return physics_damage + magic_damage + true_damage; }
    }
}

public class CombatSailor : Sailor
{
    public CombatStats cs;

    public CharBarControl bar;
    public virtual void Awake()
    {
    }
    public void CreateStatusBar()
    {
        var barPrefab = Resources.Load<GameObject>("characters/Bar/Bar2");
        var barGO = Instantiate(
            barPrefab,
            transform.Find("nodeBar"));
        barGO.transform.localScale = new Vector3(0.024f, 0.024f, 1f);
        barGO.transform.localPosition = new Vector3(0, 0, 0);
        bar = barGO.transform.GetComponent<CharBarControl>();
    }
    public void InitCombatData(CombatPosition p, Team t)
    {
        int level = Model.level;
        int quality = Model.quality;
        cs = new CombatStats()
        {
            BasePower = Model.config_stats.GetPower(level, quality),
            MaxHealth = Model.config_stats.GetHealth(level, quality),
            CurHealth = Model.config_stats.GetHealth(level, quality),
            BaseArmor = Model.config_stats.GetArmor(),
            BaseMagicResist = Model.config_stats.GetMagicResist(),
            Speed = Model.config_stats.GetSpeed(level, quality),
            CurrentSpeed = 0,
            Crit = Model.config_stats.GetCrit(),
            CritDamage = GlobalConfigs.Combat.base_crit_damage,
            Block = Model.config_stats.GetBlock(),
            BaseFury = Model.config_stats.max_fury,
            Fury = Model.config_stats.start_fury,
            position = p,
            team = t,
        };
        foreach (SailorClass type in Model.config_stats.classes)
        {
            cs.types.Add(type);
        }

        if (Model.items != null) Model.items.ForEach(item =>
        {
            cs.BasePower += item.Power;
            cs.MaxHealth += item.Health;
            cs.BaseArmor += item.Armor;
            cs.BaseMagicResist += item.MagicResist;
            cs.Speed += item.Speed;
            cs.Crit += item.Crit;
            if (item.class_buff != SailorClass.NONE) cs.types.Add(item.class_buff);
        });

        Model.level = level;
        Model.quality = quality;

        Debug.Log("-----------------------------------");
        Debug.Log(" > Model.id" + Model.id);
        Debug.Log(" > BasePower" + cs.BasePower);
        Debug.Log(" > MaxHealth" + cs.MaxHealth);
        Debug.Log(" > CurHealth" + cs.CurHealth);
        Debug.Log(" > BaseArmor" + cs.BaseArmor);
        Debug.Log(" > BaseMagicResist" + cs.BaseMagicResist);
        Debug.Log(" > Speed" + cs.Speed);
        Debug.Log(" > CurrentSpeed" + cs.CurrentSpeed);
        Debug.Log(" > Crit" + cs.Crit);
        Debug.Log(" > CritDamage" + cs.CritDamage);
        Debug.Log(" > Block" + cs.Block);
        Debug.Log(" > BaseFury" + cs.BaseFury);
        Debug.Log(" > Fury" + cs.Fury);
        Debug.Log(" > Model.level" + Model.level);
        Debug.Log(" > Model.quality" + Model.quality);
        Debug.Log("-----------------------------------");
    }
    // them giam chi so theo toc he
    public void UpdateCombatData(List<ClassBonusItem> ownTeam, List<ClassBonusItem> oppTeam)
    {
        ContainerClassBonus config = GlobalConfigs.ClassBonus;
        ownTeam.ForEach(p =>
        {
            switch (p.type)
            {
                case SailorClass.MIGHTY:
                    if (cs.HaveType(SailorClass.MIGHTY))
                    {
                        cs.MaxHealth *= 1 + config.GetParams(p.type, p.level)[0];
                        cs.CurHealth = cs.MaxHealth;
                    }
                    break;
                case SailorClass.SWORD_MAN:
                    if (cs.HaveType(SailorClass.SWORD_MAN))
                    {
                        cs.Speed += config.GetParams(p.type, p.level)[0];
                    }
                    break;
                case SailorClass.MAGE:
                    if (cs.HaveType(SailorClass.MAGE))
                    {
                        cs.BasePower *= 1 + config.GetParams(p.type, p.level)[0];
                    }
                    break;
                case SailorClass.ASSASSIN:
                    if (cs.HaveType(SailorClass.ASSASSIN))
                    {
                        cs.BasePower *= 1 + config.GetParams(p.type, p.level)[0];
                    }
                    break;
                case SailorClass.SEA_CREATURE:
                    cs.BaseMagicResist += config.GetParams(p.type, p.level)[0];
                    break;
                case SailorClass.SUPPORT:
                    cs.Fury += (int) config.GetParams(p.type, p.level)[0];
                    break;
                case SailorClass.KNIGHT:
                    if (cs.HaveType(SailorClass.KNIGHT))
                    {
                        cs.BaseArmor += (int)config.GetParams(p.type, p.level)[0];
                    }
                    break;
            }
        });

        oppTeam.ForEach(p =>
        {
            switch (p.type)
            {
                case SailorClass.DEMON:
                    cs.BaseArmor -= config.GetParams(p.type, p.level)[0];
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
            SailorStatusType.STUN,
        };
        foreach (SailorStatusType statusName in listName)
        {
            SailorStatus status = cs.listStatus.Find(status => status.name == statusName);
            if (status != null) status.remainTurn -= 1;
        }
        cs.listStatus.RemoveAll(status => status.remainTurn <= 0);
    }
    public void CheckDeath()
    {
        if (IsDeath()) return;
        if (cs.CurHealth <= 0)
        {
            AddStatus(new SailorStatus(SailorStatusType.DEATH));
            if (IsDeath()) RunDeath();
        }
    }
    public bool IsDeath()
    {
        return HaveStatus(SailorStatusType.DEATH);
    }
        
    public float DoCombatAction(CombatState combatState)
    {
        bool useSkillCondition = UseSkillCondition(combatState);
        if (HaveStatus(SailorStatusType.STUN)) return Immobile();
        else if(useSkillCondition) return UseSkill(combatState);
        else return BaseAttack(combatState);
    }
    // Base attack
    // ... client tính
    float BaseAttack(CombatState combatState)
    {
        // client auto
        CombatSailor target = GetBaseAttackTarget(combatState);
        bool isCrit = IsCrit();
        bool isBlock = target.IsBlock();
        return BaseAttack(target, isCrit, isBlock);
    }
    // ... server trả 
    public float BaseAttack(CombatSailor target, bool isCrit, bool isBlock)
    {
        var combatState = CombatState.Instance;
        cs.CurrentSpeed -= cs.SpeedNeed;
        bar.SetSpeedBar(cs.SpeedNeed, cs.CurrentSpeed);
        float delay = 0;

        // Calc Class Active
        ContainerClassBonus config = GlobalConfigs.ClassBonus;
        bool activeMarksman = false;
        if (cs.HaveType(SailorClass.MARKSMAN)
            && combatState.GetTeamClassBonus(cs.team, SailorClass.MARKSMAN) != null)
        {
            ClassBonusItem marksman = combatState.GetTeamClassBonus(cs.team, SailorClass.MARKSMAN);
            if (marksman != null)
            {
                float healthRatio = config.GetParams(marksman.type, marksman.level)[0];
                if (target.cs.GetCurrentHealthRatio() < healthRatio) activeMarksman = true;
                CombatEvents.Instance.activeClassBonus.Invoke(this, SailorClass.MARKSMAN, new List<float>());
            }
        }
        if (cs.HaveType(SailorClass.WILD))
        {
            ClassBonusItem wild = combatState.GetTeamClassBonus(cs.team, SailorClass.CYBORG);
            if (wild != null)
            {
                float percentHealthGain = config.GetParams(wild.type, wild.level)[0];
                float healthGain = percentHealthGain * cs.MaxHealth;
                GainHealth(healthGain);
                CombatEvents.Instance.activeClassBonus.Invoke(this, SailorClass.CYBORG, new List<float> { healthGain });
            }
        }
        if (cs.HaveType(SailorClass.CYBORG))
        {
            ClassBonusItem berserk = combatState.GetTeamClassBonus(cs.team, SailorClass.CYBORG);
            if (berserk != null)
            {
                int furyAdd = (int) config.GetParams(berserk.type, berserk.level)[0];
                GainFury(furyAdd);
                CombatEvents.Instance.activeClassBonus.Invoke(this, SailorClass.CYBORG, new List<float> { furyAdd });
            }
        }
        // Deal damage
        if (target != null)
        {
            delay += RunBaseAttack(target);
            float d = cs.Power;
            if (isCrit) d *= GlobalConfigs.Combat.base_crit_damage;
            if (activeMarksman) d *= 1.5f; // 1.5 hardcode
            Damage damage = new Damage()
            {
                physics_damage = d,
                isCrit = isCrit,
                isBlock = isBlock,
                fury_gain = GlobalConfigs.Combat.fury_per_take_damage,
            };
            StartCoroutine(DealBaseAttackDamageDelay(target, damage, delay));
        }
        return delay + 0.5f;
    }
    
    bool UseSkillCondition (CombatState combatState)
    {
        return cs.Fury >= cs.MaxFury && CanActiveSkill(combatState);
    }
    public virtual float UseSkill (CombatState combatState)
    {
        cs.CurrentSpeed -= cs.SpeedNeed;
        cs.Fury = 0;
        //Debug.Log("Use skill now " + skill.name);
        bar.SetSpeedBar(cs.SpeedNeed, cs.CurrentSpeed);
        bar.SetFuryBar(cs.MaxFury, cs.Fury);
        return CastSkill(combatState);
    }
    float Immobile ()
    {
        cs.CurrentSpeed -= cs.SpeedNeed;
        bar.SetSpeedBar(cs.SpeedNeed, cs.CurrentSpeed);
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition("Immobile", transform.position);

        DisplayStatus(cs.listStatus);
        return RunImmobile() + 0.2f;
    }
    bool IsCrit()
    {
        float r = Random.Range(0f, 1f);
        return r < cs.Crit;
    }
    bool IsBlock()
    {
        float r = Random.Range(0f, 1f);
        return r < cs.Block;
    }
   CombatSailor GetBaseAttackTarget(CombatState combatState)
    {
        switch (Model.config_stats.attack_type)
        {
            case AttackType.RANGE:
                return TargetsUtils.Range(this,
                    cs.team == Team.A
                    ? combatState.GetAllTeamAliveSailors(Team.B)
                    : combatState.GetAllTeamAliveSailors(Team.A));
            case AttackType.BACKSTAB:
                return TargetsUtils.Backstab(this,
                    cs.team == Team.A
                    ? combatState.GetAllTeamAliveSailors(Team.B)
                    : combatState.GetAllTeamAliveSailors(Team.A));
            default:
                return TargetsUtils.Melee(this,
                cs.team == Team.A
                    ? combatState.GetAllTeamAliveSailors(Team.B)
                    : combatState.GetAllTeamAliveSailors(Team.A));
        }
    }
    IEnumerator DealBaseAttackDamageDelay(CombatSailor target, Damage damage, float delay)
    {
        yield return new WaitForSeconds(delay);
        GainFury(GlobalConfigs.Combat.fury_per_base_attack);
        float damageDeal = target.TakeDamage(damage);
    }
    public virtual float TakeDamage(Damage d)
    {
        float physicTake, magicTake, totalDamage;
        if (d.isBlock) physicTake = magicTake = totalDamage = 0;
        else
        {
            if (cs.Armor > 0) physicTake = d.physics_damage * 100 / (100 + cs.Armor);
            else physicTake = d.physics_damage * (2 - 100 / (100 - cs.Armor));
            if (cs.MagicResist > 0) magicTake = d.magic_damage * 100 / (100 + cs.MagicResist);
            else magicTake = d.magic_damage * (2 - 100 / (100 - cs.MagicResist));
            totalDamage = physicTake + magicTake + d.true_damage;
        }
        LoseHealth(new Damage()
        {
            physics_damage = physicTake,
            magic_damage = magicTake,
            true_damage = d.true_damage,
            fury_gain = d.fury_gain,
            isCrit = d.isCrit,
            isBlock = d.isBlock,
        });
        GainFury(d.fury_gain);
        return totalDamage;
    }
    public virtual float TakeDamage(float physics_damage = 0, float magic_damage = 0, float true_damage = 0, int fury_gain = 0, bool isCrit = false)
    {
        return TakeDamage(new Damage()
        {
            physics_damage = physics_damage,
            magic_damage = magic_damage,
            true_damage = true_damage,
            fury_gain = fury_gain,
            isCrit = isCrit,
        });
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
        bar.SetHealthBar(cs.MaxHealth, cs.CurHealth);
        bar.SetSpeedBar(cs.SpeedNeed, cs.CurrentSpeed);
        bar.SetFuryBar(cs.MaxFury, cs.Fury);
        bar.SetIconType(Model.config_stats.attack_type);
        bar.SetIconSkill(Model.config_stats.skill_name);
        bar.SetName(Model.config_stats.root_name);
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
        if (cs.CurHealth <= 0) cs.CurHealth = 0;
        bar.SetHealthBar(cs.MaxHealth, cs.CurHealth);
        CheckDeath();

        CombatEvents.Instance.takeDamage.Invoke(this, new Damage() { physics_damage = health });
    }
    public void LoseHealth(Damage d)
    {
        cs.CurHealth -= d.physics_damage + d.magic_damage + d.true_damage;
        if (cs.CurHealth <= 0) cs.CurHealth = 0;
        bar.SetHealthBar(cs.MaxHealth, cs.CurHealth);
        CheckDeath();

        CombatEvents.Instance.takeDamage.Invoke(this, d);
    }

    public virtual void GainFury(int value)
    {
        cs.Fury += value;
        if (cs.Fury > cs.MaxFury) cs.Fury = cs.MaxFury;
        bar.SetFuryBar(cs.MaxFury, cs.Fury);
    }
    public int GetSpeedNeeded()
    {
        return cs.SpeedNeed - cs.CurrentSpeed;
    }
    public virtual void SpeedUp(int speedAdd)
    {
        cs.CurrentSpeed += speedAdd;
        bar.SetSpeedBar(cs.SpeedNeed, cs.CurrentSpeed);
    }
    public void IncDisplaySpeed(float spInc)
    {
        cs.Speed += spInc;

        bar.SetSpeedBar(cs.SpeedNeed, cs.CurrentSpeed);
    }
    public bool IsEnoughSpeed()
    {
        return GetSpeedNeeded() <= 0;
    }

    // animation

    private GameObject iceBlock = null;
    public virtual float RunImmobile()
    {
        float oriX = transform.position.x;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveX(oriX - 0.2f, 0.05f));
        seq.Append(transform.DOMoveX(oriX + 0.2f, 0.05f));
        seq.Append(transform.DOMoveX(oriX, 0.05f));
        return 0.15f;
    }
    public virtual float RunBaseAttack(CombatSailor target) { return 0f; }
    public virtual float RunSkill(CombatSailor target) { return 0f; }
    public virtual float RunSkill(List<CombatSailor> targets) { return 0f; }
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
        ShowInIce(listStatus.Find(x => x.name == SailorStatusType.STUN) != null);
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

    //skill
    public virtual float CastSkill(CombatState cbState)
    {
        string skill_name = Model.config_stats.skill_name;
        Debug.Log(">>>>>>>>>>>>>>>" + Model.config_stats.root_name + " active " + skill_name);
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition(skill_name, transform.position);
        return 0.5f;
    }
    public virtual bool CanActiveSkill(CombatState cbState)
    {
        return true;
    }
};
