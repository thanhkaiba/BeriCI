using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using DG.Tweening;

public class DemoSailor2 : Sailor
{
    public DemoSailor2()
    {
        skill = new IceArrow();
    }

    // animations
    public override void Awake()
    {
        base.Awake();
        modelObject = transform.Find("model").gameObject;
        startArrow = transform.FindDeepChild("goblin_L_hand");
    }
    public Transform startArrow;
    public override float RunBaseAttack(Sailor target)
    {
        TriggerAnimation("BaseAttack");
        return ArrowToTarget(target);
    }

    public float ArrowToTarget(Sailor target)
    {
        if (target == null) return 0f;
        var arrow = Instantiate(
                Resources.Load<GameObject>("GameComponents/arrow"),
                startArrow.position,
                Quaternion.identity);
        arrow.SetActive(false);
        var targetHeart = new Vector3(target.transform.position.x,
            target.transform.position.y + 2.0f,
            target.transform.position.z
        );
        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, targetHeart);
        Vector3 desPos = Vector3.MoveTowards(oriPos, targetHeart, d - 1.4f);
        int r = 1;
        if (arrow.transform.position.x < desPos.x)
        {
            arrow.transform.localScale = new Vector3(-1, 1, 1);
            r = -1;
        }

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.05f);
        seq.AppendCallback(() => {
            arrow.SetActive(true);
            arrow.transform.rotation = Quaternion.Euler(0, 0, -5 * r);
            arrow.transform.DORotate(new Vector3(0, 0, 10 * r), 0.4f);
        });
        seq.Append(arrow.transform.DOJump(desPos, 1, 1, 0.4f));
        seq.AppendCallback(() => Destroy(arrow));

        return 0.4f;
    }
    public override float RunSkill (List<Sailor> targets)
    {
        float delay = 0f;
        if (skill is Marksman)
        {
            TriggerAnimation("BaseAttack");
            targets.ForEach(target =>
            {
                delay = ArrowToTarget(target);
            });
        }
        return delay;
    }
    public override void SetFaceDirection()
    {
        int scaleX = cs.team == Team.A ? -1 : 1;
        float scale = modelObject.transform.localScale.x;
        modelObject.transform.localScale = new Vector3(-scale * scaleX, scale, scale);
    }
}
