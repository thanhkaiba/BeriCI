using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class Geechoso : CombatSailor
{
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
        var targetPos = target.transform.position;
        targetPos.y += 2.8f;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.28f);
        seq.AppendCallback(() =>
        {
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("2_hand_7");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            GameEffMgr.Instance.BulletToTarget(startPos, targetPos, 0f, 0.05f, 0.5f);
            SoundMgr.PlaySoundSkillSailor(13);
        });
        return 0.33f;
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

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor target = TargetsUtils.Range(this, enermy);
        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { physics = main_damage }, this));
        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        var target = CombatState.Instance.GetSailor(targets[0]);

        var damage = _params[0];

        var seq = DOTween.Sequence();
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => Shoot(target, damage / 2));
        seq.AppendInterval(0.7f);
        seq.AppendCallback(() => Shoot(target, damage / 2));
        return 1.3f;
    }
    private void Shoot(CombatSailor target, float damage)
    {
        var targetPos = target.transform.position;
        targetPos.y += 3.5f;

        float time = 0.05f;
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundSkillSailor(13);
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("2_hand_7");
            var startPos = gun2.GetWorldPosition(modelObject.transform);
            Transform bullet = GameEffMgr.Instance.BulletToTarget(startPos, targetPos, 0f, time, 0.5f);
        });
        seq.AppendInterval(time);
        seq.AppendCallback(() => target.LoseHealth(new Damage() { physics = damage }));
    }
}
