using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Deceive : Skill
{
    public float base_damage = 10;
    public float damage_per_level = 2.5f;
    public Deceive()
    {
        name = "Deceive";
        MAX_FURY = 20;
        START_FURY = 0;
    }
    public override bool CanActive(CombatSailor cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatSailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float physic_damage = cChar.cs.Power;
        float magic_damage = base_damage + cChar.Model.level * damage_per_level;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cChar.cs.team);
        CombatSailor target = GetFurthestTarget(cChar, enermy);

        cChar.cs.position = GetRandomAvaiablePosition(cbState.GetAllTeamAliveCharacter(cChar.cs.team));

        return RunAnimation(cChar, target, physic_damage, magic_damage);
    }
    float RunAnimation(CombatSailor attacking, CombatSailor target, float physics_damage, float magic_damage)
    {

        Vector3 newPos = GameObject.Find("slot_" + (attacking.cs.team == Team.A ? "A" : "B") + attacking.cs.position.x + attacking.cs.position.y).transform.position;
        float d = Vector3.Distance(newPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(newPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOMove(desPos, 0.3f));
        seq.AppendCallback(() => attacking.TriggerAnimation("BaseAttack"));
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => target.TakeDamage(new Damage() {
            physics_damage = physics_damage,
            magic_damage = magic_damage,
        }));
        seq.Append(attacking.transform.DOMove(newPos, 0.3f));
        return 1.0f;
    }
}