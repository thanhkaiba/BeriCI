using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using UnityEngine;

public class Galdalf : CombatSailor
{
    private Spine.Bone boneTarget;
    public Galdalf()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
        boneTarget = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("shoot");
    }
    public override float RunBaseAttack(CombatSailor target)
    {

        Vector3 relativePos = transform.InverseTransformPoint(target.transform.position);
        relativePos.y += 4.5f;
        relativePos.x *= modelObject.transform.localScale.x;
        TriggerAnimation("Attack");
        boneTarget.SetLocalPosition(relativePos);
        SoundMgr.PlaySoundAttackSailor(1);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.68f);
        seq.AppendCallback(() =>
        {
            Spine.Bone hand = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("shoot");

            Vector3 startPos = hand.GetWorldPosition(modelObject.transform);
            Vector3 endPos = target.transform.position;
            endPos.y += 2;
            GameEffMgr.Instance.TrailToTarget("Effect2D/magic_attack/Trail_green", "Effect2D/118 sprite effects bundle/25 sprite effects/ef_13_purple", startPos, endPos, 0, .4f, .3f, .4f);

        });
        seq.AppendInterval(0.4f);
        return 1.1f;
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
        return false;
    }
    public override float CastSkill(CombatState cbState)
    {
        return base.CastSkill(cbState);
    }
    public override void ActiveStartPassive()
    {
        CombatSailor _targets = TargetsUtils.Front(this, CombatState.Instance.GetAllTeamAliveSailors(cs.team));
        if (_targets == null) return;

        TriggerAnimation("Attack");
        SoundMgr.PlaySoundSkillSailor(1);
        float fury_buff = Model.config_stats.skill_params[0];
        float speed_buff = Model.config_stats.skill_params[1];
        _targets.GainFury((int)fury_buff);
        _targets.SpeedUp(speed_buff);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.4f);
        seq.AppendCallback(() =>
        {
            _targets.AddStatus(new SailorStatus(SailorStatusType.EXCITED, 1));
        });
        base.ActiveStartPassive();
    }
}

