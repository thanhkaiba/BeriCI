using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System.Collections.Generic;
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
        TriggerAnimation("Attack");

        Vector3 targetPos = target.transform.position;
        targetPos.y += 2;
        targetPos.z -= 0.1f;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1f);
        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(14);
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("TARGET_ORB");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            GameEffMgr.Instance.TrailToTarget("Effect2D/magic_attack/Trail_purple", "Effect2D/118 sprite effects bundle/25 sprite effects/ef_22_purple", startPos, targetPos, 0, .4f, .7f, .7f);
        });
        return 1.4f;
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


        return RunAnimation(enermy, new List<float> { magic_damage });
    }

    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        var listTargets = CombatState.Instance.GetSailors(targets);
        return RunAnimation(listTargets, _params);

    }
    float RunAnimation(List<CombatSailor> targets, List<float> magic_damage)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");

        CombatState.Instance.HighlightListSailor(new List<CombatSailor>() { this }, 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1.6f);
        seq.PrependCallback(() =>
        {
            SoundMgr.PlaySoundSkillSailor(15);
        });
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {
            Spine.Bone ball = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("TARGET_ORB");
            GameEffMgr.Instance.TrailToTarget("Effect2D/Duong_FX/beel_projectile_skill", "Effect2D/Duong_FX/beel_impact_skill", new Vector3(cs.team == Team.A ? -40 : 40, 0, 0), new Vector3(cs.team == Team.A ? 40 : -40, 0, 0), 0, 2.0f, 10, 1);
        });
        seq.AppendInterval(0.7f);
        seq.AppendCallback(() =>
        {
            GameEffMgr.Instance.Shake(1.2f, 1.0f);
        });
        seq.AppendInterval(0.8f);
        seq.AppendCallback(() =>
        {
            int i = 0;
            foreach (var item in targets)
            {
                var eff = Instantiate(Resources.Load<GameObject>("Effect2D/Duong_FX/beel_impact_skill"), item.transform.position, Quaternion.identity);
                seq.AppendInterval(0.3f);
                seq.AppendCallback(() => Destroy(eff));
                item.LoseHealth(new Damage() { magic = magic_damage[i] });
                i++;
            }
        });

        return 4.5f;
    }
}
