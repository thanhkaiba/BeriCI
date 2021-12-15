using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearZoom : MonoBehaviour
{
    public float delayTime = 0;
    public float duration = 0.4f;
    public float startScale = 0.4f;
    public Vector3 endScale = new Vector3(1, 1, 1);
    public Ease ease = Ease.OutBack;
    void Start()
    {
        transform.localScale = new Vector3(startScale, startScale, startScale);
        Sequence s = DOTween.Sequence();
        s.AppendInterval(delayTime);
        s.Append(transform.DOScale(endScale, duration).SetEase(ease));
    }
}
