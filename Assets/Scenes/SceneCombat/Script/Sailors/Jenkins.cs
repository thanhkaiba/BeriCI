using DG.Tweening;
using Piratera.Sound;
using Spine;
using System.Collections.Generic;
using UnityEngine;

public class Jenkins : CombatSailor
{
    public Jenkins()
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
            target.transform.position.x + offset * 6,
            target.transform.position.y,
            target.transform.position.z - 0.1f
        );
        Sequence s = DOTween.Sequence();
        s.AppendInterval(0.3f);
        s.AppendCallback(() => SoundMgr.PlaySoundAttackSailor(4));
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.1f);
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));

        seq.AppendInterval(1.2f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return .9f;
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

        float damage;
        float dame_health = Model.config_stats.skill_params[0];
        float scale_health = Model.config_stats.skill_params[1];
        List<CombatSailor> enermy = cbState.GetAliveCharacterEnemy(cs.team);
        CombatSailor target = TargetsUtils.Melee(this, enermy);
        damage = (cs.Power * dame_health) + (scale_health * target.cs.MaxHealth);
        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { physics = damage }, this));

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        ShowRandomTalk();
        base.ProcessSkill();
        TriggerAnimation("Skill");
        var target = CombatState.Instance.GetSailor(targets[0]);
        var damage = _params[0];
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
        seq.AppendInterval(.4f);
        seq.Append(transform.DOMove(desPos, 0.3f).SetEase(Ease.OutSine));
        seq.AppendCallback(() => SoundMgr.PlaySoundSkillSailor(4));
        seq.AppendInterval(.2f);
        seq.AppendCallback(() =>
        {
            Sequence seq2 = DOTween.Sequence();

            seq2.AppendCallback(() => SoundMgr.PlaySoundSkillSailor(4));
            seq2.AppendCallback(() => target.LoseHealth(new Damage() { physics = damage / 5 }, false));
            seq2.AppendInterval(.22f);
            seq2.AppendCallback(() => SoundMgr.PlaySoundSkillSailor(4));
            seq2.AppendCallback(() => target.LoseHealth(new Damage() { physics = damage / 5 }, false));
            seq2.AppendInterval(.22f);
            seq2.AppendCallback(() => SoundMgr.PlaySoundSkillSailor(4));
            seq2.AppendCallback(() => target.LoseHealth(new Damage() { physics = damage / 5 }, false));
            seq2.AppendInterval(.22f);
            seq2.AppendCallback(() => SoundMgr.PlaySoundSkillSailor(4));
            seq2.AppendCallback(() => target.LoseHealth(new Damage() { physics = damage / 5 }, false));
            seq2.AppendInterval(.22f);

            seq2.AppendCallback(() => target.LoseHealth(new Damage() { physics = damage / 5 }));
            seq2.AppendInterval(.22f);

        });
        seq.AppendInterval(1.2f);
        seq.Append(transform.DOMove(oriPos, 0.15f).SetEase(Ease.OutSine));
        return 2.8f;
    }
    private void ShowRandomTalk()
    {
        var random = MathUtils.RandomInt(0, 9);
        string text = "";
        switch (random)
        {
            case 0:
                text = "Missing Huong";
                break;
            case 1:
                text = "Love Trang";
                break;
            case 2:
                text = "Missing Thu";
                break;
            case 3:
                text = "Love Thao";
                break;
            case 4:
                text = "Lan xinh";
                break;
            case 5:
                text = "Memories with Phuong";
                break;
            case 6:
                text = "Love Ngoc";
                break;
            case 7:
                text = "Dream of Linh";
                break;
            case 8:
                text = "Dream of Mai";
                break;
            case 9:
                text = "Quyen\nDon't marry him";
                break;
        }
        GameUtils.ShowChat(transform, text, 2.2f);
    }
}