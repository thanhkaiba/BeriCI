using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class HeadShot : Skill
{
    public float damage_ratio = 1.1f;
    public float lose_health_ratio = 0.15f;

    public float sniper_damage_ratio = 1.5f;
    public float sniper_lose_health_ratio = 0.35f;
    public HeadShot()
    {
        name = "Head Shot";
        MAX_FURY = 15;
        START_FURY = 5;
    }
    public override bool CanActive(CombatCharacter cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);

        List<CombatCharacter> enermy = cbState.GetAliveCharacterEnermy(cChar.team);
        CombatCharacter target = GetLowestHealthTarget(enermy);

        float physic_damage = cChar.type == CharacterType.SNIPER
            ? damage_ratio * cChar.current_power + lose_health_ratio * (target.max_health - target.current_health)
            : sniper_damage_ratio * cChar.current_power + sniper_lose_health_ratio * (target.max_health - target.current_health);

        return RunAnimation(cChar, target, physic_damage);
    }
    float RunAnimation(CombatCharacter attacking, CombatCharacter target, float physic_damage)
    {
        float delay = attacking.display.BaseAttack(target);

        Vector3 oriPos = attacking.transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.AppendCallback(() => { target.TakeDamage(physic_damage, 0, 0); });
        return 0.8f;
    }
}