using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDamage : MonoBehaviour
{
    void Start()
    {
        transform.DOMoveY(transform.position.y+30, 0.0f).SetEase(Ease.InOutSine);
        transform.DOMoveY(transform.position.y+70, 1.0f).SetEase(Ease.InOutSine);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1.5f);
        seq.Append(transform.GetComponent<Text>().DOFade(0, 0.2f));
        seq.AppendCallback(() => { Destroy(gameObject); });
    }

    void Update()
    {
        
    }
}
