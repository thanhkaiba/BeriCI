using DG.Tweening;
using Piratera.Sound;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class Nael : CombatSailor
{
    public Nael()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    public override void GainFury(int value)
    {
        base.GainFury(value);
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        TriggerAnimation("Attack");
        Vector3 oriPos = transform.position;
        Debug.Log("target: " + target);
        Debug.Log("target id: " + target.Model.id);
        Debug.Log("target id: " + target.Model.config_stats.root_name);
        int offset = transform.position.x < target.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 8,
            target.transform.position.y,
            target.transform.position.z - 0.1f
        );
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        seq.AppendInterval(.3f);
        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(0);
        });
        seq.AppendInterval(0.8f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return .65f;
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
        float shield = Model.config_stats.skill_params[0] * cs.Power;

        targets.Add(Model.id);
        List<CombatSailor> teams = cbState.GetAllTeamAliveExceptSelfSailors(cs.team, this);
        Debug.Log("teams.Count: " + teams.Count);
        if (teams.Count > 0)
        {
            CombatSailor buff_target = TargetsUtils.Riskest(teams);
            targets.Add(buff_target.Model.id);
            _params.Add(shield);
            _params.Add(shield);
        }
        else
        {
            _params.Add(shield * 2);
        }
        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        List<CombatSailor> alies = CombatState.Instance.GetSailors(targets);
        for (int  i = 0; i < alies.Count; i++)
        {
            var pos = alies[i].transform.position;
            pos.y += 2.8f;
            pos.z -= 0.1f;

            var eff = Instantiate(Resources.Load<GameObject>("Effect2D/Duong_FX/VFX_Piratera/fx_speed"), pos, Quaternion.identity);
            eff.transform.localScale = Vector3.one * 1.8f;
            Sequence seq2 = DOTween.Sequence();
            seq2.AppendInterval(2f);
            seq2.AppendCallback(() => Destroy(eff));
        }
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.8f);
        seq.AppendCallback(() => {
            for (int i = 0; i < alies.Count; i++)
                alies[i].AddShield(_params[i]);
        });
        return 2f;
    }
}
