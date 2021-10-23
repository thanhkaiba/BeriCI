using DG.Tweening;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Meechic : CombatSailor
{
    public SailorConfig config;
    private GameObject circle;
    public Meechic()
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
        GameEffMgr.Instance.BulletToTarget(transform.FindDeepChild("nodeStartBullet").position, target.transform.position, 0.4f, 0.2f);
        return 0.6f;
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

public class MeechicBoommm : Skill
{
    public float scale_damage_ratio;
    public float around_damage_ratio;
    public override void UpdateData(SkillConfig config)
    {
        base.UpdateData(config);
        scale_damage_ratio = config._params[0];
        around_damage_ratio = config._params[1];
    }
    public override bool CanActive(CombatSailor cChar, CombatState cbState)
    {
        return base.CanActive(cChar, cbState);
    }
    public override float CastSkill(CombatSailor cChar, CombatState cbState)
    {
        base.CastSkill(cChar, cbState);
        float physic_damage = cChar.cs.Power;

        List<CombatSailor> enermy = cbState.GetAliveCharacterEnermy(cChar.cs.team);
        CombatSailor target = GetRangeAttackTarget(cChar, enermy);
        List<CombatSailor> around_target = GetAllAround(target, enermy);

        return RunAnimation(cChar, target, around_target, physic_damage);
    }
    float RunAnimation(CombatSailor attacking, CombatSailor target, List<CombatSailor> around_target, float physics_damage)
    {
        attacking.TriggerAnimation("BaseAttack");
        GameEffMgr.Instance.BulletToTarget(attacking.transform.FindDeepChild("nodeStartBullet").position, target.transform.position, 0.4f, 0.2f);
        CombatEvents.Instance.highlightTarget.Invoke(target);
        Vector3 oriPos = attacking.transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 8.0f);
        desPos.y += 1;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.6f);
        seq.AppendCallback(() => GameEffMgr.Instance.ShowExplosion(target.transform.position) );
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() =>
        {
            target.TakeDamage(physics_damage * scale_damage_ratio, 0, 0);
            around_target.ForEach(s => s.TakeDamage(physics_damage * around_damage_ratio, 0, 0, 2));
        });
        
        seq.AppendInterval(0.3f);
        return 1.1f;
    }
}