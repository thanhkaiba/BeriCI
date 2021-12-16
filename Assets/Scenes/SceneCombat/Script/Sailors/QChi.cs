using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QChi : CombatSailor
{
    public SailorConfig config;
    public QChi()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        /*   Vector3 relativePos = transform.InverseTransformPoint(target.transform.position);
           relativePos.y += 4.5f;
           relativePos.x *= modelObject.transform.localScale.x;*/
        TriggerAnimation("Attack");

        Vector3 targetPos = target.transform.position;
        targetPos.y += 3.4f;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.68f);
        seq.AppendCallback(() =>
        {
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("fx_ball_1");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            Vector3 endPos = target.transform.position;
            endPos.y += 2;
            SoundMgr.PlaySoundAttackSailor(7);
            var go = GameEffMgr.Instance.ShowPurple(endPos, startPos.x < targetPos.x);
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

        float magic_damage = cs.Power * Model.config_stats.skill_params[0];

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        List<CombatSailor> listTargets = TargetsUtils.AllFirstRow(enermy);

        listTargets.ForEach(t =>
        {
            targets.Add(t.Model.id);
            _params.Add(t.CalcDamageTake(new Damage() { magic = magic_damage }));
        });

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        var listTargets = CombatState.Instance.GetSailors(targets);
        var listHighlight = new List<CombatSailor>() { this };
        listHighlight.AddRange(listTargets);
        CombatState.Instance.HighlightListSailor(listHighlight, 2.2f);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1f);
        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundSkillSailor(7);
            for (int i = 0; i < listTargets.Count; i++)
            {

                Spine.Bone bone = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("fx_ball_1");
                Vector3 startPos = bone.GetWorldPosition(modelObject.transform);
                Vector3 endPos = listTargets[i].transform.position;
                endPos.y += 2;
                GameEffMgr.Instance.TrailToTarget("Effect2D/magic_attack/Trail_purple", "Effect2D/118 sprite effects bundle/25 sprite effects/ef_22_purple", startPos, endPos, 0, .4f,1 , 1f);
            }


        });
        seq.AppendInterval(.5f);
        seq.AppendCallback(() =>
        {
            for (int i = 0; i < listTargets.Count; i++) listTargets[i].LoseHealth(new Damage() { magic = _params[i] });

        });

        return 3.5f;
    }
}

