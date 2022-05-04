using DG.Tweening;
using Piratera.Sound;
using Spine;
using System.Collections.Generic;
using UnityEngine;

public class BeiBei : CombatSailor
{
    public BeiBei()
    {
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    public override void GainFury(int value)
    {
        base.GainFury(value);
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        TriggerAnimation("Attack");
        Vector3 oriPos = transform.position;
        Debug.Log("target: " + target);
        Debug.Log("target id: " + target.Model.id);
        Debug.Log("target id: " + target.Model.config_stats.root_name);
        int offset = transform.position.x < target.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            target.transform.position.x + offset * 5,
            target.transform.position.y,
            target.transform.position.z - 0.1f
        );
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(.7f);
        seq.Append(transform.DOMove(desPos, 0.7f).SetEase(Ease.InOutSine));
        seq.AppendInterval(.1f);
        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySoundAttackSailor(0);
        });
        seq.AppendInterval(0.4f);
        seq.Append(transform.DOMove(oriPos, 0.1f).SetEase(Ease.OutSine));
        return 1.35f;
    }
    public override float TakeDamage(Damage d)
    {
        TriggerAnimation("Hurt");
        return base.TakeDamage(d);
    }
    // skill
    public override bool CanActiveSkill(CombatState cbState)
    {
        var percentHealth = cs.CurHealth / cs.MaxHealth;
        var listAlies = cbState.GetAllTeamAliveSailors(cs.team);
        return percentHealth <= Model.config_stats.skill_params[0] && listAlies.Count >= 2;
    }
    public override float CastSkill(CombatState cbState)
    {
        List<string> targets = new List<string>();
        List<float> _params = new List<float>();

        float damage = Model.config_stats.skill_params[1] * cs.MaxHealth;
        float sub_damage = Model.config_stats.skill_params[2] * cs.MaxHealth;

        List<CombatSailor> enemies = cbState.GetAliveCharacterEnemy(cs.team);
        CombatSailor target = TargetsUtils.Melee(this, enemies);
        List<CombatSailor> around = TargetsUtils.Around(target, enemies, false);

        targets.Add(target.Model.id);
        _params.Add(target.CalcDamageTake(new Damage() { magic = damage }, this));

        around.ForEach(t =>
        {
            targets.Add(t.Model.id);
            _params.Add(t.CalcDamageTake(new Damage() { magic = sub_damage }, this));
        });

        return ProcessSkill(targets, _params);
    }
    public override float ProcessSkill(List<string> targets, List<float> _params)
    {
        base.ProcessSkill();
        TriggerAnimation("Skill");
        var listTargets = CombatState.Instance.GetSailors(targets);
        var mainTarget = CombatState.Instance.GetSailor(targets[0]);

        ShowRandomTalk();
        Vector3 oriPos = transform.position;

        int offset = transform.position.x < mainTarget.transform.position.x ? -1 : 1;
        Vector3 desPos = new Vector3(
            mainTarget.transform.position.x + offset * 4,
            mainTarget.transform.position.y,
            mainTarget.transform.position.z
        );
        desPos.z -= 0.1f;
        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(0.7f);
        seq.Append(transform.DOMove(desPos, 0.7f).SetEase(Ease.OutSine));
        seq.AppendInterval(1.6f);
        StartCoroutine(GameUtils.WaitAndDo(0.8f, () => SoundMgr.PlaySoundSkillSailor(10)));
        seq.AppendCallback(() =>
        {
            SoundMgr.PlaySound("Audio/Sailor/bomb");
            cs.CurHealth = 0;
            GameEffMgr.Instance.Shake(0.3f, 3.0f);
            for (int i = 0; i < targets.Count; i++) listTargets[i].LoseHealth(new Damage() { magic = _params[i] });
        });
        seq.AppendInterval(5f); 
        seq.AppendCallback(() => CheckDeath());
        seq.Append(transform.DOMove(oriPos, 0.15f).SetEase(Ease.OutSine));
        return 4.0f;
    }
    private void ShowRandomTalk()
    {
        var random = MathUtils.RandomInt(0, 9);
        string text = "";
        switch(random)
        {
            case 0:
                text = "I Love U, Meechik";
                break;
            case 1:
                text = "Goodbye my friend...";
                break;
            case 2:
                text = "sample only one time";
                break;
            case 3:
                text = "I Love U, LiuHi";
                break;
            case 4:
                text = "Oh, shining here";
                break;
            case 5:
                text = "You are my everything";
                break;
            case 6:
                text = "Don't touch my friend";
                break;
            case 7:
                text = "Remember me";
                break;
            case 8:
                text = "Love you all, kkk";
                break;
            case 9:
                text = "I'll be back";
                break;
        }
        GameEffMgr.Instance.ShowChat(this, text, 5);
    }
}
