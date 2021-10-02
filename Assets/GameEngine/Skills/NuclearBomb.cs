using DG.Tweening;
using System.Collections.Generic;

public class NuclearBomb : Skill
{
    public float base_damage = 350;
    public float damage_per_level = 35;
    public NuclearBomb()
    {
        name = "Nuclear Bomb";
        MAX_FURY = 100;
        START_FURY = 0;
    }
    public override bool CanActive(Sailor cChar, CombatState cbState)
    {
        List<Sailor> targets = cbState.GetAliveCharacterEnermy(cChar.cs.team);
        return targets.Count > 0;
    }
    public override float CastSkill(Sailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float spell_damage = base_damage + cChar.level * damage_per_level;

        List<Sailor> targets = cbState.GetAliveCharacterEnermy(cChar.cs.team);
        GameEffMgr.Instance.ShowFireBallFly(cChar.transform.position, GetOppositeTeam(cChar.cs.team));
        Sequence seq = DOTween.Sequence();
        seq.Append(cChar.transform.DOLocalMoveY(1f, 0.5f));
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => GameEffMgr.Instance.ShowExplosion(GetOppositeTeam(cChar.cs.team)));
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() => {
            targets.ForEach(delegate (Sailor character)
            {
                character.TakeDamage(0, spell_damage, 0);
            });
        });
        seq.AppendInterval(0.2f);
        seq.Append(cChar.transform.DOLocalMoveY(0f, 0.3f));

        return 1.4f;
    }
}