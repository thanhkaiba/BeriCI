using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Deceive : Skill
{
    public float base_damage = 10;
    public float damage_per_level = 8;
    public Deceive()
    {
        name = "Deceive";
        MAX_FURY = 10;
        START_FURY = 0;
    }
    public override bool CanActive(CombatCharacter cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float physic_damage = cChar.current_power;
        float magic_damage = base_damage + cChar.level * damage_per_level;

        List<CombatCharacter> enermy = cbState.GetAliveCharacterEnermy(cChar.team);
        CombatCharacter target = GetFurthestTarget(cChar, enermy);

        cChar.position = GetRandomAvaiablePosition(cbState.GetAllTeamAliveCharacter(cChar.team));

        return RunAnimation(cChar, target, physic_damage, magic_damage);
    }
    float RunAnimation(CombatCharacter attacking, CombatCharacter target, float physic_damage, float magic_damage)
    {

        Vector3 newPos = GameObject.Find("slot_" + (attacking.team == Team.A ? "A" : "B") + attacking.position.x + attacking.position.y).transform.position;
        float d = Vector3.Distance(newPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(newPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOMove(desPos, 0.3f));
        seq.AppendCallback(() => attacking.display.TriggerAnimation("BaseAttack"));
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => { target.TakeDamage(physic_damage, magic_damage, 0); });
        seq.Append(attacking.transform.DOMove(newPos, 0.3f));
        return 1.0f;
    }
}