using DG.Tweening;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Helti : CombatSailor
{
    private GameObject circle;
    public Helti()
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
            target.transform.position.x + offset * 4,
            target.transform.position.y,
            target.transform.position.z - 0.1f
        );
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.25f);
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.3f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return 0.5f;
    }
    public override void SetFaceDirection()
    {
        if (modelObject.activeSelf) modelObject.transform.localScale = new Vector3(cs.team == Team.A ? 1f : -1f, 1f, 1f);
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
        List<CombatSailor> behind_target = TargetsUtils.AllBehind(target, enermy);

        return RunAnimation(target, behind_target, main_damage, secondary_damage);
    }
    float RunAnimation(CombatSailor target, List<CombatSailor> behind_target, float main_damage, float secondary_damage)
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
            target.transform.position.z - 0.1f
        );
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.15f);
        seq.Append(transform.DOMove(desPos, 0.3f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.3f);

        seq.AppendCallback(() =>
        {
            target.TakeDamage(main_damage/3, 0, 0);
            behind_target.ForEach(s => s.TakeDamage(secondary_damage/3));
        });
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            target.TakeDamage(main_damage/3, 0, 0);
            behind_target.ForEach(s => s.TakeDamage(secondary_damage/3));
        });
        seq.AppendInterval(0.8f);
        seq.AppendCallback(() =>
        {
            target.TakeDamage(main_damage/3, 0, 0);
            behind_target.ForEach(s => s.TakeDamage(secondary_damage/3));
        });

        seq.AppendInterval(0.8f);
        seq.Append(transform.DOMove(oriPos, 0.15f).SetEase(Ease.OutSine));
        return 3.2f;
    }
}