using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UltimateSlash : Skill
{
    public float base_heal = 40;
    public float scale_damage = 2.5f;
    public UltimateSlash()
    {
        name = "Ultimate Slash";
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
        float true_damage = cChar.current_power * scale_damage;
        float heal = base_heal + cChar.current_power;

        List<CombatCharacter> enermy = cbState.GetAliveCharacterEnermy(cChar.team);
        CombatCharacter target = GetNearestTarget(cChar, enermy);

        return RunAnimation(cChar, target, true_damage, heal);
    }
    float RunAnimation(CombatCharacter attacking, CombatCharacter target, float damage, float heal)
    {
        Vector3 oriPos = attacking.transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOMove(desPos, 0.4f));
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() => {
            attacking.display.TriggerAnimation("BaseAttack");
            target.TakeDamage(0, 0, damage);
            attacking.GainHealth(heal);
        });
        seq.Append(attacking.transform.DOMove(oriPos, 0.2f));
        return 0.7f;
    }
}