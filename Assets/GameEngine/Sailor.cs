using DG.Tweening;
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
    private void Awake()
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
        barGO.transform.localScale = new Vector3(0.018f, 0.018f, 0.018f);
        barGO.transform.localPosition = new Vector3(0, 0, 0);
        return barGO.transform.GetComponent<CharBarControl>();
    }
    public void InitCombatData(int level, int quality, CombatPosition p, Team t)
    {
        cs = new CombatStats()
        {
            base_power = config_stats.GetPower(level, quality),
            current_power = config_stats.GetPower(level, quality),
            max_health = config_stats.GetHealth(level, quality),
            current_health = config_stats.GetHealth(level, quality),
            base_armor = config_stats.GetArmor(),
            current_armor = config_stats.GetArmor(),
            base_magic_resist = config_stats.GetMagicResist(),
            current_magic_resist = config_stats.GetMagicResist(),
            display_speed = config_stats.GetSpeed(level, quality),
            max_speed = (int)(10000f / config_stats.GetSpeed(level, quality)),
            current_speed = 0,
            position = p,
            team = t,
        };

        charName = config_stats.root_name;

        this.level = level;
        this.quality = quality;

        if (skill != null)
        {
            cs.max_fury = skill.MAX_FURY;
            cs.current_fury = skill.START_FURY;
            cs.current_max_fury = skill.MAX_FURY;
        }
        bar = CreateStatusBar();
        InitDisplayStatus();
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
        return skill != null && cs.current_fury >= cs.current_max_fury && skill.CanActive(this, combatState);
    }
    float UseSkill (CombatState combatState)
    {
        cs.current_speed -= cs.max_speed;
        cs.current_fury = 0;
        Debug.Log("Use skill now " + skill.name);
        bar.SetSpeedBar(cs.max_speed, cs.current_speed);
        bar.SetFuryBar(cs.current_max_fury, cs.current_fury);
        return skill.CastSkill(this, combatState);
    }
    float BaseAttack (CombatState combatState)
    {
        cs.current_speed -= cs.max_speed;
        bar.SetSpeedBar(cs.max_speed, cs.current_speed);
        GainFury(10);
        Sailor target = GetBaseAttackTarget(combatState);
        float delay = 0;
        if (target != null)
        {
            delay += RunBaseAttack(target);
            StartCoroutine(DealBaseAttackDamageDelay(target, cs.current_power, delay));
        }
        return delay + 0.2f;
    }
    float Immobile ()
    {
        cs.current_speed -= cs.max_speed;
        bar.SetSpeedBar(cs.max_speed, cs.current_speed);
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
    public float TakeDamage(float physicsDamage, float magicDamage = 0, float trueDamage = 0)
    {
        float physicTake, magicTake;
        if (cs.current_armor > 0) physicTake = physicsDamage * 100 / (100 + cs.current_armor);
        else physicTake = physicsDamage * (2 - 100 / (100 - cs.current_armor));
        if (cs.current_magic_resist > 0) magicTake = magicDamage * 100 / (100 + cs.current_magic_resist);
        else magicTake = magicDamage * (2 - 100 / (100 - cs.current_magic_resist));
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
        UnityEngine.Vector3 p = GameObject.Find("slot_" + (cs.team == Team.A ? "A" : "B") + cs.position.x + cs.position.y).transform.position;
        transform.position = p;
        Debug.Log("max_health " + cs.max_health + " " + cs.current_health);
        bar.SetHealthBar(cs.max_health, cs.current_health);
        bar.SetSpeedBar(cs.max_speed, cs.current_speed);
        bar.SetFuryBar(cs.current_max_fury, cs.current_fury);
        bar.SetIconType(config_stats.attack_type);
        bar.SetIconSkill(skill);
        bar.SetName(charName);
    }
    public void GainHealth(float health)
    {
        cs.current_health += health;
        if (cs.current_health > cs.max_health) cs.current_health = cs.max_health;
        bar.SetHealthBar(cs.max_health, cs.current_health);
    }
    public void LoseHealth(float health)
    {
        cs.current_health -= health;
        if (cs.current_health <= 0)
        {
            cs.current_health = 0;
            AddStatus(new SailorStatus(SailorStatusType.DEATH));
            if (IsDeath()) RunDeath();
        }
        bar.SetHealthBar(cs.max_health, cs.current_health);
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition("-" + (int)health, transform.position);
    }
    public void GainFury(int value)
    {
        if (skill != null)
        {
            cs.current_fury += value;
            if (cs.current_fury > cs.current_max_fury) cs.current_fury = cs.current_max_fury;
            bar.SetFuryBar(cs.current_max_fury, cs.current_fury);
        }
    }
    public int GetSpeedNeeded()
    {
        return cs.max_speed - cs.current_speed;
    }
    public void AddSpeed(int speedAdd)
    {
        cs.current_speed += speedAdd;
        bar.SetSpeedBar(cs.max_speed, cs.current_speed);
    }
    public void IncDisplaySpeed(float spInc)
    {
        cs.display_speed += spInc;
        cs.max_speed = (int)(10000f / cs.display_speed);

        bar.SetSpeedBar(cs.max_speed, cs.current_speed);
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
};
