using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Salvatafo : CombatSailor
{
    private Spine.Bone boneTarget;
    public Salvatafo()
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
        Sequence sq = DOTween.Sequence();
        sq.AppendInterval(0.5f);
        sq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(8);
        });
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.4f);
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.5f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return 0.8f;
    }
    public override void SetFaceDirection()
    {
        if (modelObject.activeSelf) modelObject.transform.localScale = new Vector3(cs.team == Team.A ? 1f : -1f, 1f, 1f);
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

        float magic_damage_ratio = Model.config_stats.skill_params[0];
        float healt_ratio = Model.config_stats.skill_params[1];

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        CombatSailor main_target = TargetsUtils.Melee(this, enermy);
        List<CombatSailor> aroundPlusTarget = TargetsUtils.AroundPlus(main_target, enermy);

        float magic_damage = cs.Power * magic_damage_ratio;
        float health = (1 + aroundPlusTarget.Count) * healt_ratio * cs.Power;

        targets.Add(main_target.Model.id);
        _params.Add(main_target.CalcDamageTake(new Damage() { magic = magic_damage }));

        aroundPlusTarget.ForEach(t =>
        {
            targets.Add(t.Model.id);
            _params.Add(t.CalcDamageTake(new Damage() { magic = magic_damage }));
        });

        _params.Add(health);

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        Debug.Log("targets " + targets.Count);
        Debug.Log("_params " + _params.Count);
        base.ProcessSkill();
        TriggerAnimation("Skill");
        SoundMgr.PlaySoundSkillSailor(9);
        var listTargets = CombatState.Instance.GetSailors(targets);
        var mainTarget = listTargets[0];
        Vector3 oriPos = transform.position;
        int offset = transform.position.x < mainTarget.transform.position.x ? -1 : 1;

        var targetPos = mainTarget.transform.position;
        targetPos.z -= 0.1f;
        Vector3 desPos = new Vector3(
            targetPos.x + offset * 4,
            targetPos.y,
            targetPos.z - 0.1f
        );

        GameObject shadow = modelObject.transform.Find("shadow").gameObject;
        {
            Vector3 relativePos = transform.InverseTransformPoint(mainTarget.transform.position);
            relativePos.x *= modelObject.transform.localScale.x;
            boneTarget.SetLocalPosition(relativePos);
        }
        CombatState.Instance.HighlightSailor2Step(this, listTargets, 1.0f, 2.2f);
        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(0.8f);
        seq.AppendCallback(() =>
        {
            shadow.SetActive(false);
            bar.gameObject.SetActive(false);
        });
        seq.AppendInterval(1.5f);
        seq.AppendCallback(() =>
        {
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("arm_L_03");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            GameObject ex = Instantiate(Resources.Load<GameObject>(
                "Effect2D/Duong_FX/VFX_Piratera/salvatafo_projectile_skill"),
                startPos,
                new Quaternion());
            Sequence seq2 = DOTween.Sequence();
            ex.transform.localScale = new Vector3(modelObject.transform.localScale.x, 1, 1);
            Vector3 desPos = new Vector3(mainTarget.transform.position.x, mainTarget.transform.position.y + 3, mainTarget.transform.position.z + 1f);
            seq2.Append(ex.transform.DOMove(desPos, 0.2f));
            seq2.AppendInterval(0.4f);
            seq2.AppendCallback(() => Destroy(ex));
        });
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {
            Vector3 pos = new Vector3(mainTarget.transform.position.x, mainTarget.transform.position.y, mainTarget.transform.position.z + 5);
            GameObject ex = Instantiate(Resources.Load<GameObject>(
                "Effect2D/Duong_FX/VFX_Piratera/salvatafo_impact_skill"),
                pos,
                new Quaternion());
            Sequence seq2 = DOTween.Sequence();
            ex.transform.localScale = new Vector3(3.0f, 3.5f, 2.0f);
            seq2.AppendInterval(2.0f);
            seq2.AppendCallback(() => Destroy(ex));
        });
        seq.AppendInterval(0.0f);
        seq.AppendCallback(() =>
        {
            for (int i = 0; i < listTargets.Count; i++)
                listTargets[i].LoseHealth(new Damage() { magic = _params[i] });
        });
        seq.AppendInterval(0.4f);
        seq.AppendCallback(() =>
        {
            GainHealth(_params[listTargets.Count]);
            shadow.SetActive(true);
            bar.gameObject.SetActive(true);
        });
        seq.AppendInterval(0.7f);
        return 3.2f;
    }
}