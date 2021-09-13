using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DemoSailorAnimator : CharacterAnimatorCtrl
{
    public override float BaseAttack(Sailor target)
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
    public override void SetFaceDirection(int scaleX)
    {
        float scale = modelObject.transform.localScale.x;
        modelObject.transform.localScale = new Vector3(scale * scaleX, scale, scale);
    }
}