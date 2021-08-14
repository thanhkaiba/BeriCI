using DG.Tweening;
using System.Collections.Generic;

public class LaserGun : Skill
{
    public float base_damage = 180;
    public float damage_per_level = 26;
    public LaserGun()
    {
        name = "Laser Gun";
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
        float spell_damage = base_damage + cChar.level * damage_per_level;

        List<CombatCharacter> enermy = cbState.GetAliveCharacterEnermy(cChar.team);
        List<CombatCharacter> targets = GetSameLineTarget(cChar.position.y, enermy);

        targets.ForEach(delegate (CombatCharacter character)
        {
            character.TakeDamage(0, spell_damage, 0);
        });

        return 0.8f;
    }
}