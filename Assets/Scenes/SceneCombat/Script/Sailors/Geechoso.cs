using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geechoso : CombatSailor
{
    public SailorConfig config;
    //private Spine.Bone boneTarget;
    public Geechoso()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
        //boneTarget = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("target");
        //boneTarget.SetLocalPosition(new Vector3(-200, 0, 0));
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        TriggerAnimation("Attack");
        //boneTarget.SetLocalPosition(relativePos);

        Vector3 targetPos = target.transform.position;
        targetPos.y += 2f;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() =>
        {
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("2_hand_7");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            //startPos.y -= 0.4f;
            GameEffMgr.Instance.BulletToTarget(startPos, targetPos, 0f, 0.2f);

            //var go = GameEffMgr.Instance.ShowSmokeSide(startPos, startPos.x < targetPos.x);
            //go.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        });
        return 0.6f;
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

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor target = TargetsUtils.Range(this, enermy);
        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { physics = main_damage }));
        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        var mainTarget = CombatState.Instance.GetSailor(targets[0]);
        CombatEvents.Instance.highlightTarget.Invoke(mainTarget);
        Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("2_hand_7");
        Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
        Vector3 targetPos = mainTarget.transform.position;
        targetPos.y += 2;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.4f);
        seq.AppendCallback(() =>
        {
           
            GameEffMgr.Instance.BulletToTarget(startPos, targetPos, 0f, 0.4f);
        });
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() =>
        {
            mainTarget.LoseHealth(new Damage() { physics = (_params[0] /2)});
        });
        seq.AppendInterval(0.8f);
        seq.AppendCallback(() =>
        {          
            GameEffMgr.Instance.BulletToTarget(startPos, targetPos, 0f, 0.4f);
        });
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {
            mainTarget.LoseHealth(new Damage() { physics = (_params[0]/2) });
        });


        return 2f;
    }
}
