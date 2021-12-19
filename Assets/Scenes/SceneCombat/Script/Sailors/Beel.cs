using DG.Tweening;
using Spine.Unity;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Beel : CombatSailor
{
    public SailorConfig config;
    public Beel()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        Vector3 relativePos = transform.InverseTransformPoint(target.transform.position);
        relativePos.y += 4.5f;
        relativePos.x *= modelObject.transform.localScale.x;
        TriggerAnimation("Attack");

        Vector3 targetPos = target.transform.position;
        targetPos.y += 3.4f;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.68f);
        seq.AppendCallback(() =>
        {
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("gun2");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            //startPos.y -= 0.4f;
            GameEffMgr.Instance.BulletToTarget(startPos, targetPos, 0f, 0.2f);

            var go = GameEffMgr.Instance.ShowSmokeSide(startPos, startPos.x < targetPos.x);
            go.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        });
        seq.AppendInterval(0.1f);
        return 0.9f;
    }

    public override void SetFaceDirection()
    {
        if (modelObject.activeSelf) modelObject.transform.localScale = new Vector3(cs.team == Team.A ? 1 : -1, 1, 1);
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
        base.CastSkill(cbState);
        float magic_damage = cs.Power * Model.config_stats.skill_params[0];

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor target = TargetsUtils.Range(this, enermy);
        List<CombatSailor> around_target = TargetsUtils.Around(target, enermy, true);

        return RunAnimation(target, around_target, magic_damage);
    }
    float RunAnimation(CombatSailor target, List<CombatSailor> around_target, float magic_damage)
    {
        TriggerAnimation("Skill");

        CombatEvents.Instance.highlightTarget.Invoke(target);
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 8.0f);
        desPos.y += 1;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1.6f);
        seq.AppendCallback(() =>
        {
            Debug.LogError(around_target.Count);
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("gun2");
            //Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
           // startPos.y -= 0.0f;
            Vector3 targetPos = target.transform.position;
            targetPos.y += 3.4f;
            for (int i = 0; i < around_target.Count; i++)
            {
               // Spine.Bone bone = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("fx_ball_1");
               // Vector3 startPos = bone.GetWorldPosition(modelObject.transform);
                Vector3 endPos = around_target[i].transform.position;
                endPos.y += 2;
                GameEffMgr.Instance.TrailToTarget("Effect2D/magic_attack/Trail_purple", "Effect2D/118 sprite effects bundle/25 sprite effects/ef_22_purple", transform.position, endPos, 0, .4f, 1, 1f);
            }
            //GameEffMgr.Instance.BulletToTarget(startPos, targetPos, 0f, 0.4f);
        });
        seq.AppendInterval(0.4f);
        seq.AppendCallback(() => {
            Vector3 explorePos = target.transform.position;
            explorePos.y += 3.4f;
           // GameEffMgr.Instance.ShowSmallExplosion(explorePos);
        });
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() =>
        {
            around_target.ForEach(s => s.LoseHealth(new Damage() { magic = magic_damage }));
        });

        seq.AppendInterval(0.3f);
        return 2.8f;
    }
}
