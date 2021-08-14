using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class HighNote : Skill
{
    public int fury = 8;
    public float speed_self_buff = 0.15f;
    public float buff_magic_resist = 10f;
    public float buff_armor = 10f;

    public HighNote()
    {
        name = "High Note";
        MAX_FURY = 0;
        START_FURY = 0;
        rank = SkillRank.A;
    }
    public override bool CanActive(CombatCharacter cChar, CombatState cbState)
    {
        CombatCharacter target = GetFurthestFuryMax(cbState.GetAllTeamAliveCharacter(cChar.team));
        return (target != null && (target.max_fury - target.current_fury > 0));
    }
    public override float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        cChar.current_speed += (int)System.Math.Floor(speed_self_buff * cChar.max_speed);

        CombatCharacter target = GetFurthestFuryMax(cbState.GetAllTeamAliveCharacter(cChar.team));

        return RunAnimation(cChar, target);
    }
    float RunAnimation(CombatCharacter attacking, CombatCharacter target)
    {
        attacking.display.TriggerAnimation("BaseAttack");
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOLocalMoveY(1f, 0.5f));
        seq.AppendCallback(() =>
        {
            target.GainFury(fury);
            target.current_magic_resist += buff_magic_resist;
            target.current_armor += buff_armor;
            FlyTextMgr.Instance.CreateFlyTextWith3DPosition("+Buff+", target.transform.position);
        });
        seq.AppendInterval(0.2f);
        seq.Append(attacking.transform.DOLocalMoveY(0f, 0.3f));
        return 0.8f;
    }
}