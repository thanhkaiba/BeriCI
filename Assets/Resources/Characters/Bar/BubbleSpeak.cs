using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleSpeak : MonoBehaviour
{
    [SerializeField]
    private Text content;
    public void ShowText(string text, float time)
    {
        content.text = text;
        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        Sequence s = DOTween.Sequence();
        s.Append(canvasGroup.DOFade(1, 0.5f));
        s.AppendInterval(time - 0.5f);
        s.Append(canvasGroup.DOFade(0, 0.5f));
        s.AppendCallback(() => Destroy(gameObject));
    }
}
