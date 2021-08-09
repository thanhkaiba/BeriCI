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
        return 0.8f;
    }
}