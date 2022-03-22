using DG.Tweening;
using Piratera.Sound;
using System.Collections.Generic;
using UnityEngine;

public class Daedra : CombatSailor
{
    private GameObject circle;
    public Daedra()
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
        Vector3 oriPos = transform.position;
        int offset = transform.position.x < target.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 6,
            target.transform.position.y,
            target.transform.position.z - 0.1f
        );
        Sequence seq = DOTween.Sequence();
        StartCoroutine(GameUtils.WaitAndDo(0.1f, () => SoundMgr.PlaySoundAttackSailor(9)));
        seq.AppendInterval(0.1f);
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.8f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return 0.8f;
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

        float scale_damage_ratio = Model.config_stats.skill_params[0];
        int max_mana = (int) Model.config_stats.skill_params[1];

        float main_damage = cs.Power * scale_damage_ratio;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor target = TargetsUtils.Melee(this, enermy);

        int mana = target.cs.Fury;
        int mana_suck = Mathf.Min(mana, max_mana);

        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { magic = main_damage }, this));
        _params.Add(mana_suck);

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        CombatSailor target = CombatState.Instance.GetSailor(targets[0]);


        CombatEvents.Instance.highlightTarget.Invoke(target);
        Vector3 oriPos = transform.position;

        int offset = transform.position.x < target.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 4,
            target.transform.position.y,
            target.transform.position.z
        );
        desPos.z -= 0.1f;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.3f);
        seq.Append(transform.DOMove(desPos, 0.3f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.6f);
        StartCoroutine(GameUtils.WaitAndDo(0.8f, () => SoundMgr.PlaySoundSkillSailor(10)));
        seq.AppendCallback(() =>
        {
            target.LoseHealth(new Damage() { magic = _params[0] });
            int mana_suck = (int)_params[1];
            GainFury(mana_suck);
            target.LoseFury(mana_suck);
        });
        seq.AppendInterval(1.0f);
        seq.Append(transform.DOMove(oriPos, 0.15f).SetEase(Ease.OutSine));
        return 2.6f;
    }
}