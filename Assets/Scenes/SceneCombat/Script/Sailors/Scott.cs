using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class Scott : CombatSailor
{
    private Spine.Bone boneTarget;
    public Scott()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
        boneTarget = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("bone_attack");
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        TriggerAnimation("Attack");
        var targetPos = target.transform.position;
        targetPos.y += 2.8f;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.6f);
        seq.AppendCallback(() => SoundMgr.PlaySoundSkillSailor(13));
        return 0.7f;
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
        TriggerAnimation("Skill");
        List<string> targets = new List<string>();
        List<float> _params = new List<float>();

        float main_damage = cs.Power * Model.config_stats.skill_params[0];

        bool isCrit = IsCrit();

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor target = TargetsUtils.Backstab(this, enermy);
        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { physics = main_damage * (isCrit ? cs.CritDamage : 1) }, this));
        _params.Add(isCrit ? 1 : -1);
        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        var target = CombatState.Instance.GetSailor(targets[0]);
        {
            Vector3 relativePos = transform.InverseTransformPoint(target.transform.position);
            relativePos.x *= modelObject.transform.localScale.x;
            relativePos.x -= 5;
            boneTarget.SetLocalPosition(relativePos);
        }
        var damage = _params[0];

        var seq = DOTween.Sequence();
        seq.AppendInterval(1.5f);
        seq.AppendCallback(() => target.LoseHealth(new Damage() { physics = _params[0], isCrit = _params[1] > 0 }));
        return 2.5f;
    }
}
