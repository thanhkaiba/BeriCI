using DG.Tweening;
using System.Collections.Generic;

public class LifeRain : Skill
{
    public float base_heal = 50;
    public float heal_per_level = 20;
    public int gain_fury = 8;
    public LifeRain()
    {
        name = "Life Rain";
        MAX_FURY = 25;
        START_FURY = 15;
    }
    public override bool CanActive(CombatCharacter cChar, CombatState cbState)
    {
        return true;
    }
    public override float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float heal = base_heal + cChar.level * heal_per_level;

        List<CombatCharacter> targets = cbState.GetAllTeamAliveCharacter(cChar.team);

        Sequence seq = DOTween.Sequence();
        seq.Append(cChar.transform.DOLocalMoveY(1f, 0.5f));
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => {
            targets.ForEach(delegate (CombatCharacter character)
            {
                character.GainHealth(heal);
                character.GainFury(gain_fury);
            });
        });
        seq.AppendInterval(0.2f);
        seq.Append(cChar.transform.DOLocalMoveY(0f, 0.3f));

        return 1.4f;
    }
}