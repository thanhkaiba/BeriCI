using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class HeadShot : Skill
{
    public float damage_ratio = 1.0f;
    public float lose_health_ratio = 0.1f;

    public float sniper_damage_ratio = 1.2f;
    public float sniper_lose_health_ratio = 0.25f;
    public HeadShot()
    {
        skill_name = "Head Shot";
        MAX_FURY = 15;
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
        CombatSailor target = GetLowestHealthTarget(enermy);

        float physic_damage = damage_ratio * cChar.cs.Power + lose_health_ratio * (target.cs.MaxHealth - target.cs.CurHealth);

        return RunAnimation(cChar, target, physic_damage);
    }
    float RunAnimation(CombatSailor attacking, CombatSailor target, float physic_damage)
    {
        float delay = attacking.RunBaseAttack(target);

        Vector3 oriPos = attacking.transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.AppendCallback(() => { target.TakeDamage(physic_damage, 0, 0); });
        return 0.8f;
    }
}