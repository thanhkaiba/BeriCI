using DG.Tweening;
public class LethalTempo : Skill
{
    public float speed_buff = 15f;

    public LethalTempo()
    {
        name = "Lethal Tempo";
        MAX_FURY = 10;
        START_FURY = 0;
    }
    public override bool CanActive(Sailor cChar, CombatState cbState)
    {
        return cChar.cs.max_speed > 15;
    }
    public override float CastSkill(Sailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        cChar.IncDisplaySpeed(speed_buff);
        GameEffMgr.Instance.ShowSkillIconFall(cChar.transform.position, name);
        return 0.4f;
    }
}