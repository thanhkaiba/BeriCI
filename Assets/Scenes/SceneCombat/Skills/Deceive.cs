using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Deceive : Skill
{
    public int base_damage = 0;
    public int damage_per_level = 5;
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
        int deal_damage = cChar.current_power + base_damage + cChar.level * damage_per_level;

        cChar.position = GetRandomAvaiablePosition(cbState.GetAllTeamAliveCharacter(cChar.team));

        List<CombatCharacter> enermy = cbState.GetAliveCharacterEnermy(cChar.team);
        CombatCharacter target = GetFurthestTarget(cChar, enermy);

        return RunAnimation(cChar, target, deal_damage);
    }
    float RunAnimation(CombatCharacter attacking, CombatCharacter target, int damage)
    {

        Vector3 newPos = GameObject.Find("slot_" + (attacking.team == Team.A ? "A" : "B") + attacking.position.x + attacking.position.y).transform.position;
        float d = Vector3.Distance(newPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(newPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.Append(attacking.transform.DOMove(newPos, 0.5f));
        seq.Append(attacking.transform.DOMove(desPos, 0.3f));
        seq.AppendCallback(() => attacking.display.TriggerAnimation("BaseAttack"));
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => { target.TakeDamage(damage); });
        seq.Append(attacking.transform.DOMove(newPos, 0.3f));
        return 1.5f;
    }
}