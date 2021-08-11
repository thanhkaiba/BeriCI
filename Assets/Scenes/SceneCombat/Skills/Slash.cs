using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Slash : Skill
{
    public int base_damage = 50;
    public int damage_per_level = 5;
    public Slash()
    {
        name = "Slash";
        MAX_FURY = 3;
        START_FURY = 0;
    }
    public override bool CanActive(CombatCharacter cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        int deal_damage = cChar.current_power + base_damage + cChar.level * damage_per_level;

        List<CombatCharacter> enermy = cbState.GetAliveCharacterEnermy(cChar.team);
        CombatCharacter target = GetNearestTarget(cChar, enermy);

        return RunAnimation(cChar, target, deal_damage);
    }
    float RunAnimation(CombatCharacter attacking, CombatCharacter target, int damage)
    {
        attacking.display.TriggerAnimation("BaseAttack");

        Vector3 oriPos = attacking.transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOMove(desPos, 0.3f));
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => { target.TakeDamage(damage); });
        seq.Append(attacking.transform.DOMove(oriPos, 0.3f));
        return 0.8f;
    }
}