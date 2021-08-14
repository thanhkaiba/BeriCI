using DG.Tweening;
public class FightingPassion : Skill
{
    float increaseDamageRatio = 0.25f;
    float increaseSpeedRatio = 0.8f;
    float armor_increase = 20;
    float magic_resist_increase = 20;
    public FightingPassion()
    {
        name = "Fighting Passion";
        MAX_FURY = 10;
        START_FURY = 0;
    }
    public override bool CanActive(CombatCharacter cChar, CombatState cbState)
    {
        return true;
    }
    public override float CastSkill(CombatCharacter cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float incPower = cChar.base_power * increaseDamageRatio;
        cChar.current_power += cChar.base_power * increaseDamageRatio;
        cChar.current_speed += (int)System.Math.Floor(cChar.max_speed * increaseSpeedRatio);
        cChar.current_armor += armor_increase;
        cChar.current_magic_resist += magic_resist_increase;
        FlyTextMgr.Instance.CreateFlyTextWith3DPosition("+" + (int)incPower + "POW", cChar.transform.position);
        return RunAnim(cChar);
    }
    float RunAnim(CombatCharacter cChar)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(cChar.transform.DOLocalMoveY(1f, 0.5f));
        seq.Append(cChar.transform.DOLocalMoveY(0f, 0.3f));
        return 0.8f;
    }
}