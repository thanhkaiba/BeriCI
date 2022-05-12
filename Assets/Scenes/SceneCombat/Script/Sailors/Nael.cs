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
            GameEffMgr.Instance.CreateSpeedEffect(pos);

            if (i == 1)
            {
                var startPos = transform.position;
                var targetPos = alies[i].transform.position;
                targetPos.y += 2f;
                targetPos.z -= 0.2f;
                MedicineToTarget(startPos, targetPos, 1.8f);
            }
        }
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.85f);
        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(0);
            alies[0].AddShield(_params[0]);
            SoundMgr.PlaySound("Audio/Sailor/water_move");
        });
        seq.AppendInterval(0.7f);
        seq.AppendCallback(() => {
            if  (alies.Count == 2) alies[1].AddShield(_params[1]);
        });
        return 2f;
    }
    public void MedicineToTarget(Vector3 startPos, Vector3 targetPos, float flyTime)
    {
        startPos.z -= 0.5f;
        startPos.y += 1f;
        var bulletGO = Instantiate(Resources.Load<GameObject>("Effect2D/wave/Prefab/Wave_Trail"), startPos, Quaternion.identity);
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, targetPos);
        Vector3 desPos = Vector3.MoveTowards(oriPos, targetPos, d - 1.4f);
        targetPos.y += 1.5f;
        targetPos.z -= 0.5f;
        bulletGO.transform.localScale = Vector3.one * 8;
        bulletGO.SetActive(false);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            bulletGO.SetActive(true);
        });
        seq.Append(bulletGO.transform.DOJump(targetPos, 2.0f, 1, flyTime - 0.75f).SetEase(Ease.InSine));
        seq.AppendCallback(() => Destroy(bulletGO));
    }
}
