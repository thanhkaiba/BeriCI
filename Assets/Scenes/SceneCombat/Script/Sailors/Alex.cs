using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Alex : CombatSailor
{
    public Alex()
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
        Debug.LogError(cs.Armor);
        int offset = transform.position.x < target.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 6,
            target.transform.position.y,
            target.transform.position.z
        );
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.1f);
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        seq.AppendInterval(.7f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return .8f;
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

        float gain_health = Model.config_stats.skill_params[0];
        float gain_armor = Model.config_stats.skill_params[1];

        float main_health = cs.MaxHealth * gain_health;
        float main_armor = gain_armor;
        CombatSailor target = TargetsUtils.Self(this);
        targets.Add(target.Model.id);
        _params.Add(main_health);
        _params.Add(main_armor);

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        CombatSailor target = CombatState.Instance.GetSailor(targets[0]);
        CombatEvents.Instance.highlightTarget.Invoke(target);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1.8f);
        seq.AppendCallback(() =>
        {
            Vector3 pos = transform.position;
            pos.y += 4f;
            var eff = Instantiate(Resources.Load<GameObject>("Effect2D/buff/ef_24_green"), pos, Quaternion.identity);
        });
        seq.AppendInterval(.1f);
        seq.AppendCallback(() => target.GainHealth(_params[0]));
        seq.AppendInterval(.1f);
        seq.AppendCallback(() => target.GainArmor((int)_params[1]));
        return 2f;
    }
}
