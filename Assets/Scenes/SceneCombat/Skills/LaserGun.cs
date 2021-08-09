using DG.Tweening;
using System.Collections.Generic;

public class LaserGun : Skill
{
    public int base_damage = 100;
    public int damage_per_level = 5;
    public LaserGun()
    {
        name = "LaserGun";
        MAX_FURY = 0;
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
        int deal_damage = base_damage + cChar.level * damage_per_level;

        List<CombatCharacter> enermy = cbState.GetAliveCharacterEnermy(cChar.team);
        List<CombatCharacter> targets = GetSameLineTarget(cChar.position.y, enermy);

        targets.ForEach(delegate (CombatCharacter character)
        {
            character.TakeDamage(deal_damage);
        });

        return 0.8f;
    }
}