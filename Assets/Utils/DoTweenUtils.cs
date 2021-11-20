using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.Utils
{
    public static class EffectUtils
    {
        public static void FadeAppearX(Button button, float distance, float duration, Ease ease = Ease.OutQuint)
        {
            button.transform.Translate(distance, 0, 0);
            Image image = button.GetComponent<Image>();

            float opacity = image.color.a;
            image.color -= new Color(0, 0, 0, opacity);

            Sequence s = DOTween.Sequence();
            s.Insert(0, button.transform.DOMove(new Vector3(-distance, 0, 0), duration).SetRelative().SetEase(ease));
            s.Insert(0, image.DOFade(opacity, duration));
        }
    }
}
