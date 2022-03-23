using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HandTuts : MonoBehaviour
{
    [SerializeField]
    private Transform hand;
    [SerializeField]
    private Transform fog;
    void Start()
    {
        FadeIn();
        HandClickLoop();
    }
    private void FadeIn()
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 1.0f).SetRelative();
    }
    private void HandClickLoop()
    {
        Sequence s = DOTween.Sequence();
        var curY = hand.localPosition.y;
        s.Append(hand.DOLocalMoveY(curY + 20, 0.0f));
        s.Append(hand.DOLocalMoveY(curY, 0.5f));
        s.SetLoops(-1, LoopType.Yoyo).SetSpeedBased();
    }
    public void OnClick()
    {
        Debug.Log("click right");
        Destroy(gameObject);
    }
}
