using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Encourage : Skill
{
    public int base_heal = 100;
    public int heal_per_level = 20;
    public int base_speed = 55;
    public int speed_per_level = 1;
    public int fury = 10;
    public Encourage()
    {
        name = "Encourage";
        MAX_FURY = 20;
        START_FURY = 0;
    }
    public override bool CanActive(CombatCharacter cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        int heal = base_heal + cChar.level * heal_per_level;
        int speed = base_speed + cChar.level * speed_per_level;
        int buff_fury = fury;

        List<CombatCharacter> allies = cbState.GetAllTeamAliveCharacter(cChar.team);
        CombatCharacter target = GetLowestPercentHealthTarget(allies);

        return RunAnimation(cChar, target, heal, speed, buff_fury);
    }
    float RunAnimation(CombatCharacter attacking, CombatCharacter target, int heal, int speed, int fury)
    {
        attacking.display.TriggerAnimation("BaseAttack");
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOLocalMoveY(1f, 0.5f));
        seq.AppendCallback(() =>
        {
            target.GainHealth(heal);
            target.AddSpeed(speed);
            target.GainFury(fury);
            FlyTextMgr.Instance.CreateFlyTextWith3DPosition("+Buff+", target.transform.position);
        });
        seq.AppendInterval(0.2f);
        seq.Append(attacking.transform.DOLocalMoveY(0f, 0.3f));
        return 0.8f;
    }
}