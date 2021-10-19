using DG.Tweening;
public class TakeRest : Skill
{
    public float heal_ratio = 0.35f;
    public TakeRest()
    {
        name = "Take Rest";
        MAX_FURY = 32;
        START_FURY = 0;
    }
    public override bool CanActive(CombatSailor cChar, CombatState cbState)
    {
        return cChar.cs.CurHealth < cChar.cs.MaxHealth;
    }
    public override float CastSkill(CombatSailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        cChar.GainHealth(heal_ratio * cChar.cs.MaxHealth);
        return RunAnimTakeRest(cChar);
    }
    float RunAnimTakeRest(CombatSailor cChar)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(cChar.transform.DOLocalMoveY(1f, 0.5f));
        seq.Append(cChar.transform.DOLocalMoveY(0f, 0.3f));
        GameEffMgr.Instance.ShowSkillIconActive(cChar.transform.position, name);
        return 0.8f;
    }
}