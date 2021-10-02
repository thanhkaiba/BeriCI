using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Slash : Skill
{
    public float scale_damage_ratio = 2;
    public float behind_damage_ratio = 0.7f;
    public Slash()
    {
        name = "Slash";
        MAX_FURY = 10;
        START_FURY = 7;
    }
    public override bool CanActive(Sailor cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(Sailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float physic_damage = cChar.cs.Power * scale_damage_ratio;

        List<Sailor> enermy = cbState.GetAliveCharacterEnermy(cChar.cs.team);
        Sailor target = GetNearestTarget(cChar, enermy);
        Sailor behind_target = GetBehind(target, enermy);

        return RunAnimation(cChar, target, behind_target, physic_damage);
    }
    float RunAnimation(Sailor attacking, Sailor target, Sailor behind_target, float physic_damage)
    {
        attacking.TriggerAnimation("BaseAttack");
        GameEvents.instance.highlightTarget.Invoke(target);
        Vector3 oriPos = attacking.transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOMove(desPos, 0.3f));
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => {
            target.TakeDamage(physic_damage, 0, 0);
            if (behind_target != null) behind_target.TakeDamage(physic_damage*behind_damage_ratio, 0, 0);
        });
        seq.Append(attacking.transform.DOMove(oriPos, 0.3f));
        return 0.8f;
    }
}