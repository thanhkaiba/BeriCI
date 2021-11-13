using DG.Tweening;
using System.Collections.Generic;
using System.IO;
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
    public override float UseSkill(CombatState combatState)
    {
        return base.UseSkill(combatState);
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
        seq.AppendCallback(() => {
            GameObject ex = Instantiate(
                Resources.Load<GameObject>("Effect2D/tele/tele"),
                Vector3.MoveTowards(oriPos, desPos, -1), new Quaternion());
            Sequence seq2 = DOTween.Sequence();
            seq2.AppendInterval(1.0f);
            seq2.AppendCallback(() => Destroy(ex));
        });
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => {
            GameObject ex = Instantiate(Resources.Load<GameObject>("Effect2D/tele/tele"), desPos, new Quaternion());
            Sequence seq2 = DOTween.Sequence();
            seq2.AppendInterval(1.0f);
            seq2.AppendCallback(() => Destroy(ex));
        });
        seq.AppendInterval(0.2f);
        seq.Append(transform.DOMove(desPos, 0.0f));
        float currentLocalScaleX = modelObject.transform.localScale.x;
        seq.Append(modelObject.transform.DOScaleX(currentLocalScaleX*-1, 0.0f));
        seq.AppendInterval(0.42f);
        seq.AppendCallback(() => {
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
    //private void LateUpdate()
    //{
    //    if (cs != null) SetFaceDirection();
    //}
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
        float scale_damage_ratio = Model.config_stats.skill_params[0];
        base.CastSkill(cbState);
        float damage = cs.Power * scale_damage_ratio;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor target = TargetsUtils.Backstab(this, enermy);
        List<CombatSailor> takeDamageUnit = TargetsUtils.SameRow(target, enermy);

        return RunAnimation(target, takeDamageUnit, damage);
    }
    float RunAnimation(CombatSailor target, List<CombatSailor> takeDamageUnit, float damage)
    {
        TriggerAnimation("Skill");
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d + 8.0f);
        //desPos.z -= 1;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.35f);
        seq.AppendCallback(() => {
            GameObject ex = Instantiate(
                Resources.Load<GameObject>("Effect2D/typhoon/typhoon"),
                Vector3.MoveTowards(oriPos, desPos, -1), modelObject.transform.rotation);
            Sequence seq2 = DOTween.Sequence();
            seq2.AppendInterval(0.4f);
            seq2.Append(ex.transform.DOMove(desPos, 0.5f).SetEase(Ease.InOutSine));
            seq2.Append(ex.transform.DOMove(oriPos, 0.55f).SetEase(Ease.InOutSine));
            seq2.AppendInterval(0.1f);
            seq2.AppendCallback(() => Destroy(ex));
        });
        seq.AppendInterval(2.2f);
        seq.AppendInterval(0.62f);

        Sequence seq3 = DOTween.Sequence();
        seq3.AppendInterval(1.0f);
        seq3.AppendCallback(() => takeDamageUnit.ForEach(s => s.TakeDamage(damage/5)));
        seq3.AppendInterval(0.1f);
        seq3.AppendCallback(() => takeDamageUnit.ForEach(s => s.TakeDamage(damage/5)));
        seq3.AppendInterval(0.1f);
        seq3.AppendCallback(() => takeDamageUnit.ForEach(s => s.TakeDamage(damage/5)));
        seq3.AppendInterval(0.1f);
        seq3.AppendCallback(() => takeDamageUnit.ForEach(s => s.TakeDamage(damage/5)));
        seq3.AppendInterval(0.1f);
        seq3.AppendCallback(() => takeDamageUnit.ForEach(s => s.TakeDamage(damage/5)));
        return 2.0f;
    }
}