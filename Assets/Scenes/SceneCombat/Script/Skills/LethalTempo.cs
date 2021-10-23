using DG.Tweening;
public class LethalTempo : Skill
{
    public float speed_buff = 15f;

    public LethalTempo()
    {
        skill_name = "Lethal Tempo";
        MAX_FURY = 10;
        START_FURY = 0;
    }
    public override bool CanActive(CombatSailor cChar, CombatState cbState)
    {
        return cChar.cs.MaxSpeed > 15;
    }
    public override float CastSkill(CombatSailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        cChar.IncDisplaySpeed(speed_buff);
        GameEffMgr.Instance.ShowSkillIconFall(cChar.transform.position, skill_name);
        return 0.4f;
    }
}