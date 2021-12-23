using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geechoso : CombatSailor
{
    public SailorConfig config;
    Vector3 startPos;
    Vector3 targetPos;
    CombatSailor mainTarget;
    float dame;
    //private Spine.Bone boneTarget;
    public Geechoso()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        TriggerAnimation("Attack");
        targetPos = target.transform.position;
        targetPos.y += 2f;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() =>
        {
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("2_hand_7");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            GameEffMgr.Instance.BulletToTarget(startPos, targetPos, 0f, 0.2f);
            SoundMgr.PlaySoundSkillSailor(13);
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
        mainTarget = CombatState.Instance.GetSailor(targets[0]);
        CombatEvents.Instance.highlightTarget.Invoke(mainTarget);
        Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("2_hand_7");
        startPos = gun2.GetWorldPosition(modelObject.transform);
        targetPos = mainTarget.transform.position;
        targetPos.y += 2;
        dame = _params[0];
      
        return 2f;
    }
    public void StartEff()
    {
        float time = 0;
        time = 5f / Vector3.Distance(startPos, targetPos);
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundSkillSailor(13);
            GameEffMgr.Instance.BulletToTarget(startPos, targetPos, 0f, time);
        });
        seq.AppendInterval(time);
        seq.AppendCallback(() =>
        {
            mainTarget.LoseHealth(new Damage() { physics = (dame / 2) });
        });
    }
}
