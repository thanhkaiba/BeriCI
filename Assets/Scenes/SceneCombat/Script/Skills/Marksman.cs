using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Marksman : Skill
{
    public float damage_ratio = 1.0f;
    public Marksman()
    {
        skill_name = "Marksman";
        MAX_FURY = 10;
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
        List<CombatSailor> targets = TargetsUtils.Random(enermy, 2);



        return RunAnimation(cChar, targets);
    }
    float RunAnimation(CombatSailor attacking, List<CombatSailor> targets)
    {
        float delay = attacking.RunSkill(targets);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.AppendCallback(() => {
            targets.ForEach(target =>
            {
                target.TakeDamage(attacking.cs.Power * damage_ratio, 0, 0);
            });
        });
        return 0.8f;
    }
}