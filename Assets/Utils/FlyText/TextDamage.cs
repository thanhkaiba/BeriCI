using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDamage : MonoBehaviour
{
    public GameObject iconCrit;
    void Start()
    {
        transform.DOMoveY(transform.position.y+70, 1.0f).SetEase(Ease.InOutSine);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1.5f);
        seq.Append(transform.GetComponent<Text>().DOFade(0, 0.2f));
        seq.AppendCallback(() => { Destroy(gameObject); });
    }

    public void Present(float damage, Vector3 p, bool isCrit)
    {
        GetComponent<Text>().text = "-" + (int)damage;
        transform.position = new Vector3(p.x + Random.Range(-10, 30), p.y + Random.Range(100, 125), p.z);
        GetComponent<Text>().fontSize = isCrit ? 32 : 30;
        if (iconCrit != null) iconCrit.SetActive(isCrit);
    }
}
