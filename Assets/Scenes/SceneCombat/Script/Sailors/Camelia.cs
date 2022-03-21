using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class Camelia : CombatSailor
{
    private Spine.Bone boneTarget;
    public Camelia()
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
        Vector3 oriPos = transform.position;
        int offset = transform.position.x < target.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 4,
            target.transform.position.y,
            target.transform.position.z - 0.1f
        );
        Sequence seq = DOTween.Sequence();
        StartCoroutine(GameUtils.WaitAndDo(0.45f, () => SoundMgr.PlaySoundAttackSailor(0)));
        seq.AppendInterval(0.1f);
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.75f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
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

        float scale_damage_ratio = Model.config_stats.skill_params[0];
        float scale_damage_per_losing_health = Model.config_stats.skill_params[1];
        float heal_percent = Model.config_stats.skill_params[2];

        float main_damage = cs.Power * scale_damage_ratio;
        float losing_health_percent = 1f - cs.CurHealth / cs.MaxHealth;
        main_damage *= (1f + losing_health_percent * scale_damage_per_losing_health);
        float heal = heal_percent * main_damage;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor target = TargetsUtils.Melee(this, enermy);

        targets.Add(target.Model.id);
        float dmgDeal = target.CalcDamageTake(new Damage() { magic = main_damage }, this);
        _params.Add(dmgDeal);
        _params.Add(heal);
        if (dmgDeal > target.cs.CurHealth) _params.Add(1);
        else _params.Add(-1);

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

        Vector3 desPos = target.transform.position;
        desPos.y += 2f;
        Sequence seq = DOTween.Sequence();
        GameObject shadow = modelObject.transform.Find("shadow").gameObject;
        bar.gameObject.SetActive(false);
        {
            Vector3 relativePos = transform.InverseTransformPoint(desPos);
            relativePos.x *= modelObject.transform.localScale.x;
            boneTarget.SetLocalPosition(relativePos);
        }
        seq.AppendInterval(1.6f);
        StartCoroutine(GameUtils.WaitAndDo(0.8f, () => SoundMgr.PlaySoundSkillSailor(10)));
        seq.AppendCallback(() =>
        {
            target.LoseHealth(new Damage() { magic = _params[0] });
            GainHealth(_params[1]);
            if (_params[2] > 0)
            {
                GainFury(cs.MaxFury);
                SpeedUp(1.0f);
            }
            bar.gameObject.SetActive(true);
        });
        seq.AppendInterval(0.45f);
        return 2.6f;
    }
}