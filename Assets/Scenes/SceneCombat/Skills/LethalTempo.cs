using DG.Tweening;
public class LethalTempo : Skill
{
    public float speed_decrease = 0.1f;

    public LethalTempo()
    {
        name = "Lethal Tempo";
        MAX_FURY = 10;
        START_FURY = 0;
    }
    public override bool CanActive(CombatCharacter cChar, CombatState cbState)
    {
        return cChar.max_speed > 15;
    }
    public override float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        cChar.max_speed -= (int)(cChar.max_speed * speed_decrease);
        if (cChar.max_speed < 10) cChar.max_speed = 15;
        GameEffMgr.Instance.ShowSkillIconFall(cChar.transform.position, name);
        return 0.4f;
    }
}