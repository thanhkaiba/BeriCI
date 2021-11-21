using DG.Tweening;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Obonbee : CombatSailor
{
    private GameObject wind;
    private Animator windAnimator;
    public Obonbee()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    public override void GainFury(int value)
    {
        base.GainFury(value);
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        TriggerAnimation("Attack");
        Vector3 oriPos = transform.position;
        int offset = transform.position.x < target.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 4,
            target.transform.position.y,
            target.transform.position.z - 0.1f
        );
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.25f);
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.3f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return 0.5f;
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

        float physics_damage = cs.Power * Model.config_stats.skill_params[0];

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        List<CombatSailor> listTargets = TargetsUtils.NearestColumn(enermy);

        listTargets.ForEach(t =>
        {
            targets.Add(t.Model.id);
            _params.Add(t.CalcDamageTake(new Damage() { physics = physics_damage }));
        });

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        Debug.Log("targets " + targets.Count);
        Debug.Log("_params " + _params.Count);
        base.ProcessSkill();
        TriggerAnimation("Skill");
        var listTargets = CombatState.Instance.GetSailors(targets);
        var mainTarget = listTargets[0];
        Vector3 oriPos = transform.position;
        int offset = transform.position.x < mainTarget.transform.position.x ? -1 : 1;

        var targetPos = mainTarget.transform.position;
        targetPos.z -= 0.1f;
        Vector3 desPos = new Vector3(
            targetPos.x + offset * 4,
            targetPos.y,
            targetPos.z - 0.1f
        );
        //var listHighlight = new List<CombatSailor>() { this };
        //listHighlight.AddRange(listTargets);
        CombatState.Instance.HighlightSailor2Step(this, listTargets, 0.45f, 2.5f);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.15f);
        seq.Append(transform.DOMove(desPos, 0.3f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.3f);

        seq.AppendCallback(() =>
        {
            for (int i = 0; i < listTargets.Count; i++)
                listTargets[i].LoseHealth(new Damage() { physics = _params[i] * 3 / 10 });
        });
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            for (int i = 0; i < listTargets.Count; i++)
                listTargets[i].LoseHealth(new Damage() { physics = _params[i] * 3 / 10 });
        });
        seq.AppendInterval(0.8f);
        seq.AppendCallback(() =>
        {
            for (int i = 0; i < listTargets.Count; i++)
                listTargets[i].LoseHealth(new Damage() { physics = _params[i] * 4 / 10 });
        });

        seq.AppendInterval(0.8f);
        seq.Append(transform.DOMove(oriPos, 0.15f).SetEase(Ease.OutSine));
        return 3.2f;
    }
}