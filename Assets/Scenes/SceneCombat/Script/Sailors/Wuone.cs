using DG.Tweening;
using Piratera.Config;
using Piratera.Sound;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Wuone : CombatSailor
{
    private Spine.Bone boneTarget;
    private Spine.Bone boneArr;
    public Wuone()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
        boneTarget = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("target");
        boneArr = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("crossbow");
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        Vector3 targetPos = target.transform.position;
        targetPos.y += 3.0f;
        {
            Vector3 relativePos = transform.InverseTransformPoint(targetPos);
            relativePos.x *= modelObject.transform.localScale.x;
            boneTarget.SetLocalPosition(relativePos);
        }

        TriggerAnimation("Attack");
        Sequence sq = DOTween.Sequence();

        StartCoroutine(GameUtils.WaitAndDo(0.55f, () => SoundMgr.PlaySound("Audio/Sailor/slide_hit")));
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.7f);
        seq.AppendCallback(() =>
        {
            Vector3 startPos = boneArr.GetWorldPosition(modelObject.transform);
            ArrowTarget(startPos, targetPos, 0.005f * Vector3.Distance(startPos, targetPos));
        });

        Vector3 startPos = boneArr.GetWorldPosition(modelObject.transform);
        float flyTime = 0.005f * Vector3.Distance(startPos, targetPos);
        return 0.7f + flyTime;
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
        float main_damage = cs.Power * scale_damage_ratio;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnemy(cs.team);
        CombatSailor target = TargetsUtils.Range(this, enermy);

        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { magic = main_damage }, this));

        SynergiesConfig config = GlobalConfigs.Synergies;
        ClassBonusItem wild = CombatState.Instance.GetTeamClassBonus(cs.team, SailorClass.WILD);
        float healthGain = 0;
        if (cs.HaveType(SailorClass.WILD) && wild != null)
        {
            float percentHealthGain = config.GetParams(wild.type, wild.level)[0];
            healthGain = percentHealthGain * cs.MaxHealth;
        }
        _params.Add(healthGain);
        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        CombatSailor target = CombatState.Instance.GetSailor(targets[0]);
        float loseHealth = _params[0];

        CombatState.Instance.HighlightSailor2Step(this, new List<CombatSailor> { this }, 0.5f, 0.5f);
        TriggerAnimation("Skill");
        {
            Vector3 relativePos = transform.InverseTransformPoint(target.transform.position);
            relativePos.y += 1.5f;
            relativePos.x *= modelObject.transform.localScale.x;
            boneTarget.SetLocalPosition(relativePos);
        }

        Vector3 targetPos = target.transform.position;
        targetPos.y += 3.0f;
        targetPos.z -= 0.5f;
        StartCoroutine(GameUtils.WaitAndDo(0.25f, () => SoundMgr.PlaySound("Audio/Sailor/mechanical")));
        StartCoroutine(GameUtils.WaitAndDo(0.7f, () => SoundMgr.PlaySound("Audio/Sailor/wing_flap_2")));
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.7f);
        seq.AppendCallback(() =>
        {
            GainHealth(_params[1]);
            Vector3 startPos = boneArr.GetWorldPosition(modelObject.transform);
            NetToTarget(startPos, targetPos, 0.02f * Vector3.Distance(startPos, targetPos));
        });
        seq.AppendInterval(0.4f);
        seq.AppendCallback(() =>
        {
            target.AddStatus(new SailorStatus(SailorStatusType.STUN, 1));
            target.LoseHealth(new Damage() { magic = loseHealth });
        });
        return 1.8f;
    }
    private void ArrowTarget(Vector3 startPos, Vector3 targetPos, float flyTime)
    {
        var arrGO = Instantiate(Resources.Load<GameObject>("Characters/Herminia/arrow/arrow"), startPos, Quaternion.identity);
        arrGO.SetActive(false);

        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, targetPos);
        Vector3 desPos = Vector3.MoveTowards(oriPos, targetPos, d - 1.4f);

        float rZ = (float)Math.Atan2(targetPos.y - startPos.y, targetPos.x - startPos.x);
        arrGO.transform.eulerAngles = new Vector3(0, 0, rZ * 57.3f);

        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => arrGO.SetActive(true));
        seq.Append(arrGO.transform.DOMove(desPos, flyTime).SetEase(Ease.OutSine));
        seq.AppendCallback(() => Destroy(arrGO));

        arrGO.transform.Find("eff").gameObject.SetActive(false);
    }
    private void NetToTarget(Vector3 startPos, Vector3 targetPos, float flyTime)
    {
        var bulletGo = new GameObject();
        bulletGo.transform.position = startPos;
        bulletGo.transform.localScale = Vector3.one * 0.2f;
        var image = bulletGo.AddComponent<SpriteRenderer>();
        image.sprite = Resources.Load<Sprite>("Characters/Wuone/Net");
        bulletGo.SetActive(false);

        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, targetPos);
        Vector3 desPos = Vector3.MoveTowards(oriPos, targetPos, d - 0.4f);

        float rZ = (float)Math.Atan2(targetPos.y - startPos.y, targetPos.x - startPos.x);
        bulletGo.transform.eulerAngles = new Vector3(0, 0, rZ * 57.3f);
        bulletGo.transform.DOScale(Vector3.one * 0.8f, flyTime).SetEase(Ease.InSine);
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => bulletGo.SetActive(true));
        seq.Append(bulletGo.transform.DOMove(desPos, flyTime).SetEase(Ease.InSine));
        seq.Append(image.DOFade(0, 1.0f));
        seq.AppendCallback(() => Destroy(bulletGo));
    }
}
