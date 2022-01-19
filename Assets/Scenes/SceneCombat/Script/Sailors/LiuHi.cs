using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LiuHi : CombatSailor
{
    public SailorConfig config;
    private Spine.Bone boneTarget;
    private Spine.Bone boneArr;
    public LiuHi()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
        boneTarget = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("ip");
        boneArr = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("ip");
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        Vector3 relativePos = transform.InverseTransformPoint(target.transform.position);
        relativePos.y += 1.5f;
        relativePos.x *= modelObject.transform.localScale.x;
        TriggerAnimation("Attack");
        boneTarget.SetLocalPosition(relativePos);

        Vector3 targetPos = target.transform.position;
        targetPos.y += 3.0f;

        Sequence sq = DOTween.Sequence();

        sq.AppendInterval(.3f);
        sq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(3);
        });
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1.0f);
        Vector3 startPos = boneArr.GetWorldPosition(modelObject.transform);
        return 0.55f;
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
        List<string> targets = new List<string>();
        List<float> _params = new List<float>();

        List<CombatSailor> alies = cbState.GetAllTeamAliveSailors(cs.team);
        var listTargets = TargetsUtils.AroundPlus(this, alies);
        listTargets.Add(this);

        float heal = cs.Power * Model.config_stats.skill_params[0];

        listTargets.ForEach(sailor => {
            targets.Add(sailor.Model.id);
            _params.Add(heal);
        });

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        StartCoroutine(GameUtils.WaitAndDo(0.0f, () => SoundMgr.PlaySoundSkillSailor(14)));
        base.ProcessSkill();
        List<CombatSailor> alies = CombatState.Instance.GetSailors(targets);
        float loseHealth = _params[0];

        CombatState.Instance.HighlightListSailor(alies, 1.4f);
        TriggerAnimation("Skill");
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1.2f);
        seq.AppendCallback(() => {
            for (int i = 0; i < alies.Count; i++)
            {
                alies[i].GainHealth(_params[i]);
                var pos = alies[i].transform.position;
                pos.y += 4f;
                var eff = Instantiate(Resources.Load<GameObject>("Effect2D/buff/ef_24_green"), pos, Quaternion.identity);
                seq.AppendInterval(0.3f);
                seq.AppendCallback(() => Destroy(eff));
            }
        });
        return 2.1f;
    }
}
