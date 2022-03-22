using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class Meechik : CombatSailor
{
    private Spine.Bone boneTarget;
    public Meechik()
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
        TriggerAnimation("Attack");
        boneTarget.SetLocalPosition(relativePos);

        Vector3 targetPos = target.transform.position;
        targetPos.y += 3.0f;
        Sequence sq = DOTween.Sequence();
        sq.AppendInterval(0.3f);
        sq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(5);
        });
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
        List<string> targets = new List<string>();
        List<float> _params = new List<float>();

        float main_damage = cs.Power * Model.config_stats.skill_params[0];
        float aoe_damage = cs.Power * Model.config_stats.skill_params[1];

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor target = TargetsUtils.Range(this, enermy);
        List<CombatSailor> around_target = TargetsUtils.Around(target, enermy, false);

        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { magic = main_damage }, this));

        around_target.ForEach(t =>
        {
            targets.Add(t.Model.id);
            _params.Add(t.CalcDamageTake(new Damage() { magic = aoe_damage }, this));
        });

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        SoundMgr.PlaySoundSkillSailor(5);
        var listTargets = CombatState.Instance.GetSailors(targets);
        var mainTarget = CombatState.Instance.GetSailor(targets[0]);


        CombatEvents.Instance.highlightTarget.Invoke(mainTarget);
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, mainTarget.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, mainTarget.transform.position, d - 8.0f);
        desPos.y += 1;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1.6f);
        seq.AppendCallback(() =>
        {
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("gun2");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            Vector3 targetPos = mainTarget.transform.position;
            targetPos.y += 3.3f;
            GameEffMgr.Instance.BulletToTarget(startPos, targetPos, 0f, 0.3f);

        });
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() =>
        {
            Vector3 explorePos = mainTarget.transform.position;
            explorePos.y += 3.4f;
            GameEffMgr.Instance.ShowSmallExplosion(explorePos);
        });
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() =>
        {
            GameEffMgr.Instance.Shake(0.3f, 3.0f);
            for (int i = 0; i < targets.Count; i++) listTargets[i].LoseHealth(new Damage() { magic = _params[i] });
        });

        seq.AppendInterval(0.3f);
        return 2.8f;
    }
}
