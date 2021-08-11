using DG.Tweening;
using System.Collections.Generic;

public class NuclearBomb : Skill
{
    public int base_damage = 450;
    public int damage_per_level = 45;
    public NuclearBomb()
    {
        name = "Nuclear Bomb";
        MAX_FURY = 80; // test, cho la 100
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

        List<CombatCharacter> targets = cbState.GetAliveCharacterEnermy(cChar.team);

        Sequence seq = DOTween.Sequence();
        seq.Append(cChar.transform.DOLocalMoveY(1f, 0.5f));
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => {
            targets.ForEach(delegate (CombatCharacter character)
            {
                character.TakeDamage(deal_damage);
            });
        });
        seq.AppendInterval(0.2f);
        seq.Append(cChar.transform.DOLocalMoveY(0f, 0.3f));

        return 1.4f;
    }
}