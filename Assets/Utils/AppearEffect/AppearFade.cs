using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearFade : MonoBehaviour
{
    public float delayTime = 0;
    public float duration = 0.4f;
    public float fadeValue = 1f;
    void Start()
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        Sequence s = DOTween.Sequence();
        s.AppendInterval(delayTime);
        s.Append(canvasGroup.DOFade(fadeValue, duration).SetRelative());
    }
}
