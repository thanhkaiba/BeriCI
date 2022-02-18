using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Sojeph : CombatSailor
{
    //private Spine.Bone boneTarget;
    public Sojeph()
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
        var targetPos = target.transform.position;
        targetPos.y += 3f;
        targetPos.z += 0.2f;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(.4f);
        seq.AppendCallback(() => SoundMgr.PlaySoundAttackSailor(13));
        seq.AppendInterval(.4f);
        seq.AppendCallback(() =>
        {

            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("knife2");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            KnifeToTarget(startPos, targetPos, 0f, 0.2f);
        });
        return 0.95f;
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
        return CombatState.Instance.GetAllTeamAliveExceptSelfSailors(cs.team, this).Count >= 1;
    }
    public override float CastSkill(CombatState cbState)
    {
        base.CastSkill(cbState);
        List<string> targets = new List<string>();
        List<float> _params = new List<float>();
        float healthGain = Model.config_stats.skill_params[0] * cs.Power;
        float main_damage = cs.Power;
        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        List<CombatSailor> teams = cbState.GetAllTeamAliveExceptSelfSailors(cs.team, this);
        CombatSailor healthTarget = TargetsUtils.LowestHealth(teams);
        targets.Add(healthTarget.Model.id);
        CombatSailor target = TargetsUtils.Range(this, enermy);
        targets.Add(target.Model.id);
        _params.Add(healthGain);
        _params.Add(target.CalcDamageTake(new Damage() { physics = main_damage }, this));
        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        var healthTarget = CombatState.Instance.GetSailor(targets[0]);
        var target = CombatState.Instance.GetSailor(targets[1]);
        var healthGain = _params[0];
        var damage = _params[1];

        var seq = DOTween.Sequence();
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() => StartEffHealth(healthTarget, healthGain));
        seq.AppendInterval(0.8f);
        seq.AppendCallback(() => StartEffDame(target, damage));

        return 1.8f;
    }
    public void StartEffHealth(CombatSailor target, float health)
    {
        SoundMgr.PlaySoundSkillSailor(14);
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("Medicine");
            var startPos = gun2.GetWorldPosition(modelObject.transform);
            var targetPos = target.transform.position;
            targetPos.y += 2f;
            targetPos.z -= 0.2f;
            MedicineToTarget(startPos, targetPos, 1.0f);
        });
        seq.AppendInterval(1.0f);
        seq.AppendCallback(() =>
        {
            target.GainHealth(health);
            var pos = target.transform.position;
            pos.y += 4f;
            var eff = Instantiate(Resources.Load<GameObject>("Effect2D/buff/ef_24_green"), pos, Quaternion.identity);
            seq.AppendInterval(0.3f);
            seq.AppendCallback(() => Destroy(eff));
        });
    }
    public void StartEffDame(CombatSailor target, float damage)
    {
        float time = 0;
        Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("knife2");
        var startPos = gun2.GetWorldPosition(modelObject.transform);
        var targetPos = target.transform.position;
        targetPos.y += 3;
        targetPos.z += 0.2f;
        time = 5f / Vector3.Distance(startPos, targetPos);
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(13);
            KnifeToTarget(startPos, targetPos, 0, .2f);
        });
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {

            target.LoseHealth(new Damage() { physics = damage });
        });
    }
    public void KnifeToTarget(Vector3 startPos, Vector3 targetPos, float delay, float flyTime)
    {
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, targetPos);
        Vector3 desPos = targetPos;
        for (int i = 0; i < 3; i++)
        {
            var bulletGO = Instantiate(Resources.Load<GameObject>("Characters/Sojeph/knife/knife"), startPos - new Vector3(0, i * 0.2f), Quaternion.identity);
            bulletGO.SetActive(false);
            Vector3 theScale = transform.localScale;
            if (startPos.x > desPos.x) theScale.x = -1;
            else theScale.x = 1;

            float rZ = (float)Math.Atan2(targetPos.y - startPos.y, targetPos.x - startPos.x);
            bulletGO.transform.eulerAngles = new Vector3(0, 0, rZ * 57.3f + (theScale.x == -1 ? 180 : 0));

            bulletGO.transform.localScale = theScale * 4f;
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(delay + i * 0.02f);
            seq.AppendCallback(() => bulletGO.SetActive(true));
            seq.Append(bulletGO.transform.DOMove(desPos, flyTime).SetEase(Ease.Linear));
            seq.Join(bulletGO.transform.DOScaleX(2 * theScale.x, flyTime));
            seq.Join(bulletGO.transform.DOScaleY(2, flyTime));
            seq.Join(bulletGO.transform.DOScaleZ(2, flyTime));
            seq.AppendInterval(flyTime - i * 0.02f);
            seq.AppendCallback(() => Destroy(bulletGO)).SetLink(bulletGO).SetTarget(bulletGO);
        }


    }
    public void MedicineToTarget(Vector3 startPos, Vector3 targetPos, float flyTime)
    {
        Debug.Log("Medicine");
        var bulletGO = Instantiate(Resources.Load<GameObject>("Characters/Sojeph/Medicine/Medicine"), startPos, Quaternion.identity);
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, targetPos);
        Vector3 desPos = Vector3.MoveTowards(oriPos, targetPos, d - 1.4f);
        targetPos.y += 1;
        int isFlip = 1;
        if (bulletGO.transform.position.x > desPos.x) isFlip = -1;
        bulletGO.transform.localScale = new Vector3(isFlip * 2, 2, 2);
        Sequence seq = DOTween.Sequence();
        seq.Append(bulletGO.transform.DOJump(targetPos, 5, 1, flyTime).SetEase(Ease.InSine));
        seq.AppendCallback(() => Destroy(bulletGO));
    }
}
