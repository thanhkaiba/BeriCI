using DG.Tweening;
using System.Collections.Generic;

public class LaserGun : Skill
{
    public float base_damage = 180;
    public float damage_per_level = 26;
    public LaserGun()
    {
        name = "LaserGun";
        MAX_FURY = 15;
        START_FURY = 0;
    }
    public override bool CanActive(CombatCharacter cChar, CombatState cbState)
    {
        List<CombatCharacter> enermy = cbState.GetAliveCharacterEnermy(cChar.team);
        List<CombatCharacter> targets = GetSameLineTarget(cChar.position.y, enermy);
        return targets.Count > 0;
    }
    public override float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float deal_damage = base_damage + cChar.level * damage_per_level;

        List<CombatCharacter> enermy = cbState.GetAliveCharacterEnermy(cChar.team);
        List<CombatCharacter> targets = GetSameLineTarget(cChar.position.y, enermy);

        targets.ForEach(delegate (CombatCharacter character)
        {
            character.TakeDamage(deal_damage);
        });

        return 0.8f;
    }
}