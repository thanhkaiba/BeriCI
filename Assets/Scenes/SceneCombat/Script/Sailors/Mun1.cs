using DG.Tweening;
using Piratera.Config;
using Piratera.Sound;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Mun : CombatSailor
{
    public Mun()
    {
    }
    public override void Awake()
    {
        base.Awake();
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        TriggerAnimation("Attack");
        Vector3 oriPos = transform.position;
        int offset = transform.position.x < target.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 4,
            target.transform.position.y,
            target.transform.position.z - 0.1f
        );
        Sequence sq = DOTween.Sequence();
        sq.AppendInterval(0.5f);
        sq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(8);
        });
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.1f);
        seq.Append(transform.DOMove(desPos, 0.2f).SetEase(Ease.OutSine));
        seq.AppendInterval(0.9f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return 0.9f;
    }
    // skill
    public override bool CanActiveSkill(CombatState cbState)
    {
        return false;
    }
    public override void ActiveStartPassive()
    {
        cs.Dodge = Model.config_stats.skill_params[0] + Model.config_stats.skill_params[1] * Model.star;
        base.ActiveStartPassive();
    }
    private CombatSailor MUN_enemy = null;
    public override float RunDodge(CombatSailor attacker, float dodgeDamage)
    {
        TriggerAnimation("Skill");
        AddCurSpeed(CalSpeedAdd());
        MUN_enemy = attacker;
        return 0.3f;
    }
    int CalSpeedAdd()
    {
        var combatState = CombatState.Instance;
        int speedAllNeed = 99999999;
        combatState.GetAllAliveCombatSailors().ForEach(character =>
        {
            int speedNeed = character.GetSpeedNeeded();
            speedAllNeed = Math.Min(speedAllNeed, speedNeed);
        });

        var speedMUNNeed = GetSpeedNeeded();
        if (speedAllNeed <= 0) return speedMUNNeed - speedAllNeed + 1;
        else return speedMUNNeed;
    }
    protected override CombatSailor GetBaseAttackTarget(CombatState combatState)
    {
        if (MUN_enemy != null)
        {
            var cache = MUN_enemy;
            MUN_enemy = null;
            return cache;
        }
        return base.GetBaseAttackTarget(combatState);
    }
}