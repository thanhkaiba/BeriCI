using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;

public class Helti : Sailor
{
    public Helti()
    {
        //skill = new Slash();
        config_url = "Assets/Config/Sailors/Helti.json";
    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    public override float RunBaseAttack(Sailor target)
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
        return 0.5f;
    }
    public override void SetFaceDirection()
    {
        modelObject.transform.localScale = new Vector3(cs.team == Team.A ? 1 : -1, 1, 1);
    }
    private void LateUpdate()
    {
        SetFaceDirection();
    }
}
