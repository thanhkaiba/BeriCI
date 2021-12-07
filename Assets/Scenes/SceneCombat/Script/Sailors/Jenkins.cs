using DG.Tweening;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Jenkins : CombatSailor
{
    public Jenkins()
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
        seq.AppendInterval(0.1f);
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        seq.AppendInterval(1.2f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return .9f;
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
        float damage;
        float dame_health = Model.config_stats.skill_params[0];
        float scale_health = Model.config_stats.skill_params[1];
        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor target = TargetsUtils.Melee(this, enermy);
        damage = (cs.Power * dame_health) + (scale_health * target.cs.MaxHealth);
        return RunAnimation(target, damage);
    }
    float RunAnimation(CombatSailor target, float damage)
    {
        base.ProcessSkill();
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
        seq.AppendInterval(.4f);
        seq.Append(transform.DOMove(desPos, 0.3f).SetEase(Ease.OutSine));
        seq.AppendInterval(.2f);
        seq.AppendCallback(() =>
        {
            Sequence seq2 = DOTween.Sequence();
            seq2.AppendCallback(() => target.TakeDamage(damage / 5));
            seq2.AppendInterval(.22f);
            seq2.AppendCallback(() => target.TakeDamage(damage / 5));
            seq2.AppendInterval(.22f);
            seq2.AppendCallback(() => target.TakeDamage(damage / 5));
            seq2.AppendInterval(.22f);
            seq2.AppendCallback(() => target.TakeDamage(damage / 5));
            seq2.AppendInterval(.22f);
            seq2.AppendCallback(() => target.TakeDamage(damage / 5));
            seq2.AppendInterval(.22f);

        });
        seq.AppendInterval(1.2f);
        seq.Append(transform.DOMove(oriPos, 0.15f).SetEase(Ease.OutSine));
        return 3;
    }
}