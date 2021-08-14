using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class FrozenAxe : Skill
{
    public int frozen_turn = 1;
    public float damage_ratio = 1.0f;
    public FrozenAxe()
    {
        name = "Frozen Axe";
        MAX_FURY = 30;
        START_FURY = 0;
    }
    public override bool CanActive(CombatCharacter cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float spell_damage = cChar.current_power;

        List<CombatCharacter> enermy = cbState.GetAliveCharacterEnermy(cChar.team);
        CombatCharacter target = GetNearestTarget(cChar, enermy);

        return RunAnimation(cChar, target);
    }
    float RunAnimation(CombatCharacter attacking, CombatCharacter target)
    {
        attacking.display.TriggerAnimation("BaseAttack");

        Vector3 oriPos = attacking.transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOMove(desPos, 0.3f));
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => {
            target.TakeDamage(damage_ratio * attacking.current_power, damage_ratio * attacking.current_power, 0);
            target.AddStatus(new CombatCharacterStatus(CombatCharacterStatusName.FROZEN, frozen_turn));
        });
        seq.Append(attacking.transform.DOMove(oriPos, 0.3f));
        return 0.8f;
    }
}