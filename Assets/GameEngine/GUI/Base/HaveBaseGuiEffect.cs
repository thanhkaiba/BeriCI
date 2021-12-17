using UnityEngine;
using DG.Tweening;
using System;

namespace Piratera.GUI
{
    public enum GuiEff
    {
        NONE,
        ZOOM,
        FLY_UP,
        FALL,
        FADE,
        LEFT,
        RIGHT,
    }
    public class HaveBaseGuiEffect: MonoBehaviour
    {

        [SerializeField]
        private GuiEff appearEffect = GuiEff.NONE;

        [SerializeField]
        private GuiEff disappearEffect = GuiEff.NONE;

        [SerializeField]
        private float appearTime = 0.6f;

        [SerializeField]
        private float disappearTime = 0.4f;

        private HaveFog haveFog;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            haveFog = GetComponent<HaveFog>();
            canvasGroup = GetComponent<CanvasGroup>();
        }


        public void runAppearEffect(GuiEff eff)
        {
           

            switch(eff)
            {
                case GuiEff.ZOOM:
                    {
                        
                        Sequence s = DOTween.Sequence();
                        s.SetTarget(transform).SetLink(gameObject);
                       
                        transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                        s.Insert(0, transform.DOScale(new Vector3(1f, 1f, 1f), appearTime));

                        if (haveFog != null)
                        {
                            haveFog.FadeIn(appearTime);
                        }
                    }
                    break;
                case GuiEff.FALL:
                    {
                       
                        Sequence s = DOTween.Sequence();
                        s.SetTarget(transform).SetLink(gameObject);
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = false;
                            s.Insert(0, canvasGroup.DOFade(1, appearTime / 2).From());
                            s.AppendCallback(() => canvasGroup.interactable = true);
                        }
                        s.Insert(0, (transform as RectTransform).DOAnchorPosY(Screen.height + (transform as RectTransform).sizeDelta.y, appearTime).From().SetEase(Ease.OutBack));
                        if (haveFog != null)
                        {
                            haveFog.FadeIn(appearTime);
                        }


                    }
                    break;
                case GuiEff.FLY_UP:
                    {
                     
                        Sequence s = DOTween.Sequence();
                        s.SetTarget(transform).SetLink(gameObject);
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = false;
                            s.Insert(0, canvasGroup.DOFade(1, appearTime / 2).From());
                            s.AppendCallback(() => canvasGroup.interactable = true);
                        }
                        s.Insert(0, (transform as RectTransform).DOAnchorPosY(-(transform as RectTransform).sizeDelta.y, appearTime).From().SetEase(Ease.OutBack));
                        if (haveFog != null)
                        {
                            haveFog.FadeIn(appearTime);
                        }

                    }
                    break;

                case GuiEff.LEFT:
                    {
                       
                        Sequence s = DOTween.Sequence();
                        s.SetTarget(transform).SetLink(gameObject);
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = false;
                            s.Insert(0, canvasGroup.DOFade(1, appearTime / 2).From());
                            s.AppendCallback(() => canvasGroup.interactable = true);
                        }
                        s.Insert(0, (transform as RectTransform).DOAnchorPosX(-(transform as RectTransform).sizeDelta.x, appearTime).From().SetEase(Ease.OutBack));
                        if (haveFog != null)
                        {
                            haveFog.FadeIn(appearTime);
                        }

                    }
                    break;
                case GuiEff.RIGHT:
                    {
                      
                        Sequence s = DOTween.Sequence();
                        s.SetTarget(transform).SetLink(gameObject);
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = false;
                            s.Insert(0, canvasGroup.DOFade(0, appearTime / 2).From());
                            s.AppendCallback(() => canvasGroup.interactable = true);
                        }
                        s.Insert(0, (transform as RectTransform).DOAnchorPosX(Screen.width + (transform as RectTransform).sizeDelta.x, appearTime).From().SetEase(Ease.OutBack));
                        if (haveFog != null)
                        {
                            haveFog.FadeIn(appearTime);
                        }

                    }
                    break;
                case GuiEff.FADE:
                    {
                        Sequence s = DOTween.Sequence();
                        s.SetTarget(transform).SetLink(gameObject);
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = false;
                            s.Insert(0, canvasGroup.DOFade(0, appearTime / 2).From());
                            s.AppendCallback(() => canvasGroup.interactable = true);
                        }
                        if (haveFog != null)
                        {
                            haveFog.FadeIn(appearTime);
                        }

                    }
                    break;


            }
        }

        public void runAppearEffect()
        { 
            runAppearEffect(appearEffect);
        }



        public void runDisappearEffectEff(GuiEff eff, Action func)
        {
            switch (eff)
            {
                case GuiEff.ZOOM:
                    {
                        if (haveFog != null)
                        {
                            haveFog.VisibleFog(false);
                        }
                        Sequence s = DOTween.Sequence();
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = false;
                            s.Insert(0, canvasGroup.DOFade(0, disappearTime));
                        }
                        s.Insert(0, transform.DOScale(new Vector3(3, 3, 3), disappearTime))
                            .AppendCallback(() => func());
                        s.SetTarget(transform);
                        s.SetLink(gameObject);
                    }
                    break;
                case GuiEff.NONE:
                    func();
                    break;
                case GuiEff.FLY_UP:
                    {
                        if (haveFog != null)
                        {
                            haveFog.VisibleFog(false);
                        }

                      
                        Sequence s = DOTween.Sequence();
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = false;
                            s.Insert(0, canvasGroup.DOFade(0, disappearTime));
                        }
                        s.Insert(0, (transform as RectTransform).DOAnchorPosY(Screen.height / 2, disappearTime).SetRelative().SetEase(Ease.InBack))
                            .AppendCallback(() => func());
                        s.SetTarget(transform);
                        s.SetLink(gameObject);

                       
                    }
                    break;

                case GuiEff.FALL:
                    {
                        if (haveFog != null)
                        {
                            haveFog.VisibleFog(false);
                        }


                        Sequence s = DOTween.Sequence();
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = false;
                            s.Insert(0, canvasGroup.DOFade(0, disappearTime));
                        }
                        s.Insert(0, (transform as RectTransform).DOAnchorPosY(-Screen.height / 2, disappearTime).SetRelative().SetEase(Ease.InBack))
                            .AppendCallback(() => func());
                        s.SetTarget(transform);
                        s.SetLink(gameObject);


                    }
                    break;

                case GuiEff.LEFT:
                    {
                        if (haveFog != null)
                        {
                            haveFog.VisibleFog(false);
                        }


                        Sequence s = DOTween.Sequence();
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = false;
                            s.Insert(0, canvasGroup.DOFade(0, disappearTime));
                        }
                        s.Insert(0, (transform as RectTransform).DOAnchorPosX(-Screen.width / 2, disappearTime).SetRelative().SetEase(Ease.InBack))
                            .AppendCallback(() => func());
                        s.SetTarget(transform);
                        s.SetLink(gameObject);


                    }
                    break;
                case GuiEff.RIGHT:
                    {
                        if (haveFog != null)
                        {
                            haveFog.VisibleFog(false);
                        }


                        Sequence s = DOTween.Sequence();
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = false;
                            s.Insert(0, canvasGroup.DOFade(0, disappearTime));
                        }
                        s.Insert(0, (transform as RectTransform).DOAnchorPosX(Screen.width / 2, disappearTime).SetRelative().SetEase(Ease.InBack))
                            .AppendCallback(() => func());
                        s.SetTarget(transform);
                        s.SetLink(gameObject);


                    }
                    break;

                case GuiEff.FADE:
                    {
                        if (haveFog != null)
                        {
                            haveFog.VisibleFog(false);
                        }


                        Sequence s = DOTween.Sequence();
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = false;
                            s.Insert(0, canvasGroup.DOFade(0, disappearTime));
                        }
                        s.AppendCallback(() => func());
                        s.SetTarget(transform);
                        s.SetLink(gameObject);


                    }
                    break;

            }
        }

        public void runDisappearEffectEff(Action func)
        {
            runDisappearEffectEff(disappearEffect, func);
        }
    }
}
