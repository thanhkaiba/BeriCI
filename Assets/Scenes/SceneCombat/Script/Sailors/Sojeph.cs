using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class Sojeph : CombatSailor
{
    public SailorConfig config;
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

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(.8f);

        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(13);
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("knife2");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            KnifeToTarget(startPos, targetPos, 0f, 0.2f);
        });
        return 1;
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
        base.CastSkill(cbState);
        List<string> targets = new List<string>();
        List<float> _params = new List<float>();
        float healthGain = Model.config_stats.skill_params[0] * cs.Power;
        float main_damage = cs.Power;
        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        List<CombatSailor> teams = cbState.GetAllTeamAliveSailors(cs.team);
        CombatSailor healthTarget = TargetsUtils.LowestHealth(teams);
        targets.Add(healthTarget.Model.id);
        CombatSailor target = TargetsUtils.Range(this, enermy);
        targets.Add(target.Model.id);
        _params.Add(healthGain);
        _params.Add(target.CalcDamageTake(new Damage() { physics = main_damage }));
        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        var healthTarget = CombatState.Instance.GetSailor(targets[0]);
        var target = CombatState.Instance.GetSailor(targets[1]);
        CombatEvents.Instance.highlightTarget.Invoke(healthTarget);
        var healthGain = _params[0];
        var damage = _params[1];


        CombatEvents.Instance.highlightTarget.Invoke(target);

        var seq = DOTween.Sequence();
        seq.AppendInterval(1.0f);
        seq.AppendCallback(() => StartEffHealth(healthTarget, healthGain));
        seq.AppendInterval(.8f);
        seq.AppendCallback(() => {
            StartEffDame(target, damage);
        });
    
        return 3.5f;
    }
    public void StartEffHealth(CombatSailor target, float health)
    {
        SoundMgr.PlaySoundSkillSailor(14);
        Sequence seq = DOTween.Sequence();
        Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("Magic/Magic_00000");
        var startPos = gun2.GetWorldPosition(modelObject.transform);
        var targetPos = target.transform.position;
        //time = 3f / Vector3.Distance(startPos, targetPos);
        seq.AppendCallback(() =>
        {
            MedicineToTarget(startPos, targetPos, 0, .3f);
        });
        seq.AppendInterval(.3f);
        seq.AppendCallback(() =>
        {
            var pos = target.transform.position;
            pos.y += 4f;
            var eff = Instantiate(Resources.Load<GameObject>("Effect2D/buff/ef_24_green"), pos, Quaternion.identity);
            target.GainHealth(health);
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
        time = 5f / Vector3.Distance(startPos, targetPos);
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            KnifeToTarget(startPos, targetPos, 0, time);
        });
        seq.AppendInterval(time);
        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(13);
            target.LoseHealth(new Damage() { physics = damage });
        });
    }
    public void KnifeToTarget(Vector3 startPos, Vector3 targetPos, float delay, float flyTime)
    {
        var bulletGO = Instantiate(Resources.Load<GameObject>("Characters/Sojeph/knife/knife"), startPos, Quaternion.identity);
        bulletGO.SetActive(false);
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, targetPos);
        Vector3 desPos = Vector3.MoveTowards(oriPos, targetPos, d - 1.4f);
        int isFlip = 1;
        if (bulletGO.transform.position.x > desPos.x) isFlip = -1;
        bulletGO.transform.localScale = new Vector3(isFlip * 2, 2, 2);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.AppendCallback(() => bulletGO.SetActive(true));
        seq.Append(bulletGO.transform.DOMove(desPos, flyTime).SetEase(Ease.OutSine));
        seq.AppendInterval(flyTime);
        seq.AppendCallback(() => Destroy(bulletGO));

    }
    public void MedicineToTarget(Vector3 startPos, Vector3 targetPos, float delay, float flyTime)
    {
        var bulletGO = Instantiate(Resources.Load<GameObject>("Characters/Sojeph/Medicine/Medicine"), startPos, Quaternion.identity);
        bulletGO.SetActive(false);
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, targetPos);
        Vector3 desPos = Vector3.MoveTowards(oriPos, targetPos, d - 1.4f);
        targetPos.y += 1; 
        int isFlip = 1;
        if (bulletGO.transform.position.x > desPos.x) isFlip = -1;
        bulletGO.transform.localScale = new Vector3(isFlip * 2, 2, 2);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.AppendCallback(() => bulletGO.SetActive(true));
        seq.Append(bulletGO.transform.DOJump(targetPos,5,1, flyTime));
        seq.AppendInterval(.2f);
        seq.AppendCallback(() => Destroy(bulletGO));

    }
}
