using DG.Tweening;
using System.Collections.Generic;

public class LifeRain : Skill
{
    public float base_heal = 50;
    public float heal_per_level = 20;
    public int gain_fury = 8;
    public LifeRain()
    {
        skill_name = "Life Rain";
        MAX_FURY = 25;
        START_FURY = 15;
    }
    public override bool CanActive(CombatSailor cChar, CombatState cbState)
    {
        return true;
    }
    public override float CastSkill(CombatSailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float heal = base_heal + cChar.Model.level * heal_per_level;

        List<CombatSailor> targets = cbState.GetAllTeamAliveSailors(cChar.cs.team);

        Sequence seq = DOTween.Sequence();
        seq.Append(cChar.transform.DOLocalMoveY(1f, 0.5f));
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => {
            targets.ForEach(delegate (CombatSailor character)
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