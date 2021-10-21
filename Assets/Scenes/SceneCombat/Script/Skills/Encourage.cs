using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Encourage : Skill
{
    public float base_heal = 40;
    public float heal_per_level = 10;

    public float speed_percent = 45;

    public int fury = 8;

    public float base_power_buff = 10;
    public float power_buff_per_level = 1;
    public Encourage()
    {
        name = "Encourage";
        MAX_FURY = 20;
        START_FURY = 0;
        rank = SkillRank.S;
    }
    public override bool CanActive(CombatSailor cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatSailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float heal = base_heal + cChar.Model.level * heal_per_level;
        int buff_fury = fury;
        float power_buff = base_power_buff + power_buff_per_level * cChar.Model.level;

        List<CombatSailor> allies = cbState.GetAllTeamAliveCharacter(cChar.cs.team);
        CombatSailor target = GetLowestPercentHealthTarget(allies);

        return RunAnimation(cChar, target, heal, target != cChar ? speed_percent : 0f, buff_fury, power_buff);
    }
    float RunAnimation(CombatSailor attacking, CombatSailor target, float heal, float speed, int fury, float power_buff)
    {
        attacking.TriggerAnimation("BaseAttack");
        GameEffMgr.Instance.ShowBuffEnergy(attacking.transform.position, target.transform.position);
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOLocalMoveY(1f, 0.5f));
        seq.AppendInterval(0.4f);
        seq.AppendCallback(() =>
        {
            target.GainHealth(heal);
            target.AddSpeed((int) speed * target.cs.MaxSpeed);
            target.GainFury(fury);
            target.cs.BasePower += power_buff;
            FlyTextMgr.Instance.CreateFlyTextWith3DPosition("+Buff+", target.transform.position);
        });
        seq.AppendInterval(0.2f);
        seq.Append(attacking.transform.DOLocalMoveY(0f, 0.3f));
        return 1.0f;
    }
}