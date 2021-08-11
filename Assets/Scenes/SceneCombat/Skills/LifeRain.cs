using DG.Tweening;
using System.Collections.Generic;

public class LifeRain : Skill
{
    public int base_heal = 50;
    public int heal_per_level = 40;
    public LifeRain()
    {
        name = "Life Rain";
        MAX_FURY = 40;
        START_FURY = 10;
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
        int heal = base_heal + cChar.level * heal_per_level;

        List<CombatCharacter> targets = cbState.GetAllTeamAliveCharacter(cChar.team);

        Sequence seq = DOTween.Sequence();
        seq.Append(cChar.transform.DOLocalMoveY(1f, 0.5f));
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => {
            targets.ForEach(delegate (CombatCharacter character)
            {
                character.GainHealth(heal);
            });
        });
        seq.AppendInterval(0.2f);
        seq.Append(cChar.transform.DOLocalMoveY(0f, 0.3f));

        return 1.4f;
    }
}