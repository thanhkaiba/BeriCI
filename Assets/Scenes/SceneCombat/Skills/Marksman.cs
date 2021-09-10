using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Marksman : Skill
{
    public float damage_ratio = 1.0f;
    public Marksman()
    {
        name = "Marksman";
        MAX_FURY = 10;
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
        List<CombatCharacter> targets = GetRandomTarget(enermy, 2);



        return RunAnimation(cChar, targets);
    }
    float RunAnimation(CombatCharacter attacking, List<CombatCharacter> targets)
    {
        float delay = 0;
        attacking.display.TriggerAnimation("BaseAttack");

        targets.ForEach(target =>
        {
            delay = attacking.display.ArrowToTarget(target);
        });

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.AppendCallback(() => {
            targets.ForEach(target =>
            {
                target.TakeDamage(attacking.current_power * damage_ratio, 0, 0);
            });
        });
        return 0.8f;
    }
}