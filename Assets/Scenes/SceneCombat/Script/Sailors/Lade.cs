using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class Lade : CombatSailor
{
    private Spine.Bone boneTarget;
    public Lade()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
        boneTarget = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("target");
    }
    public override void GainFury(int value)
    {
        base.GainFury(value);
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        TriggerAnimation("Attack");
        {
            Vector3 relativePos = transform.InverseTransformPoint(target.transform.position);
            relativePos.x *= modelObject.transform.localScale.x;
            relativePos.y += 3.5f;
            boneTarget.SetLocalPosition(relativePos);
        }
        Sequence seq = DOTween.Sequence();
        StartCoroutine(GameUtils.WaitAndDo(0.65f, () => SoundMgr.PlaySound("Audio/Sailor/laze_attack")));
        return 1.0f;
    }
    public override void SetFaceDirection()
    {
        if (modelObject.activeSelf) modelObject.transform.localScale = new Vector3(cs.team == Team.A ? 1f : -1f, 1f, 1f);
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

        float scale_damage_ratio = Model.config_stats.skill_params[0];
        float main_damage = cs.Power * scale_damage_ratio;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor firstTarget = TargetsUtils.Range(this, enermy);
        List<CombatSailor> listTargets = new List<CombatSailor>();
        listTargets.Add(firstTarget);
        listTargets.AddRange(TargetsUtils.AllBehind(firstTarget, enermy));

        listTargets.ForEach(t => targets.Add(t.Model.id));
        for (int i = 0; i < 5; i++)
            listTargets.ForEach(t =>
            {
                _params.Add(t.CalcDamageTake(new Damage() { magic = main_damage/5f }, this));
            });

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        var listTargets = CombatState.Instance.GetSailors(targets);
        CombatState.Instance.HighlightSailor2Step(this, listTargets, 1.0f, 1.4f);

        var mainTarget = listTargets[0];
        Vector3 oriPos = transform.position;
        int offset = transform.position.x < mainTarget.transform.position.x ? -1 : 1;

        var targetPos = mainTarget.transform.position;
        targetPos.z -= 0.1f;
        Vector3 desPos = new Vector3(
            offset * 2,
            targetPos.y,
            targetPos.z - 0.1f
        );

        StartCoroutine(GameUtils.WaitAndDo(0.2f, () => SoundMgr.PlaySound("Audio/Sailor/gear")));
        StartCoroutine(GameUtils.WaitAndDo(0.8f, () => SoundMgr.PlaySound("Audio/Sailor/sucking_surfacing")));
        StartCoroutine(GameUtils.WaitAndDo(1.2f, () => SoundMgr.PlaySound("Audio/Sailor/sucking_surfacing")));
        StartCoroutine(GameUtils.WaitAndDo(1.6f, () => SoundMgr.PlaySound("Audio/Sailor/sucking_surfacing")));
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(.2f);
        seq.Append(transform.DOMove(desPos, 0.3f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.8f);
        seq.AppendCallback(() => GameEffMgr.Instance.Shake(0.8f, 3));

        seq.AppendCallback(() =>
        {
            for (int i = 0; i < listTargets.Count; i++) listTargets[i].LoseHealth(new Damage() { magic = _params[i] });
        });
        seq.AppendInterval(.35f);
        seq.AppendCallback(() =>
        {
            for (int i = 0; i < listTargets.Count; i++) listTargets[i].LoseHealth(new Damage() { magic = _params[listTargets.Count + i] });
        });
        seq.AppendInterval(.35f);
        seq.AppendCallback(() =>
        {
            for (int i = 0; i < listTargets.Count; i++) listTargets[i].LoseHealth(new Damage() { magic = _params[listTargets.Count * 2 + i] });
        });
        seq.AppendInterval(.35f);
        seq.AppendCallback(() =>
        {
            for (int i = 0; i < listTargets.Count; i++) listTargets[i].LoseHealth(new Damage() { magic = _params[listTargets.Count * 3 + i] });
        });
        seq.AppendInterval(.35f);
        seq.AppendCallback(() =>
        {
            for (int i = 0; i < listTargets.Count; i++) listTargets[i].LoseHealth(new Damage() { magic = _params[listTargets.Count * 4 + i] });
        });
        seq.AppendInterval(0.35f);
        seq.Append(transform.DOMove(oriPos, 0.15f).SetEase(Ease.OutSine));

        return 3.6f;
    }
}