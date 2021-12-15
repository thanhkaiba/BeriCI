using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearMove : MonoBehaviour
{
    public float delayTime = 0;
    public float duration = 0.4f;
    public float x = 0;
    public float y = 0;
    public Ease ease = Ease.OutCirc;
    void Start()
    {
        transform.Translate(x, y, 0);
        Sequence s = DOTween.Sequence();
        s.AppendInterval(delayTime);
        s.Append(transform.DOMove(new Vector3(-x, -y, 0), duration).SetRelative().SetEase(ease));
    }
}
