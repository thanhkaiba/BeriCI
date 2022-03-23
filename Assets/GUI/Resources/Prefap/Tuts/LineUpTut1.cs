using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineUpTut1 : MonoBehaviour
{
    private int step = 0;
    private bool block = true;
    [SerializeField]
    private Text text;
    [SerializeField]
    private Transform arrow;

    [SerializeField]
    private Transform fog;
    private void Start()
    {
        StartCoroutine(UnlockClick(2));
        ArrowClickLoop();
    }
    private void ArrowClickLoop()
    {
        Sequence s = DOTween.Sequence();
        var curY = arrow.localPosition.y;
        s.Append(arrow.DOLocalMoveY(curY, 0.0f));
        s.Append(arrow.DOLocalMoveY(curY - 30, 0.5f));
        s.SetLoops(-1, LoopType.Yoyo).SetSpeedBased();
    }
    IEnumerator UnlockClick(float time)
    {
        yield return new WaitForSeconds(time);
        block = false;
    }
    public void OnClickPanel()
    {
        Debug.Log("click");
        if (!block)
        {
            step += 1;
            switch (step)
            {
                case 1:
                    {
                        Sequence s = DOTween.Sequence();
                        var canvasGroup = GetComponent<CanvasGroup>();
                        s.Append(canvasGroup.DOFade(0, 0.6f));
                        s.AppendCallback(() =>
                        {
                            Destroy(gameObject);
                        });
                        block = true;
                        break;
                    }
            }
        }
    }
}

