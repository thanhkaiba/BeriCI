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
        skill_name = "High Note";
        MAX_FURY = 0;
        START_FURY = 0;
        rank = SkillRank.A;
    }
    public override bool CanActive(CombatSailor cChar, CombatState cbState)
    {
        CombatSailor target = TargetsUtils.FurthestMaxFury(cbState.GetAllTeamAliveCharacter(cChar.cs.team));
        return (target != null && (target.cs.MaxFury - target.cs.Fury > 0));
    }
    public override float CastSkill(CombatSailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        cChar.cs.CurrentSpeed += (int)System.Math.Floor(speed_self_buff * cChar.cs.MaxSpeed);

        CombatSailor target = TargetsUtils.FurthestMaxFury(cbState.GetAllTeamAliveCharacter(cChar.cs.team));

        return RunAnimation(cChar, target);
    }
    float RunAnimation(CombatSailor attacking, CombatSailor target)
    {
        attacking.TriggerAnimation("BaseAttack");
        GameEffMgr.Instance.ShowBuffEnergy(attacking.transform.position, target.transform.position);
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOLocalMoveY(1f, 0.5f));
        seq.AppendInterval(0.4f);
        seq.AppendCallback(() =>
        {
            target.GainFury(fury);
            target.cs.BaseMagicResist += buff_magic_resist;
            target.cs.BaseArmor += buff_armor;
            FlyTextMgr.Instance.CreateFlyTextWith3DPosition("+Buff+", target.transform.position);
        });
        seq.AppendInterval(0.2f);
        seq.Append(attacking.transform.DOLocalMoveY(0f, 0.3f));
        return 1.0f;
    }
}