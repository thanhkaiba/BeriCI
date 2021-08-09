using DG.Tweening;
public class TakeRest : Skill
{
    public TakeRest()
    {
        name = "TakeRest";
        MAX_FURY = 10;
        START_FURY = 5;
    }
    public override float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        cChar.GainHealth((int)(0.4 * cChar.max_health));
        return RunAnimTakeRest(cChar);
    }
    float RunAnimTakeRest(CombatCharacter cChar)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(cChar.transform.DOLocalMoveY(1f, 0.5f));
        seq.Append(cChar.transform.DOLocalMoveY(0f, 0.3f));
        return 0.8f;
    }
}