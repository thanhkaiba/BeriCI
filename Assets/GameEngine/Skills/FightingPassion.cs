using DG.Tweening;
public class FightingPassion : Skill
{
    float increaseDamageRatio = 0.18f;
    float increaseSpeedRatio = 0.4f;
    float armor_increase = 12;
    float magic_resist_increase = 12;
    float increase_health = 30;
    float increase_health_per_level = 3.0f;
    public FightingPassion()
    {
        name = "Fighting Passion";
        MAX_FURY = 10;
        START_FURY = 0;
    }
    public override bool CanActive(Sailor cChar, CombatState cbState)
    {
        return true;
    }
    public override float CastSkill(Sailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float incPower = cChar.cs.base_power * increaseDamageRatio;
        cChar.cs.current_power += cChar.cs.base_power * increaseDamageRatio;
        cChar.cs.current_speed += (int)System.Math.Floor(cChar.cs.max_speed * increaseSpeedRatio);
        cChar.cs.current_armor += armor_increase;
        cChar.cs.current_magic_resist += magic_resist_increase;
        cChar.cs.max_health += increase_health + increase_health_per_level * cChar.level;
        cChar.GainHealth(increase_health + increase_health_per_level * cChar.level);
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition("+" + (int)incPower + "POW", cChar.transform.position);
        return RunAnim(cChar);
    }
    float RunAnim(Sailor cChar)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(cChar.transform.DOLocalMoveY(1f, 0.5f));
        seq.Append(cChar.transform.DOLocalMoveY(0f, 0.3f));
        return 0.8f;
    }
}