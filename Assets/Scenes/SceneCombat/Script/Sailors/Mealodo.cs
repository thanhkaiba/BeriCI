using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class Mealodo : CombatSailor
{
    public Mealodo()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    void Start()
    {
        // appear eff
        if (Model.id.EndsWith("summoned"))
        {
            Spine.Skeleton skeleton = transform.GetComponentInChildren<SkeletonMecanim>().skeleton;
            skeleton.SetColor(new Color(0, 0, 0, 0));
            Vector3 pos = transform.position;
            pos.y += 6.1f;
            pos.z -= 0.1f;
            var eff = Instantiate(Resources.Load<GameObject>("Effect2D/lazer/Lazer_purple"), pos, Quaternion.identity);
            eff.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            SoundMgr.PlaySound(PirateraSoundEffect.SUMMON);
        }
    }
    public override void GainFury(int value)
    {
        base.GainFury(value);
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        DOTween.Kill(transform);
        TriggerAnimation("Attack");
        Vector3 oriPos = transform.position;
        int offset = transform.position.x < target.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 5,
            target.transform.position.y,
            target.transform.position.z - 0.1f
        );
        Sequence seq1 = DOTween.Sequence();
        seq1.AppendInterval(0.4f);
        seq1.AppendCallback(() => SoundMgr.PlaySoundAttackSailor(11));
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.1f);
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        seq.AppendInterval(.6f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        seq.SetTarget(transform).SetLink(gameObject);
        return .45f;
    }
    public override float TakeDamage(Damage d)
    {
        TriggerAnimation("Hurt");
        return base.TakeDamage(d);
    }

    public override bool CanActiveSkill(CombatState cbState)
    {
        return base.CanActiveSkill(cbState);
    }
    public override float CastSkill(CombatState cbState)
    {
        float damage;
        float dame_health = Model.config_stats.skill_params[0];
        List<CombatSailor> enermy = cbState.GetAliveCharacterEnemy(cs.team);
        CombatSailor target = TargetsUtils.Melee(this, enermy);
        damage = cs.Power * dame_health;
        return RunAnimation(target, damage);
    }

    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        CombatSailor target = CombatState.Instance.GetSailor(targets[0]);
        float loseHealth = _params[0];
        return RunAnimation(target, loseHealth);

    }

    float RunAnimation(CombatSailor target, float damage)
    {
        DOTween.Kill(transform);
        base.ProcessSkill();
        TriggerAnimation("Skill");
        CombatEvents.Instance.highlightTarget.Invoke(target);
        Vector3 oriPos = transform.position;

        int offset = transform.position.x < target.transform.position.x ? -1 : 1;

        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 4f,
            target.transform.position.y,
            target.transform.position.z
        );
        desPos.z -= 0.1f;
        SoundMgr.PlaySoundSkillSailor(12);
        var x = damage / 2;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(desPos, 0.3f).SetEase(Ease.OutSine));
        seq.AppendInterval(.2f);
        seq.AppendCallback(() =>
        {
            Sequence seq2 = DOTween.Sequence();
            seq2.AppendCallback(() => target.LoseHealth(new Damage() { physics = damage / 2 }));
            seq2.AppendInterval(.35f);
            seq2.AppendCallback(() => target.LoseHealth(new Damage() { physics = damage / 2 }));
        });
        seq.AppendInterval(.6f);
        seq.Append(transform.DOMove(oriPos, 0.15f).SetEase(Ease.OutSine));
        seq.SetTarget(transform).SetLink(gameObject);
        return 1.5f;
    }
}
