using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sojeph : CombatSailor
{
    public SailorConfig config;
    Vector3 startPos;
    Vector3 targetPos;
    Vector3 targetHealPos;
    CombatSailor mainTarget;
    CombatSailor healthTarget;
    float dame;
    float healthGain;
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
        targetPos = target.transform.position;
        targetPos.y += 2f;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(.8f);
        seq.AppendCallback(() =>
        {
            Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("knife2");
            Vector3 startPos = gun2.GetWorldPosition(modelObject.transform);
            GameEffMgr.Instance.BulletToTarget(startPos, targetPos, 0f, 0.2f);
        });
        return 0.8f;
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
        float main_damage = cs.Power;
        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        List<CombatSailor> teams = cbState.GetAllTeamAliveExceptSelfSailors(cs.team, this);   
        if(teams.Count > 0)
        {
            CombatSailor healthTarget = TargetsUtils.LowestHealth(teams);
            targets.Add(healthTarget.Model.id);
        }
       
        CombatSailor target = TargetsUtils.Range(this, enermy);
        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { physics = main_damage }));
        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        if (targets.Count == 2)
        {
            mainTarget = CombatState.Instance.GetSailor(targets[1]);
            healthTarget = CombatState.Instance.GetSailor(targets[0]);
            healthGain = cs.Power * Model.config_stats.skill_params[0];
            CombatEvents.Instance.highlightTarget.Invoke(healthTarget);
            targetHealPos = healthTarget.transform.position;
        }
        else 
        {
            mainTarget = CombatState.Instance.GetSailor(targets[0]);
            healthGain = -1;
        }
        dame = _params[0];
        CombatEvents.Instance.highlightTarget.Invoke(mainTarget); 
        Spine.Bone gun2 = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("knife2");
        startPos = gun2.GetWorldPosition(modelObject.transform);
        targetPos = mainTarget.transform.position;
        targetPos.y += 2;
        return 2;
    }
    public void StartEffHealth()
    {
        if (healthGain < 0)
            return;
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            Vector3 pos = targetHealPos;
            pos.y += 4f;
            var eff = Instantiate(Resources.Load<GameObject>("Effect2D/buff/ef_24_green"), pos, Quaternion.identity);
            healthTarget.GainHealth(healthGain);
            seq.AppendInterval(0.3f);
            seq.AppendCallback(() => Destroy(eff));
        });
        
       
    }
    public void StartEffDame()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            GameEffMgr.Instance.BulletToTarget(startPos, targetPos, .5f, 0.4f);
        });
        seq.AppendInterval(0.7f);
        seq.AppendCallback(() =>
        {
            mainTarget.LoseHealth(new Damage() { physics = (dame) });
        });
    }
}
