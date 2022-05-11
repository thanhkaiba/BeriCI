using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zeke : CombatSailor
{
    private Spine.Bone boneTarget;
    private Spine.Bone boneArr;
    public Zeke()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
        boneTarget = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("target");
        boneArr = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("shoot");
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        Vector3 relativePos = transform.InverseTransformPoint(target.transform.position);
        relativePos.y += 2.5f;
        relativePos.x *= modelObject.transform.localScale.x;
        TriggerAnimation("Attack");
        boneTarget.SetLocalPosition(relativePos);

        Vector3 targetPos = target.transform.position;
        targetPos.y += 3.0f;

        SoundMgr.PlaySound("Audio/Sailor/gear");
        StartCoroutine(GameUtils.WaitAndDo(0.55f, () => SoundMgr.PlaySound("Audio/Sailor/air_whistle_punch")));

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.7f);
        seq.AppendCallback(() =>
        {
            Vector3 startPos = boneArr.GetWorldPosition(modelObject.transform);
            BulletToTarget(startPos, targetPos, 0.01f * Vector3.Distance(startPos, targetPos));
        });


        Vector3 startPos = boneArr.GetWorldPosition(modelObject.transform);
        float flyTime = 0.01f * Vector3.Distance(startPos, targetPos);
        return flyTime + 0.7f;
    }

    public override float TakeDamage(Damage d)
    {
        TriggerAnimation("Hurt");
        return base.TakeDamage(d);
    }
    // skill
    public override bool CanActiveSkill(CombatState cbState)
    {
        List<CombatSailor> enermy = cbState.GetAliveCharacterEnemy(cs.team);
        CombatSailor target = TargetsUtils.Range(this, enermy);
        return target.cs.position.x > 0;
    }
    public override float CastSkill(CombatState cbState)
    {
        List<string> targets = new List<string>();
        List<float> _params = new List<float>();

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnemy(cs.team);
        CombatSailor target = TargetsUtils.Range(this, enermy);

        float arg0 = Model.config_stats.skill_params[0];
        float arg1 = Model.config_stats.skill_params[1];
        float DAM = cs.Power + (arg0 + arg1 * Model.star) * target.cs.CurHealth;

        var isCrit = IsCrit();
        if (isCrit) DAM *= cs.CritDamage;

        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { physics = DAM }, this));
        _params.Add(isCrit ? 1 : -1);

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        CombatSailor target = CombatState.Instance.GetSailor(targets[0]);
        float loseHealth = _params[0];

        CombatState.Instance.HighlightListSailor(new List<CombatSailor> { this }, 1.0f);

        target.cs.position.x = 0;
        var newTargetPos = target.GetScenePosition();

        SoundMgr.PlaySound("Audio/Sailor/gear");
        StartCoroutine(GameUtils.WaitAndDo(0.55f, () => SoundMgr.PlaySound("Audio/Sailor/air_whistle_punch")));
        StartCoroutine(GameUtils.WaitAndDo(1.0f, () => SoundMgr.PlaySoundSkillSailor(3)));

        Vector3 oriPos = transform.position;
        Sequence seq = DOTween.Sequence();
        //if (cs.position.y == target.cs.position.y) seq.AppendInterval(0.3f);
        //else seq.Append(transform.DOMove(new Vector3(0f, target.transform.position.y, target.transform.position.z + 0.1f), 0.3f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() => {
            Vector3 relativePos = transform.InverseTransformPoint(target.transform.position);
            relativePos.y += 3f;
            relativePos.x *= modelObject.transform.localScale.x;
            boneTarget.SetLocalPosition(relativePos);
        });
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => {
            target.LoseHealth(new Damage() { physics = loseHealth, isCrit = _params[1] > 0 });
        });
        seq.AppendInterval(0.4f);
        seq.Append(target.transform.DOMove(newTargetPos, 0.3f).SetEase(Ease.OutBack));
        seq.AppendInterval(0.2f);
        seq.Append(transform.DOMove(oriPos, 0.15f).SetEase(Ease.OutSine));
        return 2.1f;
    }
    private void BulletToTarget(Vector3 startPos, Vector3 targetPos, float flyTime)
    {
        var bulletGo = new GameObject();
        bulletGo.transform.position = startPos;
        bulletGo.transform.localScale = Vector3.one * 1.2f;
        var image = bulletGo.AddComponent<SpriteRenderer>();
        image.sprite = Resources.Load<Sprite>("Characters/Zeke/Bullet");
        bulletGo.SetActive(false);

        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, targetPos);
        Vector3 desPos = Vector3.MoveTowards(oriPos, targetPos, d - 1.4f);

        float rZ = (float)Math.Atan2(targetPos.y - startPos.y, targetPos.x - startPos.x);
        bulletGo.transform.eulerAngles = new Vector3(0, 0, rZ * 57.3f);

        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => bulletGo.SetActive(true));
        seq.Append(bulletGo.transform.DOMove(desPos, flyTime).SetEase(Ease.InSine));
        seq.AppendCallback(() => Destroy(bulletGo));
    }
}
