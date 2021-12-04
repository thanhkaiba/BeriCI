using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galdalf : CombatSailor
{
    public SailorConfig config;
    public Galdalf()
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
        Vector3 targetPos = target.transform.position;
        targetPos.y += 3.4f;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.68f);
        seq.AppendCallback(() =>
        {
            Spine.Bone hand = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("shoot");

            Vector3 startPos = hand.GetWorldPosition(modelObject.transform);
            Vector3 endPos = target.transform.position;
            endPos.y += 2;
            GameEffMgr.Instance.TrailToTarget("Effect2D/magic_attack/Trail_purple", "Effect2D/118 sprite effects bundle/25 sprite effects/ef_22_purple", startPos, endPos, 0, .4f, .3f,.2f);

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
        CombatSailor _targets = TargetsUtils.GetSailorFrontSameTeam(this, CombatState.Instance.GetAllTeamAliveSailors(cs.team));
        if (_targets == null)
            return;
        float fury_buff = Model.config_stats.skill_params[0];
        float power_buff = cs.Power * Model.config_stats.skill_params[1];
        float speed_buff = Model.config_stats.skill_params[2];
        _targets.GainFury((int)fury_buff);
        _targets.GainPower(power_buff);
        _targets.SpeedUp(speed_buff);
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            Vector3 pos = _targets.transform.position;
            pos.y += 4f;
            var eff = Instantiate(Resources.Load<GameObject>("Effect2D/buff/ef_7_purple"), pos, Quaternion.identity);
            seq.AppendInterval(0.3f);
            seq.AppendCallback(() => Destroy(eff));
        });
        base.ActiveStartPassive();
    }
}

