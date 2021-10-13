using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircleSkillAvaiable : MonoBehaviour
{
    GameObject ch;
    public void SetCharacter(GameObject character)
    {
        ch = character;
    }
    void Update()
    {
        transform.Rotate(Vector3.up * (60 * Time.deltaTime));
    }
    void LateUpdate()
    {
        Vector3 charP = ch.transform.position;
        transform.position = new Vector3(charP.x, 0, charP.z);
        if (!ch.activeSelf) gameObject.SetActive(false);
    }
    public void Appear()
    {
        gameObject.SetActive(true);
        transform.localScale = new Vector3(0, 0, 0);
        transform.DOScale(new Vector3(0.25f, 0.25f, 0.25f), 0.5f).SetEase(Ease.OutBack); ;
    }
    public void Disappear()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(new Vector3(0, 0, 0), 0.25f).SetEase(Ease.InBack));
        seq.AppendCallback(() => gameObject.SetActive(false));
    }
}
