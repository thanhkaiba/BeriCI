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
    public override bool CanActive(Sailor cChar, CombatState cbState)
    {
        Sailor target = GetFurthestFuryMax(cbState.GetAllTeamAliveCharacter(cChar.cs.team));
        return (target != null && (target.cs.max_fury - target.cs.current_fury > 0));
    }
    public override float CastSkill(Sailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        cChar.cs.current_speed += (int)System.Math.Floor(speed_self_buff * cChar.cs.max_speed);

        Sailor target = GetFurthestFuryMax(cbState.GetAllTeamAliveCharacter(cChar.cs.team));

        return RunAnimation(cChar, target);
    }
    float RunAnimation(Sailor attacking, Sailor target)
    {
        attacking.TriggerAnimation("BaseAttack");
        GameEffMgr.Instance.ShowBuffEnergy(attacking.transform.position, target.transform.position);
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOLocalMoveY(1f, 0.5f));
        seq.AppendInterval(0.4f);
        seq.AppendCallback(() =>
        {
            target.GainFury(fury);
            target.cs.current_magic_resist += buff_magic_resist;
            target.cs.current_armor += buff_armor;
            FlyTextMgr.Instance.CreateFlyTextWith3DPosition("+Buff+", target.transform.position);
        });
        seq.AppendInterval(0.2f);
        seq.Append(attacking.transform.DOLocalMoveY(0f, 0.3f));
        return 1.0f;
    }
}