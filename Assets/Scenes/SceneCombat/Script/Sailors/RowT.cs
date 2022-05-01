using DG.Tweening;
using Piratera.Config;
using Piratera.Sound;
using System.Collections.Generic;
using UnityEngine;

public class RowT : CombatSailor
{
    private Spine.Bone boneTarget;
    private Spine.Bone boneArr;
    public RowT()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        return 0f;
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
        List<string> targets = new List<string>();
        List<float> _params = new List<float>();

        SynergiesConfig config = GlobalConfigs.Synergies;
        var state = CombatState.Instance;

        float magic_damage = cs.Power * Model.config_stats.skill_params[0];

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnemy(cs.team);
        CombatSailor target = TargetsUtils.Random(enermy);

        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { magic = magic_damage }, this));

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        Debug.Log("targets.Count: " + targets.Count);
        Debug.Log("_params.Count: " + _params.Count);
        CombatSailor target = CombatState.Instance.GetSailor(targets[0]);
        float loseHealth = _params[0];

        CombatState.Instance.HighlightListSailor(new List<CombatSailor> { this, target }, 2.1f);
        TriggerAnimation("Skill");

        Sequence seq = DOTween.Sequence();
        SoundMgr.PlaySoundSkillSailor(8);
        seq.AppendInterval(1.2f);
        seq.AppendCallback(() =>
        {
            Vector3 pos = target.transform.position;
            pos.y += 5.2f;
            pos.z -= 0.1f;
            var eff = Instantiate(Resources.Load<GameObject>("Effect2D/lazer/Lazer_blue"), pos, Quaternion.identity);
            eff.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        });
        seq.AppendInterval(0.45f);
        seq.AppendCallback(() => target.LoseHealth(new Damage() { magic = loseHealth }));
        return 2.0f;
    }
}
