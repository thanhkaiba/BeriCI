using DG.Tweening;
using System.Collections.Generic;
using System.IO;
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
    public override float UseSkill(CombatState combatState)
    {
        return base.UseSkill(combatState);
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        TriggerAnimation("Attack");
        Vector3 oriPos = transform.position;
        int offset = transform.position.x < target.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 6,
            target.transform.position.y,
            target.transform.position.z
        );
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.1f);
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.3f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return 0.4f;
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
        float scale_damage_ratio = Model.config_stats.skill_params[0];
        float behind_damage_ratio = Model.config_stats.skill_params[1];
        base.CastSkill(cbState);
        float main_damage = cs.Power * scale_damage_ratio;
        float secondary_damage = cs.Power * behind_damage_ratio;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor target = TargetsUtils.Melee(this, enermy);
        CombatSailor behind_target = TargetsUtils.Behind(target, enermy);

        return RunAnimation(target, behind_target, main_damage, secondary_damage);
    }
    float RunAnimation(CombatSailor target, CombatSailor behind_target, float main_damage, float secondary_damage)
    {
        float scale_damage_ratio = Model.config_stats.skill_params[0];
        float behind_damage_ratio = Model.config_stats.skill_params[1];
        TriggerAnimation("Skill");
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
        seq.AppendCallback(() =>
        {
            target.TakeDamage(main_damage, 0, 0);
            if (behind_target) behind_target.TakeDamage(secondary_damage);
        });
        seq.AppendInterval(0.45f);
        seq.Append(transform.DOMove(oriPos, 0.15f).SetEase(Ease.OutSine));
        return 1.5f;
    }
}