using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class FrozenAxe : Skill
{
    public int frozen_turn = 1;
    public float damage_ratio = 0.7f;
    public FrozenAxe()
    {
        skill_name = "Frozen Axe";
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
        float spell_damage = cChar.cs.Power;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cChar.cs.team);
        CombatSailor target = TargetsUtils.Melee(cChar, enermy);

        return RunAnimation(cChar, target);
    }
    float RunAnimation(CombatSailor attacking, CombatSailor target)
    {
        attacking.TriggerAnimation("BaseAttack");

        Vector3 oriPos = attacking.transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOMove(desPos, 0.3f));
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => {
            target.TakeDamage(damage_ratio * attacking.cs.Power, damage_ratio * attacking.cs.Power, 0);
            target.AddStatus(new SailorStatus(SailorStatusType.STUN, frozen_turn));
        });
        seq.Append(attacking.transform.DOMove(oriPos, 0.3f));
        return 0.8f;
    }
}