using DG.Tweening;
using Piratera.Sound;
using System.Collections.Generic;
using UnityEngine;

public class Tons : CombatSailor
{
    private GameObject circle;
    public Tons()
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
        float d = Vector3.Distance(oriPos, target.transform.position);
        //Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 4.0f);
        Vector3 targetPos = target.transform.position;
        Vector3 desPos = new Vector3(targetPos.x - 3.0f * (transform.position.x > targetPos.x ? 1 : -1), targetPos.y, targetPos.z);
        desPos.z -= 1;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.22f);
        seq.AppendCallback(() =>
        {
            GameObject ex = Instantiate(
                Resources.Load<GameObject>("Effect2D/tele/tele"),
                Vector3.MoveTowards(oriPos, desPos, -1), new Quaternion());
            Sequence seq2 = DOTween.Sequence();
            seq2.AppendInterval(1.0f);
            seq2.AppendCallback(() => Destroy(ex));
        });

        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => SoundMgr.PlaySoundAttackSailor(10));
        seq.AppendCallback(() =>
        {
            GameObject ex = Instantiate(Resources.Load<GameObject>("Effect2D/tele/tele"), desPos, new Quaternion());
            Sequence seq2 = DOTween.Sequence();
            seq2.AppendInterval(1.0f);
            seq2.AppendCallback(() => Destroy(ex));
        });
        seq.AppendInterval(0.2f);
        seq.Append(transform.DOMove(desPos, 0.0f));
        float currentLocalScaleX = modelObject.transform.localScale.x;
        seq.Append(modelObject.transform.DOScaleX(currentLocalScaleX * -1, 0.0f));
        seq.AppendInterval(0.42f);
        seq.AppendCallback(() =>
        {
            GameObject ex = Instantiate(
                Resources.Load<GameObject>("Effect2D/tele/tele"),
                oriPos, new Quaternion());
            Sequence seq2 = DOTween.Sequence();
            seq2.AppendInterval(1.0f);
            seq2.AppendCallback(() => Destroy(ex));
        });
        seq.AppendInterval(0.2f);
        seq.Append(transform.DOMove(oriPos, 0f).SetEase(Ease.InOutElastic));
        seq.Append(modelObject.transform.DOScaleX(currentLocalScaleX, 0.0f));
        return 1.0f;
    }
    public override void SetFaceDirection()
    {
        if (modelObject.activeSelf) modelObject.transform.localScale = new Vector3(cs.team == Team.A ? 1 : -1, 1, 1);
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
        base.CastSkill(cbState);
        float damage = cs.Power * scale_damage_ratio;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor target = TargetsUtils.Backstab(this, enermy);

        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { physics = damage }));

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        CombatSailor target = CombatState.Instance.GetSailor(targets[0]);
        float damage = _params[0];
        TriggerAnimation("Skill");
        base.ProcessSkill();
        Vector3 posEnemy = target.transform.position;
        posEnemy.y += 1.7f;
        posEnemy.z -= 0.1f;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(.4f);
        seq.AppendCallback(() =>
        {
            GameObject ex = Instantiate(
                Resources.Load<GameObject>("Effect2D/tele/tele"),
                Vector3.MoveTowards(transform.position, posEnemy, -1), new Quaternion());
            Sequence seq2 = DOTween.Sequence();
            seq2.AppendInterval(1.0f);
            seq2.AppendCallback(() => Destroy(ex));
        });
        seq.AppendInterval(.2f);
        seq.AppendCallback(() =>
        {
            bar.gameObject.SetActive(false);
            GameObject ex = Instantiate(
                Resources.Load<GameObject>("Effect2D/Impact/Impact"),
               posEnemy, modelObject.transform.rotation);
            ex.transform.localScale = new Vector3(2f, 2f, 2f);
            SoundMgr.PlaySoundSkillSailor(11);
            Sequence seq2 = DOTween.Sequence();
            seq2.AppendCallback(() => target.LoseHealth(damage / 5));
            seq2.AppendInterval(.2f);
            seq2.AppendCallback(() => target.LoseHealth(damage / 5));
            seq2.AppendInterval(.2f);
            seq2.AppendCallback(() => target.LoseHealth(damage / 5));
            seq2.AppendInterval(.2f);
            seq2.AppendCallback(() => target.LoseHealth(damage / 5));
            seq2.AppendInterval(.2f);
            seq2.AppendCallback(() => target.LoseHealth(damage / 5));
            seq2.AppendInterval(.2f);
            seq2.AppendCallback(() => Destroy(ex));
        });
        seq.AppendInterval(1.0f);
        seq.AppendCallback(() =>
        {
            bar.gameObject.SetActive(true);
            GameObject ex = Instantiate(
                Resources.Load<GameObject>("Effect2D/tele/tele"),
                Vector3.MoveTowards(transform.position, posEnemy, -1), new Quaternion());
            Sequence seq2 = DOTween.Sequence();
            seq2.AppendInterval(1.0f);
            seq2.AppendCallback(() => Destroy(ex));
        });
        return 2.6f;
    }
}