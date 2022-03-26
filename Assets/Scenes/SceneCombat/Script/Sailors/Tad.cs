using DG.Tweening;
using Piratera.Config;
using Piratera.Sound;
using System.Collections.Generic;
using UnityEngine;

public class Tad : CombatSailor
{
    public Tad()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    public override void GainFury(int value)
    {
        base.GainFury(value);
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        TriggerAnimation("Attack");
        Vector3 oriPos = transform.position;
        int offset = transform.position.x < target.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 4,
            target.transform.position.y,
            target.transform.position.z - 0.1f
        );
        Sequence seq = DOTween.Sequence();
        StartCoroutine(GameUtils.WaitAndDo(0.35f, () => SoundMgr.PlaySoundAttackSailor(2)));
        StartCoroutine(GameUtils.WaitAndDo(0.5f, () => GameEffMgr.Instance.Shake(0.1f, 0.5f)));
        seq.AppendInterval(0.05f);
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.75f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return 0.7f;
    }
    public override void SetFaceDirection()
    {
        if (modelObject.activeSelf) modelObject.transform.localScale = new Vector3(cs.team == Team.A ? 1f : -1f, 1f, 1f);
    }
    // skill
    public override bool CanActiveSkill(CombatState cbState)
    {
        return base.CanActiveSkill(cbState);
    }
    public override float CastSkill(CombatState cbState)
    {
        List<string> targets = new List<string>();
        List<float> _params = new List<float>();

        float scale_damage_ratio = Model.config_stats.skill_params[0];
        float attackTimes = Model.config_stats.skill_params[1];
        float damage = cs.Power * scale_damage_ratio;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor main_target = TargetsUtils.Melee(this, enermy);

        targets.Add(main_target.Model.id);
        for (int i = 0; i < attackTimes; i++)
        {
            _params.Add(main_target.CalcDamageTake(new Damage() { physics = damage }, this));
        }

        SynergiesConfig config = GlobalConfigs.Synergies;
        ClassBonusItem wild = CombatState.Instance.GetTeamClassBonus(cs.team, SailorClass.WILD);
        float healthGain = 0;
        if (cs.HaveType(SailorClass.WILD) && wild != null)
        {
            float percentHealthGain = config.GetParams(wild.type, wild.level)[0];
            healthGain = percentHealthGain * cs.MaxHealth * attackTimes;
        }
        _params.Add(healthGain);
        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        Debug.Log("targets " + targets.Count);
        Debug.Log("_params " + _params.Count);
        base.ProcessSkill();
        TriggerAnimation("Skill");
        var mainTarget = CombatState.Instance.GetSailor(targets[0]);
        Vector3 oriPos = transform.position;
        int offset = transform.position.x < mainTarget.transform.position.x ? -1 : 1;

        var targetPos = mainTarget.transform.position;
        targetPos.z -= 0.1f;
        Vector3 desPos = new Vector3(
            targetPos.x + offset * 4,
            targetPos.y,
            targetPos.z - 0.1f
        );

        //var listHighlight = new List<CombatSailor>() { this };
        //listHighlight.AddRange(listTargets);

        Sequence seq = DOTween.Sequence();
        StartCoroutine(GameUtils.WaitAndDo(0.0f, () => SoundMgr.PlaySoundSkillSailor(2)));
        seq.AppendInterval(0.15f);
        seq.Append(transform.DOMove(desPos, 0.3f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() =>
        {
            mainTarget.LoseHealth(new Damage() { physics = _params[1] }, false);
            GainHealth(_params[0] / 3f);
            GameEffMgr.Instance.Shake(0.2f, 1);
        });
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() =>
        {
            mainTarget.LoseHealth(new Damage() { physics = _params[2] }, false);
            GameEffMgr.Instance.Shake(0.2f, 1);
            GainHealth(_params[0] / 3f);
        });
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() =>
        {
            mainTarget.LoseHealth(new Damage() { physics = _params[3] });
            GameEffMgr.Instance.Shake(0.3f, 2);
            GainHealth(_params[0] / 3f);
        });

        seq.AppendInterval(0.8f);
        seq.Append(transform.DOMove(oriPos, 0.15f).SetEase(Ease.OutSine));
        return 2.8f;
    }
}