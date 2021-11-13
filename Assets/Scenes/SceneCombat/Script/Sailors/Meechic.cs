using DG.Tweening;
using Spine.Unity;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Meechic : CombatSailor
{
    public SailorConfig config;
    private Spine.Bone boneTarget;
    public Meechic()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
        boneTarget = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("target");
        boneTarget.SetLocalPosition(new Vector3(-200, 0, 0));
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        Vector3 relativePos = transform.InverseTransformPoint(target.transform.position);
        relativePos.y += 4.5f;
        relativePos.x *= modelObject.transform.localScale.x;
        TriggerAnimation("attack");
        boneTarget.SetLocalPosition(relativePos);

        Vector3 targetPos = target.transform.position;
        targetPos.y += 3.4f;
        
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.48f);
        seq.AppendCallback(() =>
        {
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("gun2");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            startPos.y -= 0.55f;
            GameEffMgr.Instance.BulletToTarget(startPos, targetPos, 0.4f, 0.2f);
        });
        seq.AppendInterval(0.4f);
        seq.AppendCallback(() =>
        {
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("gun2");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            startPos.y += 2.3f;
            startPos.x += 1f;
            var go = GameEffMgr.Instance.ShowSmokeSide(startPos, startPos.x < targetPos.x);
            go.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        });
        return 1.1f;
    }
    
    public override void SetFaceDirection()
    {
        if (modelObject.activeSelf) modelObject.transform.localScale = new Vector3(cs.team == Team.A ? 1 : -1, 1, 1);
    }
    public override float TakeDamage(Damage d)
    {
        TriggerAnimation("hurt");
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
        //GameEffMgr.Instance.BulletToTarget(transform.FindDeepChild("nodeStartBullet").position, target.transform.position, 0.4f, 0.2f);
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
