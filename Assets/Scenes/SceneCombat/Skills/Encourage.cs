using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Encourage : Skill
{
    public float base_heal = 40;
    public float heal_per_level = 10;

    public int base_speed = 45;
    public int speed_per_level = 0;

    public int fury = 4;

    public float base_power_buff = 10;
    public float power_buff_per_level = 1;
    public Encourage()
    {
        name = "Encourage";
        MAX_FURY = 20;
        START_FURY = 0;
        rank = SkillRank.S;
    }
    public override bool CanActive(CombatCharacter cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float heal = base_heal + cChar.level * heal_per_level;
        int speed = base_speed + cChar.level * speed_per_level;
        int buff_fury = fury;
        float power_buff = base_power_buff + power_buff_per_level * cChar.level;

        List<CombatCharacter> allies = cbState.GetAllTeamAliveCharacter(cChar.team);
        CombatCharacter target = GetLowestPercentHealthTarget(allies);

        return RunAnimation(cChar, target, heal, target != cChar ? speed : 0, buff_fury, power_buff);
    }
    float RunAnimation(CombatCharacter attacking, CombatCharacter target, float heal, int speed, int fury, float power_buff)
    {
        attacking.display.TriggerAnimation("BaseAttack");
        GameEffMgr.Instance.ShowBuffEnergy(attacking.transform.position, target.transform.position);
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOLocalMoveY(1f, 0.5f));
        seq.AppendInterval(0.4f);
        seq.AppendCallback(() =>
        {
            target.GainHealth(heal);
            target.AddSpeed(speed);
            target.GainFury(fury);
            target.current_power += power_buff;
            FlyTextMgr.Instance.CreateFlyTextWith3DPosition("+Buff+", target.transform.position);
        });
        seq.AppendInterval(0.2f);
        seq.Append(attacking.transform.DOLocalMoveY(0f, 0.3f));
        return 1.0f;
    }
}