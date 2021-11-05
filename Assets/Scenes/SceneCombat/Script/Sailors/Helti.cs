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
        circle = Instantiate(Resources.Load<GameObject>("GameComponents/SkillAvaiableCircle/circle"));
        circle.GetComponent<CircleSkillAvaiable>().SetCharacter(gameObject);
        circle.SetActive(false);

        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    public override void GainFury(int value)
    {
        base.GainFury(value);
        if (cs.Fury >= cs.MaxFury && !circle.activeSelf) circle.GetComponent<CircleSkillAvaiable>().Appear();
    }
    public override float UseSkill(CombatState combatState)
    {
        circle.GetComponent<CircleSkillAvaiable>().Disappear();
        return base.UseSkill(combatState);
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        TriggerAnimation("BaseAttack");
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 4.0f);
        desPos.y += 1;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.15f);
        seq.Append(transform.DOJump(desPos, 1.2f, 1, 0.3f));
        seq.AppendInterval(0.2f);
        seq.Append(transform.DOJump(oriPos, 1, 1, 0.3f));
        return 0.4f;
    }
    public override void SetFaceDirection()
    {
        if (modelObject.activeSelf) modelObject.transform.localScale = new Vector3(cs.team == Team.A ? 1.42f : -1.42f, 1.42f, 1.42f);
    }
    private void LateUpdate()
    {
        if (cs != null) SetFaceDirection();
    }
    public override float TakeDamage(Damage d)
    {
        TriggerAnimation("TakeDamage");
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
        CombatSailor target = Targets.Melee(this, enermy);
        List<CombatSailor> behind_target = Targets.AllBehind(target, enermy);

        return RunAnimation(target, behind_target, main_damage, secondary_damage);
    }
    float RunAnimation(CombatSailor target, List<CombatSailor> behind_target, float main_damage, float secondary_damage)
    {
        float scale_damage_ratio = Model.config_stats.skill_params[0];
        float behind_damage_ratio = Model.config_stats.skill_params[1];
        TriggerAnimation("CastSkill");
        CombatEvents.Instance.highlightTarget.Invoke(target);
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 8.0f);
        desPos.y += 1;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.15f);
        seq.Append(transform.DOJump(desPos, 1.2f, 1, 0.3f));

        seq.AppendCallback(() =>
        {
            target.TakeDamage(main_damage, 0, 0);
            behind_target.ForEach(s => s.TakeDamage(secondary_damage, 0, 0, 2));
        });
        seq.AppendInterval(0.35f);
        seq.AppendCallback(() =>
        {
            target.TakeDamage(main_damage, 0, 0);
            behind_target.ForEach(s => s.TakeDamage(secondary_damage, 0, 0, 2));
        });
        seq.AppendInterval(0.35f);
        seq.AppendCallback(() =>
        {
            target.TakeDamage(main_damage, 0, 0);
            behind_target.ForEach(s => s.TakeDamage(secondary_damage, 0, 0, 2));
        });

        seq.AppendInterval(0.3f);
        seq.Append(transform.DOJump(oriPos, 0.8f, 1, 0.4f));
        return 1.8f;
    }
}