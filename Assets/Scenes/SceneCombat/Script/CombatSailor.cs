using DG.Tweening;
using Piratera.Config;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Damage
{
    public float physics = 0;
    public float magic = 0;
    public float pure = 0;
    public int fury_gain = 0;
    public bool isCrit = false;
    public float total
    {
        get { return physics + magic + pure; }
    }
}

public class CombatSailor : Sailor
{
    public CombatStats cs;

    public CharBarControl bar;
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
        cs = new CombatStats(Model);
        cs.position = p;
        cs.team = t;
    }
    // them giam chi so theo toc he
    public void UpdateCombatData(List<ClassBonusItem> ownTeam, List<ClassBonusItem> oppTeam)
    {
        cs.UpdateStatsWithSynergy(ownTeam, oppTeam);
        bar.SetFuryBar(cs.MaxFury, cs.Fury);
        /*
        Debug.Log("-----------------------------------" +
        "\n > Model.id: " + Model.id +
        "\n > Name: " + Model.name +
        "\n > Power: " + cs.BasePower +
        "\n > Health: " + cs.MaxHealth +
        "\n > Armor: " + cs.BaseArmor +
        "\n > MagicResist: " + cs.BaseMagicResist +
        "\n > Speed: " + cs.Speed +
        "\n > CurrentSpeed: " + cs.CurrentSpeed +
        "\n > Crit: " + cs.Crit +
        "\n > CritDamage: " + cs.CritDamage +
        "\n > Block: " + cs.Block +
        "\n > MaxFury: " + cs.BaseFury +
        "\n > Fury: " + cs.Fury +
        "\n > level: " + Model.level +
        "\n > quality: " + Model.quality
        );
        */
    }

    public virtual void ActiveStartPassive()
    {
        // do nothing, se say ra o 1 so tuong skill passive
    }
    public bool HaveStatus(SailorStatusType name)
    {
        return cs.listStatus.Find(x => x.name == name) != null;
    }
    public void SyncStatus(List<SailorStatus> statuses)
    {
        // update sau, lam cho death truoc
        statuses.ForEach(status =>
        {
            var clientStatus = cs.listStatus.Find(x => x.name == status.name);
            Debug.Log("Check Status: " + status.name);
            if (clientStatus == null)
            {
                Debug.LogWarning(Model.config_stats.root_name + " missing Status " + status.name);
                AddStatus(status);
                switch (status.name)
                {
                    case SailorStatusType.DEATH:
                        cs.CurHealth = 0;
                        gameObject.SetActive(false);
                        break;
                }
            }
            else
            {
                if (clientStatus.name != SailorStatusType.DEATH && clientStatus.stack != status.stack)
                {
                    Debug.LogWarning("Client status stack wrong: "
                        + " " + Model.config_stats.root_name
                        + " " + clientStatus.name.ToString()
                        + "\n client | server: " + clientStatus.stack + " | " + status.stack); ;
                }
                clientStatus.stack = status.stack;
            }
        });

        cs.listStatus.ForEach(status =>
        {
            var serverStatus = statuses.Find(x => x.name == status.name);
            if (serverStatus == null)
            {
                Debug.LogWarning("Client status wrong: " + status.name);
                RemoveStatus(status.name);
            }
        });
        DisplayStatus();
    }
    public int GetStatusRemainTurn(SailorStatusType name)
    {
        SailorStatus status = cs.listStatus.Find(x => x.name == name);
        if (status == null) return 0;
        return status.stack;
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
            if (status != null) status.stack -= 1;
        }
        cs.listStatus.RemoveAll(status => status.stack <= 0);
        DisplayStatus();
    }
    public void CheckDeath()
    {
        if (IsDeath()) return;
        //Debug.Log("cs.CurHealth: " + cs.CurHealth);
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
        float delay = 0;
        bool useSkillCondition = UseSkillCondition(combatState);
        if (HaveStatus(SailorStatusType.STUN)) delay = Immobile();
        else if (useSkillCondition) delay = CastSkill(combatState);
        else delay = BaseAttack(combatState);
        return delay;
    }
    // Base attack
    // ... client tính
    float BaseAttack(CombatState combatState)
    {
        // client auto
        CombatSailor target = GetBaseAttackTarget(combatState);
        bool isCrit = IsCrit();
        // class bonus
        SynergiesConfig config = GlobalConfigs.Synergies;
        float healthGain = 0;
        ClassBonusItem wild = combatState.GetTeamClassBonus(cs.team, SailorClass.WILD);
        if (cs.HaveType(SailorClass.WILD) && wild != null)
        {
            float percentHealthGain = config.GetParams(wild.type, wild.level)[0];
            healthGain = percentHealthGain * cs.MaxHealth;
            CombatEvents.Instance.activeClassBonus.Invoke(this, SailorClass.WILD, new List<float> { healthGain });
        }
        // Deal damage
        float d = cs.Power;
        if (isCrit) d *= cs.CritDamage;
        Damage damage = new Damage()
        {
            physics = cs.HaveType(SailorClass.MAGE) ? 0 : d,
            magic = cs.HaveType(SailorClass.MAGE) ? d : 0,
            isCrit = isCrit,
            fury_gain = GlobalConfigs.Combat.fury_per_take_damage,
        };
        float damageTake = target.CalcDamageTake(damage, this);
        return BaseAttack(target, isCrit, damageTake, healthGain);
    }
    // ... server trả 
    public float BaseAttack(CombatSailor target, bool isCrit, float damageDeal, float wildHeal)
    {
        Debug.Log("Base Attack >>>>" + isCrit + " " + damageDeal);
        var combatState = CombatState.Instance;
        cs.CurrentSpeed -= cs.SpeedNeed;
        bar.SetSpeedBar(cs.SpeedNeed, cs.CurrentSpeed);

        // Calc Class Active
        SynergiesConfig config = GlobalConfigs.Synergies;
        ClassBonusItem cyborg = combatState.GetTeamClassBonus(cs.team, SailorClass.CYBORG);
        if (cs.HaveType(SailorClass.CYBORG) && cyborg != null)
        {
            int furyAdd = (int)config.GetParams(cyborg.type, cyborg.level)[0];
            GainFury(furyAdd);
            CombatEvents.Instance.activeClassBonus.Invoke(this, SailorClass.CYBORG, new List<float> { furyAdd });
        }
        target.CheckActiveGrappler();

        // Deal damage
        float delay = RunBaseAttack(target);
        Damage damage = new Damage()
        {
            physics = cs.HaveType(SailorClass.MAGE) ? 0 : damageDeal,
            magic = cs.HaveType(SailorClass.MAGE) ? damageDeal : 0,
            isCrit = isCrit,
            fury_gain = GlobalConfigs.Combat.fury_per_take_damage,
        };
        StartCoroutine(WaitAndDo(delay, () =>
        {
            GainHealth(wildHeal);
            GainFury(GlobalConfigs.Combat.fury_per_base_attack);
            target.LoseHealth(damage);
        }));
        return delay + 0.8f;
    }
    public void CheckActiveGrappler()
    {
        SynergiesConfig config = GlobalConfigs.Synergies;
        ClassBonusItem grappler = CombatState.Instance.GetTeamClassBonus(cs.team, SailorClass.GRAPPLER);
        if (cs.HaveType(SailorClass.GRAPPLER) && grappler != null)
        {
            float speedUpPercent = config.GetParams(SailorClass.GRAPPLER, 2)[0];
            SpeedUp(speedUpPercent);
            CombatEvents.Instance.activeClassBonus.Invoke(this, SailorClass.GRAPPLER, new List<float> { speedUpPercent });
        }
    }
    IEnumerator WaitAndDo(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }
    bool UseSkillCondition(CombatState combatState)
    {
        return cs.Fury >= cs.MaxFury && CanActiveSkill(combatState);
    }
    float Immobile()
    {
        return RunImmobile() + 0.2f;
    }
    protected bool IsCrit()
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
    public virtual float TakeDamage(Damage d)
    {
        float physicTake, magicTake, totalDamage;

        if (cs.Armor > 0) physicTake = d.physics * 100 / (100 + cs.Armor);
        else physicTake = d.physics * (2 - 100 / (100 - cs.Armor));
        if (cs.MagicResist > 0) magicTake = d.magic * 100 / (100 + cs.MagicResist);
        else magicTake = d.magic * (2 - 100 / (100 - cs.MagicResist));
        totalDamage = physicTake + magicTake + d.pure;

        LoseHealth(new Damage()
        {
            physics = physicTake,
            magic = magicTake,
            pure = d.pure,
            fury_gain = d.fury_gain,
            isCrit = d.isCrit,
        });
        return totalDamage;
    }
    public virtual float TakeDamage(float physics_damage = 0, float magic_damage = 0, float true_damage = 0, int fury_gain = 0, bool isCrit = false)
    {
        return TakeDamage(new Damage()
        {
            physics = physics_damage,
            magic = magic_damage,
            pure = true_damage,
            fury_gain = fury_gain,
            isCrit = isCrit,
        });
    }
    public float CalcDamageTake(Damage d, CombatSailor actor)
    {
        float damage = CalcDamageTake(d);
        var state = CombatState.Instance;
        SynergiesConfig config = GlobalConfigs.Synergies;
        ClassBonusItem marksman = state.GetTeamClassBonus(actor.cs.team, SailorClass.MARKSMAN);
        if (actor.cs.HaveType(SailorClass.MARKSMAN) && marksman != null)
        {
            float bonusDamagePerTile = config.GetParams(marksman.type, marksman.level)[0];
            damage += damage * bonusDamagePerTile * (actor.cs.position.x + cs.position.x + 1);
        }
        ClassBonusItem criminal = state.GetTeamClassBonus(actor.cs.team, SailorClass.CRIMINAL);
        if (actor.cs.HaveType(SailorClass.CRIMINAL) && criminal != null)
        {
            float bonusUnit = config.GetParams(criminal.type, criminal.level)[0];
            float percentLoseHealth = 1 - cs.CurHealth / cs.MaxHealth;
            float bonusPercent = percentLoseHealth * bonusUnit;
            damage *= 1 + bonusPercent;
        }
        return damage;
    }
    public float CalcDamageTake(Damage d)
    {
        float physicTake, magicTake;

        if (cs.Armor > 0) physicTake = d.physics * 100 / (100 + cs.Armor);
        else physicTake = d.physics * (2 - 100 / (100 - cs.Armor));
        if (cs.MagicResist > 0) magicTake = d.magic * 100 / (100 + cs.MagicResist);
        else magicTake = d.magic * (2 - 100 / (100 - cs.MagicResist));

        return physicTake + magicTake + d.pure;
    }
    public void AddStatus(SailorStatus status)
    {
        SailorStatusType[] statusStacks = {
            SailorStatusType.EXCITED
        };
        SailorStatusType[] statusTurn = {
            SailorStatusType.STUN
        };
        SailorStatusType[] noStack = {
            SailorStatusType.DEATH,
            SailorStatusType.EPIDEMIC,
        };
        SailorStatus existStatus = cs.GetStatus(status.name);
        if (existStatus != null)
        {
            if (statusTurn.Contains(status.name) && status.stack > existStatus.stack) existStatus.stack = status.stack;
            if (statusStacks.Contains(status.name)) existStatus.stack += status.stack;
        }
        else cs.listStatus.Add(status);
        DisplayStatus();
    }
    public void RemoveStatus(SailorStatusType name)
    {
        cs.listStatus = cs.listStatus.Where(status => status.name != name).ToList();
    }
    public void InitDisplayStatus()
    {
        transform.position = GetScenePosition();
        bar.Init(Model);
        bar.SetHealthBar(cs.MaxHealth, cs.CurHealth);
        bar.SetSpeedBar(cs.SpeedNeed, cs.CurrentSpeed);
        bar.SetFuryBar(cs.MaxFury, cs.Fury);
        bar.SetIconType(Model.config_stats.attack_type);
        bar.SetName(Model.config_stats.root_name);
        SetFaceDirection();
    }
    public Vector3 GetScenePosition()
    {
        var p = cs.position;
        return GameObject.Find(cs.team == Team.A ? "FieldA" : "FieldB").transform.Find("slot_A" + p.x + p.y).transform.position;
    }
    public void GainHealth(float health)
    {
        SynergiesConfig config = GlobalConfigs.Synergies;
        ClassBonusItem support = CombatState.Instance.GetTeamClassBonus(cs.team, SailorClass.SUPPORT);
        if (support != null)
        {
            float extra_health_percent = config.GetParams(support.type, support.level)[1];
            health += health * extra_health_percent;
        }

        cs.CurHealth += health;
        if (cs.CurHealth > cs.MaxHealth) cs.CurHealth = cs.MaxHealth;
        bar.SetHealthBar(cs.MaxHealth, cs.CurHealth);
        CombatEvents.Instance.gainHealth.Invoke(this, health);
    }
    public void GainArmor(float armor)
    {
        cs.BaseArmor += armor;
    }
    public void LoseHealth(Damage d, bool checkDeath = true)
    {
        TriggerAnimation("Hurt");
        GainFury(d.fury_gain);

        cs.CurHealth -= d.physics + d.magic + d.pure;
        if (cs.CurHealth <= 0) cs.CurHealth = 0;
        bar.SetHealthBar(cs.MaxHealth, cs.CurHealth);
        if (checkDeath) CheckDeath();

        CombatEvents.Instance.takeDamage.Invoke(this, d);
    }

    public virtual void GainFury(int value)
    {
        cs.Fury += value;
        if (cs.Fury > cs.MaxFury) cs.Fury = cs.MaxFury;
        bar.SetFuryBar(cs.MaxFury, cs.Fury);
    }
    public virtual void LoseFury(int value)
    {
        cs.Fury -= value;
        if (cs.Fury < 0) cs.Fury = 0;
        bar.SetFuryBar(cs.MaxFury, cs.Fury);
    }
    public virtual void GainPower(float value)
    {
        cs.BasePower += value;
    }
    public virtual void GainSpeed(int value)
    {
        cs.Speed += value;
    }
    public int GetSpeedNeeded()
    {
        return cs.SpeedNeed - cs.CurrentSpeed;
    }
    public void AddCurSpeed(int value)
    {
        cs.CurrentSpeed += value;
        bar.SetSpeedBar(cs.SpeedNeed, cs.CurrentSpeed);
    }
    public virtual void SpeedUp(float percent)
    {
        cs.CurrentSpeed += (int)(percent * cs.SpeedNeed);
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

        cs.CurrentSpeed -= cs.SpeedNeed;
        bar.SetSpeedBar(cs.SpeedNeed, cs.CurrentSpeed);
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition("Immobile", transform.position);
        return 0.3f;
    }
    public virtual float RunBaseAttack(CombatSailor target) { return 0f; }
    public virtual float RunSkill(CombatSailor target) { return 0f; }
    public virtual float RunSkill(List<CombatSailor> targets) { return 0f; }
    public virtual float RunDeath()
    {
        // TriggerAnimation("Death");
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() => { gameObject.SetActive(false); });
        return 0f;
    }
    public void DisplayStatus()
    {
        var listStatus = cs.listStatus;
        ShowInStun(listStatus.Find(x => x.name == SailorStatusType.STUN) != null);
        ShowInExcited(listStatus.Find(x => x.name == SailorStatusType.EXCITED));
    }
    public virtual void SetFaceDirection()
    {
        float scale = modelObject.transform.localScale.x;
        if (modelObject.activeSelf) modelObject.transform.localScale = new Vector3(cs.team == Team.A ? scale : -scale, scale, scale);
    }

    //skill
    public virtual float CastSkill(CombatState cbState)
    {
        return 0.0f;
    }
    public virtual float ProcessSkill(List<string> targets = null, List<float> _params = null)
    {
        cs.CurrentSpeed -= cs.SpeedNeed;
        cs.Fury = 0;
        bar.SetSpeedBar(cs.SpeedNeed, cs.CurrentSpeed);
        bar.SetFuryBar(cs.MaxFury, cs.Fury);
        return 0;
    }
    public virtual bool CanActiveSkill(CombatState cbState)
    {
        return true;
    }

    private Color modelColor;
    public virtual void Awake()
    {
        modelColor = Color.white;
    }
    public void DoModelColor(Color color)
    {
        modelColor = color;
    }
    private void LateUpdate()
    {
        var skel = modelObject.GetComponent<SkeletonMecanim>();
        if (!skel) return;
        Spine.Skeleton skeleton = skel.skeleton;
        Color curColor = skeleton.GetColor();
        if (curColor == modelColor) return;
        skeleton.SetColor(Color.Lerp(curColor, modelColor, Mathf.PingPong(Time.time, 0.1f)));
    }
    private GameObject stunEff;
    public void ShowInStun(bool isShow)
    {
        if (isShow)
        {
            if (stunEff == null)
            {
                var prefab = Resources.Load<GameObject>("Effect2D/Duong_FX/status/fx_stun");
                stunEff = Instantiate(prefab, transform.Find("nodeBar"));
                stunEff.transform.localScale = Vector3.one * 1.3f;
            }
            stunEff.SetActive(true);
        }
        else if (stunEff != null) stunEff.SetActive(false);
    }
    private GameObject excitedEff;
    public void ShowInExcited(SailorStatus excited)
    {
        if (excited != null)
        {
            if (excitedEff == null)
            {
                var prefab = Resources.Load<GameObject>("Effect2D/Duong_FX/status/fx_buffdamage_yelow");
                excitedEff = Instantiate(prefab, transform);
            }
            excitedEff.SetActive(true);
            switch (excited.stack)
            {
                case 1:
                    excitedEff.transform.localScale = Vector3.one * 0.7f;
                    break;
                case 2:
                    excitedEff.transform.localScale = Vector3.one * 1.0f;
                    break;
                case 3:
                    excitedEff.transform.localScale = Vector3.one * 1.3f;
                    break;
                default:
                    excitedEff.transform.localScale = Vector3.one * 1.6f;
                    break;
            }
        }
        else if (excitedEff != null) excitedEff.SetActive(false);
    }
};
