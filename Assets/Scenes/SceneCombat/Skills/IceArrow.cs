using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class IceArrow : Skill
{
    public int frozen_turn = 1;
    public IceArrow()
    {
        name = "Ice Arrow";
        MAX_FURY = 20;
        START_FURY = 0;
    }
    public override bool CanActive(CombatCharacter cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);

        List<CombatCharacter> enermy = cbState.GetAliveCharacterEnermy(cChar.team);
        CombatCharacter target = GetNearestInRowTarget(cChar, enermy);

        return RunAnimation(cChar, target);
    }
    float RunAnimation(CombatCharacter attacking, CombatCharacter target)
    {
        float delay = attacking.display.BaseAttack(target);

        Vector3 oriPos = attacking.transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.AppendCallback(() => {
            target.TakeDamage(0, 0, attacking.current_power);
            target.AddStatus(new CombatCharacterStatus(CombatCharacterStatusName.FROZEN, frozen_turn));
        });
        return delay;
    }
}