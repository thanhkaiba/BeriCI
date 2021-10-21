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
    public override bool CanActive(CombatSailor cChar, CombatState cbState)
    {
        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cChar.cs.team);
        List<CombatSailor> targets = GetSameLineTarget(cChar.cs.position.y, enermy);
        return targets.Count > 0;
    }
    public override float CastSkill(CombatSailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float spell_damage = base_damage + cChar.Model.level * damage_per_level;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cChar.cs.team);
        List<CombatSailor> targets = GetSameLineTarget(cChar.cs.position.y, enermy);

        targets.ForEach(delegate (CombatSailor character)
        {
            character.TakeDamage(0, spell_damage, 0);
        });

        return 0.8f;
    }
}