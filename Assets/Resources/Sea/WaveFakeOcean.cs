using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveFakeOcean : MonoBehaviour
{
    public bool isReverse = false;
    void Start()
    {
        float x = transform.position.x;
        if (isReverse)
            DOTween.Sequence()
                .Append(transform.DOMoveX(x + 2f, 2).SetEase(Ease.InOutSine))
                .Append(transform.DOMoveX(x, 2).SetEase(Ease.InOutSine))
                .SetLoops(-1);
        else
            DOTween.Sequence()
                .Append(transform.DOMoveX(x - 2f, 2).SetEase(Ease.InOutSine))
                .Append(transform.DOMoveX(x, 2).SetEase(Ease.InOutSine))
                .SetLoops(-1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
