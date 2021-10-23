using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UltimateSlash : Skill
{
    public float base_heal = 40;
    public float scale_damage = 2.0f;
    public UltimateSlash()
    {
        skill_name = "Ultimate Slash";
        MAX_FURY = 30;
        START_FURY = 0;
    }
    public override bool CanActive(CombatSailor cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatSailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float true_damage = cChar.cs.Power * scale_damage;
        float heal = base_heal + cChar.cs.Power;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cChar.cs.team);
        CombatSailor target = GetMeleeTarget(cChar, enermy);

        return RunAnimation(cChar, target, true_damage, heal);
    }
    float RunAnimation(CombatSailor attacking, CombatSailor target, float damage, float heal)
    {
        Vector3 oriPos = attacking.transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 2.0f);
        attacking.TriggerAnimation("BaseAttack");
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOMove(desPos, 0.4f));
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() => {
            target.TakeDamage(0, 0, damage);
            attacking.GainHealth(heal);
        });
        seq.Append(attacking.transform.DOMove(oriPos, 0.2f));
        return 0.7f;
    }
}