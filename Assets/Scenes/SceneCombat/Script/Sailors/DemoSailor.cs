using DG.Tweening;
using UnityEngine;

public class DemoSailor : CombatSailor
{
    public DemoSailor()
    {

    }
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
    }
    public override float RunBaseAttack(CombatSailor target)
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
