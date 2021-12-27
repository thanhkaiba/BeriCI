using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class Sojeph : CombatSailor
{
    public SailorConfig config;
    //private Spine.Bone boneTarget;
    public Sojeph()
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
        targetPos.y += 2f;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(.8f);

        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(13);
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("knife2");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            GameEffMgr.Instance.BulletToTarget(startPos, targetPos, 0f, 0.2f);
        });
        return 1;
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
        return cbState.GetAllTeamAliveExceptSelfSailors(cs.team, this).Count > 0;
    }
    public override float CastSkill(CombatState cbState)
    {
        base.CastSkill(cbState);
        List<string> targets = new List<string>();
        List<float> _params = new List<float>();
        float main_damage = cs.Power;
        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        List<CombatSailor> teams = cbState.GetAllTeamAliveExceptSelfSailors(cs.team, this);
        CombatSailor healthTarget = TargetsUtils.LowestHealth(teams);
        targets.Add(healthTarget.Model.id);
        CombatSailor target = TargetsUtils.Range(this, enermy);
        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { physics = main_damage }));
        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        var healthTarget = CombatState.Instance.GetSailor(targets[0]);
        var target = CombatState.Instance.GetSailor(targets[1]);
        var healthGain = cs.Power * Model.config_stats.skill_params[0];
        CombatEvents.Instance.highlightTarget.Invoke(healthTarget);
        var damage = _params[0];
        CombatEvents.Instance.highlightTarget.Invoke(target);

        var seq = DOTween.Sequence();
        seq.AppendInterval(1.0f);
        seq.AppendCallback(() => StartEffHealth(healthTarget, healthGain));
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => {
            StartEffDame(target, damage);
        });
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {
            target.LoseHealth(new Damage() { physics = damage });
        });

        return 3;
    }
    public void StartEffHealth(CombatSailor target, float health)
    {
        SoundMgr.PlaySoundSkillSailor(14);
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            var pos = target.transform.position;
            pos.y += 4f;
            var eff = Instantiate(Resources.Load<GameObject>("Effect2D/buff/ef_24_green"), pos, Quaternion.identity);
            target.GainHealth(health);
            seq.AppendInterval(0.3f);
            seq.AppendCallback(() => Destroy(eff));
        });


    }
    public void StartEffDame(CombatSailor target, float damage)
    {
        float time = 0;
        Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("knife2");
        var startPos = gun2.GetWorldPosition(modelObject.transform);
        var targetPos = target.transform.position;
        targetPos.y += 2;
        time = 5f / Vector3.Distance(startPos, targetPos);
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            GameEffMgr.Instance.BulletToTarget(startPos, targetPos, 0, time);
        });
        seq.AppendInterval(time);
        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(13);
            target.LoseHealth(new Damage() { physics = damage });
        });
    }
}
