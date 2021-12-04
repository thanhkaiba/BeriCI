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
        Debug.Log(base.CanActiveSkill(cbState));
        return base.CanActiveSkill(cbState);
    }
    public override float CastSkill(CombatState cbState)
    {
        base.CastSkill(cbState);
        List<string> targets = new List<string>();
        List<float> _params = new List<float>();

        float magic_damage = cs.Power * Model.config_stats.skill_params[0];

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cs.team);
        List<CombatSailor> listTargets = TargetsUtils.AllFistRow(enermy);

        listTargets.ForEach(t =>
        {
            targets.Add(t.Model.id);
            _params.Add(t.CalcDamageTake(new Damage() { magic = magic_damage }));
        });

        return ProcessSkill(targets, _params);
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
        _targets.GainPower((int)power_buff);
        _targets.GainSpeed((int)speed_buff);
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
    /* public override float ProcessSkill(List<string> targets, List<float> _params)
     {
         Debug.Log("targets " + targets.Count);
         Debug.Log("_params " + _params.Count);
         base.ProcessSkill();
         //  TriggerAnimation("Skill");
         var listTargets = CombatState.Instance.GetSailors(targets);
         var listHighlight = new List<CombatSailor>() { this };
         listHighlight.AddRange(listTargets);
         CombatState.Instance.HighlightListSailor(listHighlight, 2.2f);
         Sequence seq = DOTween.Sequence();
         seq.AppendInterval(1f);
         seq.AppendCallback(() =>
         {
             for (int i = 0; i < listTargets.Count; i++)
             {
                 Spine.Bone bone = modelObject.GetComponent<SkeletonMecanim>().skeleton.FindBone("shoot");
                 Vector3 startPos = bone.GetWorldPosition(modelObject.transform);
                 Vector3 endPos = listTargets[i].transform.position;
                 endPos.y += 2;
                 // GameEffMgr.Instance.TrailToTarget("Effect2D/magic_attack/Trail_purple", "Effect2D/118 sprite effects bundle/25 sprite effects/ef_22_purple", startPos, endPos, 0, .4f);
             }


         });
         seq.AppendInterval(.5f);
         seq.AppendCallback(() =>
         {
             for (int i = 0; i < listTargets.Count; i++) listTargets[i].LoseHealth(new Damage() { magic = _params[i] });

         });

         return 3.5f;
     }*/
}

