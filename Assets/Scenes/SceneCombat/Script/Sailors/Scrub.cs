using DG.Tweening;
using Piratera.Sound;
using System.Collections.Generic;
using UnityEngine;

public class Scrub : CombatSailor
{
    private GameObject circle;
    public Scrub()
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
            target.transform.position.x + offset * 6,
            target.transform.position.y,
            target.transform.position.z - 0.1f
        );
        Sequence seq = DOTween.Sequence();
        StartCoroutine(GameUtils.WaitAndDo(0.1f, () => SoundMgr.PlaySoundAttackSailor(9)));
        seq.AppendInterval(0.1f);
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.3f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return 0.45f;
    }
    public override float TakeDamage(Damage d)
    {
        TriggerAnimation("Hurt");
        return base.TakeDamage(d);
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
        float behind_damage_ratio = Model.config_stats.skill_params[1];

        float main_damage = cs.Power * scale_damage_ratio;
        float secondary_damage = cs.Power * behind_damage_ratio;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor target = TargetsUtils.Melee(this, enermy);
        CombatSailor behind_target = TargetsUtils.Behind(target, enermy);

        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { physics = main_damage }));

        if (behind_target)
        {
            targets.Add(behind_target.Model.id);
            _params.Add(behind_target.CalcDamageTake(new Damage() { physics = secondary_damage }));
        }

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        CombatSailor target = CombatState.Instance.GetSailor(targets[0]);
        CombatSailor behind_target = null;
        if (targets.Count > 1) behind_target = CombatState.Instance.GetSailor(targets[1]);


        CombatEvents.Instance.highlightTarget.Invoke(target);
        Vector3 oriPos = transform.position;

        int offset = transform.position.x < target.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 4,
            target.transform.position.y,
            target.transform.position.z
        );
        desPos.z -= 0.1f;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.3f);
        seq.Append(transform.DOMove(desPos, 0.3f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.35f);
        StartCoroutine(GameUtils.WaitAndDo(0.8f, () => SoundMgr.PlaySoundSkillSailor(10)));
        seq.AppendCallback(() =>
        {
            target.LoseHealth(new Damage() { physics = _params[0] });
            if (behind_target) behind_target.LoseHealth(new Damage() { physics = _params[1] });
        });
        seq.AppendInterval(0.45f);
        seq.Append(transform.DOMove(oriPos, 0.15f).SetEase(Ease.OutSine));
        return 2.0f;
    }
}