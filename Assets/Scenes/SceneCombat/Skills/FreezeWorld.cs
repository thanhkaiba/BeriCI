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
    public override bool CanActive(CombatCharacter cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);

        List<CombatCharacter> targets = cbState.GetAliveCharacterEnermy(cChar.team);

        return RunAnimation(cChar, targets);
    }
    float RunAnimation(CombatCharacter attacking, List<CombatCharacter> targets)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => {
            targets.ForEach(target =>
            {
                target.TakeDamage(0, damage + attacking.level * damage_p_lv, 0);
                target.AddStatus(new CombatCharacterStatus(CombatCharacterStatusName.FROZEN, 1));
                target.current_armor -= decrease_armor;
            });
        });
        GameEffMgr.Instance.ShowSkillIconActive(attacking.transform.position, name);
        return 0.8f;
    }
}