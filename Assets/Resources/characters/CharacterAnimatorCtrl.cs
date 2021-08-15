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
    private CharacterModel model;
    private void Start()
    {
        model = GetComponent<CombatCharacter>().model;
    }
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
    public float BaseAttack(CombatCharacter target)
    {
        TriggerAnimation("BaseAttack");
        if (model == CharacterModel.WARRIOR)
        {
            Vector3 oriPos = transform.position;
            float d = Vector3.Distance(oriPos, target.transform.position);
            Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 2.0f);
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOMove(desPos, 0.3f));
            seq.AppendInterval(0.2f);
            seq.Append(transform.DOMove(oriPos, 0.2f));
            return 0.4f;
        } else
        {
            var arrow = Instantiate(
                Resources.Load<GameObject>("GameComponents/arrow"),
                startArrow.position,
                Quaternion.identity);
            arrow.SetActive(false);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.05f);
            seq.AppendCallback(() => {
                arrow.SetActive(true);
            });
            var targetHeart = new Vector3(target.transform.position.x,
                target.transform.position.y + 2.0f,
                target.transform.position.z
            );
            Vector3 oriPos = transform.position;
            float d = Vector3.Distance(oriPos, targetHeart);
            Vector3 desPos = Vector3.MoveTowards(oriPos, targetHeart, d - 1.4f);
            if (arrow.transform.position.x < desPos.x) arrow.transform.localScale = new Vector3(-1, 1, 1);
            seq.Append(arrow.transform.DOMove(desPos, 0.4f));
            seq.AppendCallback(() => Destroy(arrow));
            return 0.4f;
        }
    }
    public void Death()
    {
        TriggerAnimation("Death");
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.6f);
        seq.AppendCallback(() => { gameObject.SetActive(false); });
    }
    public void DisplayStatus(List<CombatCharacterStatus> listStatus)
    {
        ShowInIce(listStatus.Find(x => x.name == CombatCharacterStatusName.FROZEN) != null);
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
    public void SetFaceDirection(int scaleX)
    {
        float scale = modelObject.transform.localScale.x;
        int xModel = model == CharacterModel.GOBLIN_ARCHER ? -1 : 1;
        modelObject.transform.localScale = new Vector3(scale * xModel * scaleX, scale, scale);
    }
}
