using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class CharacterAnimatorCtrl : MonoBehaviour
{
    public GameObject modelObject;
    public CharBarControl barControl;

    public Image iceBlock;
    private void Start()
    {
        iceBlock.gameObject.SetActive(false);
    }
    public void TriggerAnimation(string trigger)
    {
        modelObject.GetComponent<Animator>().SetTrigger(trigger);
    }
    public void Immobile()
    {
        float oriX = transform.position.x;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveX(oriX - 0.5f, 0.1f));
        seq.Append(transform.DOMoveX(oriX + 0.5f, 0.1f));
        seq.Append(transform.DOMoveX(oriX, 0.1f));
    }
    public float BaseAttack(CombatCharacter target)
    {
        TriggerAnimation("BaseAttack");

        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, target.transform.position);
        Vector3 desPos = Vector3.MoveTowards(oriPos, target.transform.position, d - 2.0f);
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(desPos, 0.3f));
        seq.AppendInterval(0.2f);
        seq.Append(transform.DOMove(oriPos, 0.3f));
        return 0.8f;
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
        iceBlock.gameObject.SetActive(b);
    }
}
