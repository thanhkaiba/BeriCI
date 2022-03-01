using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Kamijita : CombatSailor
{
    //private Spine.Bone boneTarget;
    public Kamijita()
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
        seq.AppendCallback(() => SoundMgr.Instance.PlaySoundEffect(Resources.Load<AudioClip>("Audio/Sailor/play_harp")));
        seq.AppendInterval(.4f);
        seq.AppendCallback(() =>
        {

            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("shoot");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            KnifeToTarget(startPos, targetPos, 0.2f);
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
        List<CombatSailor> teams = cbState.GetAllTeamAliveSailors(cs.team);
        CombatSailor target = TargetsUtils.FurthestMaxFury(teams);
        return target.cs.MaxFury - target.cs.Fury > 0;
    }
    public override float CastSkill(CombatState cbState)
    {
        base.CastSkill(cbState);
        List<string> targets = new List<string>();
        List<float> _params = new List<float>();
        float furyGain = Model.config_stats.skill_params[0] * cs.Power;
        List<CombatSailor> teams = cbState.GetAllTeamAliveExceptSelfSailors(cs.team, this);
        CombatSailor buffTarget = TargetsUtils.FurthestMaxFury(teams);
        targets.Add(buffTarget.Model.id);
        _params.Add(furyGain);
        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        Debug.Log("targets length: " + targets.Count);
        var buffTarget = CombatState.Instance.GetSailor(targets[0]);
        var furyGain = _params[0];
        var speedUp = _params[1];
        Debug.Log("kami_params 0" + _params[0]);
        Debug.Log("kami_params 1" + _params[1]);

        var seq = DOTween.Sequence();
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() => StartEffBuff(buffTarget, (int)furyGain, speedUp));

        return 2.4f;
    }
    public void StartEffBuff(CombatSailor target, int furyGain, float speedUp)
    {
        SoundMgr.Instance.PlaySoundEffect(Resources.Load<AudioClip>("Audio/Sailor/play_harp"));
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("shoot");
            var startPos = gun2.GetWorldPosition(modelObject.transform);
            var targetPos = target.transform.position;
            targetPos.y += 2f;
            targetPos.z -= 0.2f;
            MedicineToTarget(startPos, targetPos, 1.0f);
        });
        seq.AppendInterval(1.0f);
        seq.AppendCallback(() =>
        {
            target.GainFury(furyGain);
            target.SpeedUp(speedUp);
            var pos = target.transform.position;
            pos.y += 2.8f;
            var eff = Instantiate(Resources.Load<GameObject>("Effect2D/Duong_FX/VFX_Piratera/fx_speed"), pos, Quaternion.identity);
            eff.transform.localScale = Vector3.one * 1.8f;
            seq.AppendInterval(2f);
            seq.AppendCallback(() => Destroy(eff));
        });
    }
    public void StartEffDame(CombatSailor target, float damage)
    {
        float time = 0;
        Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("shoot");
        var startPos = gun2.GetWorldPosition(modelObject.transform);
        var targetPos = target.transform.position;
        targetPos.y += 3;
        targetPos.z += 0.2f;
        time = 5f / Vector3.Distance(startPos, targetPos);
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(13);
            KnifeToTarget(startPos, targetPos, .2f);
        });
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {

            target.LoseHealth(new Damage() { physics = damage });
        });
    }
    public void KnifeToTarget(Vector3 startPos, Vector3 targetPos, float flyTime)
    {
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, targetPos);
        Vector3 desPos = targetPos;
            var bulletGO = Instantiate(Resources.Load<GameObject>("Effect2D/Duong_FX/VFX_Piratera/kamijita_attack"), startPos, Quaternion.identity);
            bulletGO.SetActive(false);
            Vector3 theScale = transform.localScale;
            if (startPos.x > desPos.x) theScale.x = -1;
            else theScale.x = 1;

            bulletGO.transform.localScale = theScale * 1f;
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() => bulletGO.SetActive(true));
            seq.Append(bulletGO.transform.DOMove(desPos, flyTime).SetEase(Ease.Linear));
            seq.AppendInterval(flyTime);
            seq.AppendCallback(() => Destroy(bulletGO)).SetLink(bulletGO).SetTarget(bulletGO);

    }
    public void MedicineToTarget(Vector3 startPos, Vector3 targetPos, float flyTime)
    {
        Debug.Log("Medicine");
        var bulletGO = Instantiate(Resources.Load<GameObject>("Effect2D/Duong_FX/VFX_Piratera/kamijita_skill"), startPos, Quaternion.identity);
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, targetPos);
        Vector3 desPos = Vector3.MoveTowards(oriPos, targetPos, d - 1.4f);
        targetPos.y += 2.2f;
        bulletGO.transform.localScale = Vector3.one * 2;
        Sequence seq = DOTween.Sequence();
        seq.Append(bulletGO.transform.DOJump(targetPos, 0.5f, 1, flyTime).SetEase(Ease.InSine));
        seq.AppendCallback(() => Destroy(bulletGO));
    }
}
