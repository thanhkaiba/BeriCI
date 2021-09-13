using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class CharacterAnimatorCtrl : MonoBehaviour
{
    public GameObject modelObject;

    public Transform startArrow;
    private GameObject iceBlock = null;
    public void TriggerAnimation(string trigger)
    {
        modelObject.GetComponent<Animator>().SetTrigger(trigger);
    }
    public float Immobile()
    {
        float oriX = transform.position.x;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveX(oriX - 0.2f, 0.05f));
        seq.Append(transform.DOMoveX(oriX + 0.2f, 0.05f));
        seq.Append(transform.DOMoveX(oriX, 0.05f));
        return 0.15f;
    }
    public virtual float BaseAttack(Sailor target)
    {
        TriggerAnimation("BaseAttack");
        return ArrowToTarget(target);
    }
    public void Death()
    {
        TriggerAnimation("Death");
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.6f);
        seq.AppendCallback(() => { gameObject.SetActive(false); });
    }
    public void DisplayStatus(List<SailorStatus> listStatus)
    {
        ShowInIce(listStatus.Find(x => x.name == SailorStatusType.FROZEN) != null);
    }
    public void ShowInIce(bool b)
    {
        if (b)
            if (iceBlock == null)
            {
                iceBlock = Instantiate(Resources.Load<GameObject>("GameComponents/IceBlock"), transform);
            }
            else iceBlock.SetActive(b);
        else if (iceBlock != null) iceBlock.SetActive(b);
    }
    public virtual void SetFaceDirection(int scaleX)
    {
        float scale = modelObject.transform.localScale.x;
        modelObject.transform.localScale = new Vector3(- scale * scaleX, scale, scale);
    }
    public float ArrowToTarget(Sailor target)
    {
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
}
