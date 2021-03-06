using DG.Tweening;
using Piratera.Sound;
using System.Collections.Generic;
using UnityEngine;

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
        Debug.Log("target: " + target);
        Debug.Log("target id: " + target.Model.id);
        Debug.Log("target id: " + target.Model.config_stats.root_name);
        int offset = transform.position.x < target.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 5,
            target.transform.position.y,
            target.transform.position.z - 0.1f
        );
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        seq.AppendInterval(.3f);
        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(0);
        });
        seq.AppendInterval(0.8f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return .65f;
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

        float shield_percent = Model.config_stats.skill_params[0] + Model.config_stats.skill_params[1]*Model.star;

        float shield = cs.MaxHealth * shield_percent;
        CombatSailor target = TargetsUtils.Self(this);
        targets.Add(target.Model.id);
        _params.Add(shield);

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        CombatSailor target = CombatState.Instance.GetSailor(targets[0]);
        CombatEvents.Instance.highlightTarget.Invoke(target);
        Sequence seq = DOTween.Sequence();
        SoundMgr.PlaySoundSkillSailor(0);
        seq.AppendInterval(1.8f);
        //seq.AppendCallback(() =>
        //{
        //    Vector3 pos = transform.position;
        //    pos.y += 4f;
        //    var eff = Instantiate(Resources.Load<GameObject>("Effect2D/buff/ef_24_green"), pos, Quaternion.identity);
        //    seq.AppendInterval(0.3f);
        //    seq.AppendCallback(() => Destroy(eff));
        //});
        seq.AppendInterval(.1f);
        seq.AppendCallback(() => target.AddShield(_params[0]));
        seq.AppendInterval(.1f);
        return 2f;
    }
    public override float CalcDamageTake(Damage d, float iAR = 0, float iMR = 0, float piAR = 0, float piMR = 0)
    {
        float dmg = base.CalcDamageTake(d, iAR, iMR, piAR, piMR);
        if (cs.Shield > 0) dmg *= 1 - Model.config_stats.skill_params[2];
        return dmg;
    }
}
