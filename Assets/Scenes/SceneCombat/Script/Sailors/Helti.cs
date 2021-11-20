using DG.Tweening;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Helti : CombatSailor
{
    private GameObject wind;
    private Animator windAnimator;
    public Helti()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;

        wind = Instantiate(Resources.Load<GameObject>("Characters/Helti/skill/skill"));
        windAnimator = wind.transform.Find("eff").GetComponent<Animator>();
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

        float scale_damage_ratio = Model.config_stats.skill_params[0];
        float behind_damage_ratio = Model.config_stats.skill_params[1];
        float main_damage = cs.Power * scale_damage_ratio;
        float secondary_damage = cs.Power * behind_damage_ratio;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor main_target = TargetsUtils.Melee(this, enermy);
        List<CombatSailor> behind_targets = TargetsUtils.AllBehind(main_target, enermy);

        targets.Add(main_target.Model.id);
        _params.Add( main_target.CalcDamageTake(new Damage() { physics = main_damage }) );

        behind_targets.ForEach(t =>
        {
            targets.Add(t.Model.id);
            _params.Add(t.CalcDamageTake(new Damage() { physics = secondary_damage }));
        });

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        var listTarget = CombatState.Instance.GetSailors(targets);
        var mainTarget = listTarget[0];
        Vector3 oriPos = transform.position;
        int offset = transform.position.x < mainTarget.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            mainTarget.transform.position.x + offset * 4,
            mainTarget.transform.position.y,
            mainTarget.transform.position.z - 0.1f
        );

        wind.transform.position = mainTarget.transform.position;
        wind.transform.localScale = new Vector3(cs.team == Team.A ? 1.5f : -1.5f, 1.5f, 1.5f);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.15f);
        seq.Append(transform.DOMove(desPos, 0.3f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.3f);

        seq.AppendCallback(() =>
        {
            for (int i = 0; i < listTarget.Count; i++)
                listTarget[i].LoseHealth(new Damage() { physics = _params[i]/4 });
            windAnimator.SetTrigger("run");
        });
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            for (int i = 0; i < listTarget.Count; i++)
                listTarget[i].LoseHealth(new Damage() { physics = _params[i]/4 });
            windAnimator.SetTrigger("run");
        });
        seq.AppendInterval(0.8f);
        seq.AppendCallback(() =>
        {
            for (int i = 0; i < listTarget.Count; i++)
                listTarget[i].LoseHealth(new Damage() { physics = _params[i]/2 });
            windAnimator.SetTrigger("run");
        });

        seq.AppendInterval(0.8f);
        seq.Append(transform.DOMove(oriPos, 0.15f).SetEase(Ease.OutSine));
        return 3.2f;
    }
}