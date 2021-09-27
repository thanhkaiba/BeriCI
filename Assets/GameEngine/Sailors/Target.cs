using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;

public class Target : Sailor
{
    public Target()
    {
        skill = new Slash();
        config_url = "Assets/Config/Sailors/Target.json";
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    public override float TakeDamage(float physicsDamage = 0, float magicDamage = 0, float trueDamage = 0)
    {
        TriggerAnimation("TakeDamage");
        Debug.Log("THE HELL");
        return base.TakeDamage(physicsDamage, magicDamage, trueDamage);
    }
    public override void AddSpeed(int speedAdd)
    {
        // do nothing
    }
    public override float RunBaseAttack(Sailor target)
    {
        TriggerAnimation("BaseAttack");
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(desPos, 0.3f));
        seq.AppendInterval(0.2f);
        seq.Append(transform.DOMove(oriPos, 0.2f));
        return 0.4f;
    }
}
