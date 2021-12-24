using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Herminia : CombatSailor
{
    public SailorConfig config;
    private Spine.Bone boneTarget;
    private Spine.Bone boneArr;
    public Herminia()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
        boneTarget = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("target");
        boneArr = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("bow");
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
        seq.AppendCallback(() =>
        {
            Vector3 startPos = boneArr.GetWorldPosition(modelObject.transform);
            ArrowTarget(startPos, targetPos, 2f/Vector3.Distance(startPos, targetPos));
        });
        return 1.1f;
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

        ContainerClassBonus config = GlobalConfigs.ClassBonus;
        var state = CombatState.Instance;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor target = TargetsUtils.Range(this, enermy);

        bool activeMarksman = false;
        ClassBonusItem marksman = state.GetTeamClassBonus(cs.team, SailorClass.MARKSMAN);
        if (marksman != null)
        {
            float healthRatio = config.GetParams(marksman.type, marksman.level)[0];
            if (target.cs.GetCurrentHealthRatio() < healthRatio) activeMarksman = true;
            CombatEvents.Instance.activeClassBonus.Invoke(this, SailorClass.MARKSMAN, new List<float>());
        }

        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { pure = cs.Power * (activeMarksman ? 1.5f : 1) }));

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        Debug.Log("targets.Count: " + targets.Count);
        Debug.Log("_params.Count: " + _params.Count);
        CombatSailor target = CombatState.Instance.GetSailor(targets[0]);
        float loseHealth = _params[0];

        CombatState.Instance.HighlightListSailor(new List<CombatSailor> { this }, 1.4f);
        TriggerAnimation("Skill");
        Vector3 relativePos = transform.InverseTransformPoint(target.transform.position);
        relativePos.y += 1.5f;
        relativePos.x *= modelObject.transform.localScale.x;
        boneTarget.SetLocalPosition(relativePos);

        Vector3 targetPos = target.transform.position;
        targetPos.y += 3.0f;
        Sequence sq = DOTween.Sequence();

        sq.AppendInterval(1);
        sq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundSkillSailor(3);
        });
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1.35f);
        seq.AppendCallback(() =>
        {
            Vector3 startPos = boneArr.GetWorldPosition(modelObject.transform);
            ArrowTarget(startPos, targetPos, 2f / Vector3.Distance(startPos, targetPos), true);
        });
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() => target.LoseHealth(new Damage() { pure = loseHealth }));
        return 2.1f;
    }
    private void ArrowTarget(Vector3 startPos, Vector3 targetPos, float flyTime, bool haveAnims = false)
    {
        var arrGO = Instantiate(Resources.Load<GameObject>("Characters/Herminia/arrow/arrow"), startPos, Quaternion.identity);
        arrGO.SetActive(false);

        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, targetPos);
        Vector3 desPos = Vector3.MoveTowards(oriPos, targetPos, d - 1.4f);
        
        float rZ = (float) Math.Atan2(targetPos.y - startPos.y, targetPos.x - startPos.x);
        arrGO.transform.eulerAngles = new Vector3(0, 0, rZ*57.3f);

        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => arrGO.SetActive(true));
        seq.Append(arrGO.transform.DOMove(desPos, flyTime).SetEase(Ease.OutSine));
        seq.AppendCallback(() => Destroy(arrGO));

        arrGO.transform.Find("eff").gameObject.SetActive(haveAnims);
    }
}
