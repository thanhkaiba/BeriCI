using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class IceArrow : Skill
{
    public int frozen_turn = 1;
    public float scale_damage = 2.0f;
    public IceArrow()
    {
        skill_name = "Ice Arrow";
        MAX_FURY = 50;
        START_FURY = 0;
    }
    public override bool CanActive(CombatSailor cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatSailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cChar.cs.team);
        CombatSailor target = TargetsUtils.Range(cChar, enermy);

        return RunAnimation(cChar, target);
    }
    float RunAnimation(CombatSailor attacking, CombatSailor target)
    {
        float delay = attacking.RunBaseAttack(target);

        Vector3 oriPos = attacking.transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.AppendCallback(() => {
            target.TakeDamage(0, attacking.cs.Power *scale_damage);
            target.AddStatus(new SailorStatus(SailorStatusType.FROZEN, frozen_turn));
        });
        return delay + 0.2f;
    }
}