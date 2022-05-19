using DG.Tweening;
using Piratera.Config;
using Piratera.Sound;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Macay : CombatSailor
{
    public Macay()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        TriggerAnimation("Attack");

        Vector3 targetPos = target.transform.position;

        Sequence sq = DOTween.Sequence();
        sq.AppendInterval(1.0f);
        sq.AppendCallback(() =>
        {
            SoundMgr.PlaySound("Audio/Sailor/shoot");
            Root(targetPos);
        });
        return 1.1f;
    }
    public override float TakeDamage(Damage d)
    {
        TriggerAnimation("Hurt");
        return base.TakeDamage(d);
    }
    // skill
    public override bool CanActiveSkill(CombatState cbState)
    {
        return base.CanActiveSkill(cbState);
    }
    public override float CastSkill(CombatState cbState)
    {
        base.CastSkill(cbState);
        List<string> targets = new List<string>();
        List<float> _params = new List<float>();

        float magic_damage = cs.Power * Model.config_stats.skill_params[0];

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnemy(cs.team);
        List<CombatSailor> listTargets = TargetsUtils.AllLastRow(enermy);

        listTargets.ForEach(t =>
        {
            targets.Add(t.Model.id);
            _params.Add(t.CalcDamageTake(new Damage() { magic = magic_damage }, this));
        });

        SynergiesConfig config = GlobalConfigs.Synergies;
        ClassBonusItem wild = CombatState.Instance.GetTeamClassBonus(cs.team, SailorClass.WILD);
        _params.Add(CalcHealFromWild());

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        var listTargets = CombatState.Instance.GetSailors(targets);
        GameEffMgr.Instance.Shake(0.6f, 2);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1f);
        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundSkillSailor(7);
            for (int i = 0; i < listTargets.Count; i++)
            {
                Vector3 endPos = listTargets[i].transform.position;
                Root(endPos, 1);
            }
            GainHealth(_params[listTargets.Count]);
        });
        seq.AppendInterval(.2f);
        seq.AppendCallback(() =>
        {
            for (int i = 0; i < listTargets.Count; i++)
            {
                var target = listTargets[i];
                target.AddStatus(new SailorStatus(SailorStatusType.STUN, 1));
                target.LoseHealth(new Damage() { magic = _params[i] });
            }
        });

        return 3.5f;
    }
    private void Root(Vector3 pos, int type = 0)
    {
        SoundMgr.PlaySound("Audio/Sailor/ground_crack");
        pos.z -= 0.1f;
        var root = Instantiate(Resources.Load<GameObject>("Characters/Macay/root/MacayRoot"));
        root.transform.position = pos;
        root.transform.localScale = type == 0 ? new Vector3(1.8f, 1.0f) : new Vector3(3.4f, 1.2f);
        root.transform.Find("model").GetComponent<Animator>().SetTrigger(type == 0 ? "Attack" : "Skill");
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(2f);
        seq.AppendCallback(() => Destroy(root));
    }
}
