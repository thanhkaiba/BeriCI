﻿using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.Utils
{
    public static class DoTweenUtils
    {
        public static Sequence FadeAppearX(Button button, float distance, float duration, Ease ease = Ease.OutQuint)
        {
            button.transform.Translate(distance, 0, 0);
            Image image = button.GetComponent<Image>();
            Sequence s = DOTween.Sequence();
            s.Insert(0, button.transform.DOMove(new Vector3(-distance, 0, 0), duration).SetRelative().SetEase(ease));
            s.Insert(0, FadeAppear(image, duration));
            return s;
        }

        public static Sequence FadeAppearY(Button button, float distance, float duration, Ease ease = Ease.OutQuint)
        {
            button.transform.Translate(0, distance, 0);
            Image image = button.GetComponent<Image>();
            Sequence s = DOTween.Sequence();
            s.Insert(0, button.transform.DOMove(new Vector3(0, -distance, 0), duration).SetRelative().SetEase(ease));
            s.Insert(0, FadeAppear(image, duration));
            return s;
        }

        public static Sequence FadeAppearY(Image image, float distance, float duration, Ease ease = Ease.OutQuint)
        {
            image.transform.Translate(0, distance, 0);
            Sequence s = DOTween.Sequence();
            s.Insert(0, image.transform.DOMove(new Vector3(0, -distance, 0), duration).SetRelative().SetEase(ease));
            s.Insert(0, FadeAppear(image, duration));
            return s;
        }

        public static void FadeAppearX(Button button, float distance, float duration, float delay = 0, Ease ease = Ease.OutQuint)
        {
            FadeAppearX(button, distance, duration, ease).SetDelay(delay);
        }


        public static void FadeAppearY(Button button, float distance, float duration, float delay = 0, Ease ease = Ease.OutQuint)
        {
            FadeAppearY(button, distance, duration, ease).SetDelay(delay);
        }

        public static void FadeAppearY(Image image, float distance, float duration, float delay = 0, Ease ease = Ease.OutQuint)
        {
            FadeAppearY(image, distance, duration, ease).SetDelay(delay);
        }


        public static void ButtonBigAppear(Button button, float duration, Vector3 delta, float delay = 0)
        {
            Vector3 originScale = button.transform.localScale;
            button.transform.localScale += delta;
            Image image = button.GetComponent<Image>();
            Sequence s = DOTween.Sequence();

            s.Insert(0, button.transform.DOScale(originScale, duration).SetEase(Ease.OutExpo));
            s.Insert(0, FadeAppear(image, duration));
            s.SetDelay(delay);
        }

        public static Tweener FadeAppear(Image image, float duration, float delay = 0 )
        {
            float opacity = image.color.a;
            image.color -= new Color(0, 0, 0, opacity);
            return image.DOFade(opacity, duration).SetDelay(delay);
        }


    }
}
