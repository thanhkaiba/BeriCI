using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class FreezeWorld : Skill
{
    float decrease_armor = 10f;
    float damage = 10f;
    float damage_p_lv = 2f;
    public FreezeWorld()
    {
        name = "Freeze World";
        MAX_FURY = 16;
        START_FURY = 0;
    }
    public override bool CanActive(CombatSailor cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatSailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);

        List<CombatSailor> targets = cbState.GetAliveCharacterEnermy(cChar.cs.team);

        return RunAnimation(cChar, targets);
    }
    float RunAnimation(CombatSailor attacking, List<CombatSailor> targets)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => {
            targets.ForEach(target =>
            {
                target.TakeDamage(0, damage + attacking.Model.level * damage_p_lv, 0);
                target.AddStatus(new SailorStatus(SailorStatusType.FROZEN, 1));
                target.cs.BaseArmor -= decrease_armor;
            });
        });
        GameEffMgr.Instance.ShowSkillIconActive(attacking.transform.position, name);
        return 0.8f;
    }
}