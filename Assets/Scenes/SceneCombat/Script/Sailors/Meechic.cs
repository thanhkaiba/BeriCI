using DG.Tweening;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Meechic : CombatSailor
{
    public SailorConfig config;
    private GameObject circle;
    public Meechic()
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
        GameEffMgr.Instance.BulletToTarget(transform.FindDeepChild("nodeStartBullet").position, target.transform.position, 0.4f, 0.2f);
        return 0.6f;
    }
    
    public override void SetFaceDirection()
    {
        if (modelObject.activeSelf) modelObject.transform.localScale = new Vector3(cs.team == Team.A ? 1 : -1, 1, 1);
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
        base.CastSkill(cbState);
        float physic_damage = cs.Power;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor target = TargetsUtils.Range(this, enermy);
        List<CombatSailor> around_target = TargetsUtils.Around(target, enermy);

        return RunAnimation(target, around_target, physic_damage);
    }
    float RunAnimation(CombatSailor target, List<CombatSailor> around_target, float physics_damage)
    {
        float scale_damage_ratio = Model.config_stats.skill_params[0];
        float around_damage_ratio = Model.config_stats.skill_params[1];
        TriggerAnimation("BaseAttack");
        GameEffMgr.Instance.BulletToTarget(transform.FindDeepChild("nodeStartBullet").position, target.transform.position, 0.4f, 0.2f);
        CombatEvents.Instance.highlightTarget.Invoke(target);
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 8.0f);
        desPos.y += 1;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.6f);
        seq.AppendCallback(() => GameEffMgr.Instance.ShowExplosion(target.transform.position));
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() =>
        {
            target.TakeDamage(physics_damage * scale_damage_ratio, 0, 0);
            around_target.ForEach(s => s.TakeDamage(physics_damage * around_damage_ratio, 0, 0, 2));
        });

        seq.AppendInterval(0.3f);
        return 1.1f;
    }
}
