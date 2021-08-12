using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UltimateSlash : Skill
{
    public int base_damage = 120;
    public int damage_per_level = 12;
    public int base_heal = 10;
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
        int deal_damage = cChar.current_power * 2 + base_damage + cChar.level * damage_per_level;
        int heal = base_heal + cChar.current_power;

        List<CombatCharacter> enermy = cbState.GetAliveCharacterEnermy(cChar.team);
        CombatCharacter target = GetNearestTarget(cChar, enermy);

        return RunAnimation(cChar, target, deal_damage, heal);
    }
    float RunAnimation(CombatCharacter attacking, CombatCharacter target, int damage, int heal)
    {
        attacking.display.TriggerAnimation("BaseAttack");

        Vector3 oriPos = attacking.transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOMove(desPos, 0.6f));
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => {
            target.TakeDamage(damage);
            attacking.GainHealth(heal);
        });
        seq.Append(attacking.transform.DOMove(oriPos, 0.4f));
        return 1.2f;
    }
}