using DG.Tweening;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Helti : CombatSailor
{
    public SailorConfig config;
    private GameObject circle;
    public Helti()
    {
    }
    public override void Awake()
    {
        //SailorConfig a = Resources.Load<SailorConfig>("ScriptableObject/Sailors/Helti");
        //string json = a.Serialize(a);
        //Debug.Log("AAAA " + json);
        //File.WriteAllText("Assets/Helti.json", "");
        //StreamWriter writer = new StreamWriter("Assets/Helti.json", true);
        //writer.WriteLine(json);
        //writer.Close();

        circle = Instantiate(Resources.Load<GameObject>("GameComponents/SkillAvaiableCircle/circle"));
        circle.GetComponent<CircleSkillAvaiable>().SetCharacter(gameObject);
        circle.SetActive(false);

        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    public override void GainFury(int value)
    {
        base.GainFury(value);
        if (cs.Fury >= cs.MaxFury && !circle.activeSelf) circle.GetComponent<CircleSkillAvaiable>().Appear();
    }
    public override float UseSkill(CombatState combatState)
    {
        circle.GetComponent<CircleSkillAvaiable>().Disappear();
        return base.UseSkill(combatState);
    }
    public override float RunBaseAttack(CombatSailor target)
    {
        TriggerAnimation("BaseAttack");
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 4.0f);
        desPos.y += 1;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.15f);
        seq.Append(transform.DOJump(desPos, 1.2f, 1, 0.3f));
        seq.AppendInterval(0.2f);
        seq.Append(transform.DOJump(oriPos, 1, 1, 0.3f));
        return 0.4f;
    }
    public override void SetFaceDirection()
    {
        if (modelObject.activeSelf) modelObject.transform.localScale = new Vector3(cs.team == Team.A ? 1 : -1, 1, 1);
    }
    private void LateUpdate()
    {
        if (cs != null) SetFaceDirection();
    }
    public override float TakeDamage(Damage d)
    {
        TriggerAnimation("TakeDamage");
        return base.TakeDamage(d);
    }
}
public class WindSlash : Skill
{
    public float scale_damage_ratio;
    public float behind_damage_ratio;
    public override void UpdateData(SkillConfig config)
    {
        base.UpdateData(config);
        scale_damage_ratio = config._params[0];
        behind_damage_ratio = config._params[1];
    }
    public override bool CanActive(CombatSailor cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatSailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float physic_damage = cChar.cs.Power * scale_damage_ratio;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cChar.cs.team);
        CombatSailor target = GetNearestTarget(cChar, enermy);
        List<CombatSailor> behind_target = GetAllBehind(target, enermy);

        return RunAnimation(cChar, target, behind_target, physic_damage);
    }
    float RunAnimation(CombatSailor attacking, CombatSailor target, List<CombatSailor> behind_target, float physics_damage)
    {
        attacking.TriggerAnimation("CastSkill");
        CombatEvents.Instance.highlightTarget.Invoke(target);
        Vector3 oriPos = attacking.transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 8.0f);
        desPos.y += 1;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.15f);
        seq.Append(attacking.transform.DOJump(desPos, 1.2f, 1, 0.3f));

        seq.AppendCallback(() =>
        {
            target.TakeDamage(physics_damage, 0, 0);
            behind_target.ForEach(s => s.TakeDamage(physics_damage * behind_damage_ratio, 0, 0, 2));
        });
        seq.AppendInterval(0.35f);
        seq.AppendCallback(() =>
        {
            target.TakeDamage(physics_damage, 0, 0);
            behind_target.ForEach(s => s.TakeDamage(physics_damage * behind_damage_ratio, 0, 0, 2));
        });
        seq.AppendInterval(0.35f);
        seq.AppendCallback(() =>
        {
            target.TakeDamage(physics_damage, 0, 0);
            behind_target.ForEach(s => s.TakeDamage(physics_damage * behind_damage_ratio, 0, 0, 2));
        });

        seq.AppendInterval(0.3f);
        seq.Append(attacking.transform.DOJump(oriPos, 0.8f, 1, 0.4f));
        return 2.0f;
    }
}