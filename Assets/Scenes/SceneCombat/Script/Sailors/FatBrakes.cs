using DG.Tweening;
using Piratera.Sound;
using System.Collections.Generic;
using UnityEngine;

public class FatBrakes : CombatSailor
{
    private GameObject circle;
    public FatBrakes()
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
        int offset = transform.position.x < target.transform.position.x ? 1 : -1;
        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 4,
            target.transform.position.y,
            target.transform.position.z - 0.1f
        );
        Sequence seq = DOTween.Sequence();
        StartCoroutine(GameUtils.WaitAndDo(0.35f, () => SoundMgr.PlaySoundAttackSailor(2)));
        StartCoroutine(GameUtils.WaitAndDo(0.5f, () => GameEffMgr.Instance.Shake(0.1f, 0.5f)));
        seq.AppendInterval(0.25f);
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        float currentLocalScaleX = modelObject.transform.localScale.x;
        seq.Append(modelObject.transform.DOScaleX(currentLocalScaleX * -1, 0.0f));
        seq.AppendInterval(0.8f);
        seq.Append(modelObject.transform.DOScaleX(currentLocalScaleX, 0.0f));
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return 0.7f;
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
        List<string> targets = new List<string>();
        List<float> _params = new List<float>();

        base.CastSkill(cbState);

        List<CombatSailor> listTargets = cbState.GetAllTeamAliveSailors(cs.team);
        listTargets.ForEach(sailor => targets.Add(sailor.Model.id));

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        var listTargets = CombatState.Instance.GetSailors(targets);
        TriggerAnimation("Skill");
        GameObject ex = Instantiate(Resources.Load<GameObject>("Effect2D/FatBrakesBall/VFX_Piratera/fatbrakes_skill"), transform);
        Sequence seq2 = DOTween.Sequence();
        ex.transform.localScale = ex.transform.localScale * 2;
        ex.transform.DOLocalMoveY(5, 3);
        seq2.AppendInterval(1.0f);
        seq2.AppendCallback(() => listTargets.ForEach(sailor =>
        {
            sailor.AddStatus(new SailorStatus(SailorStatusType.EXCITED, 1));
        }));
        seq2.AppendInterval(3.0f);
        seq2.AppendCallback(() => Destroy(ex));
        base.ProcessSkill();
        return 2.0f;
    }
}