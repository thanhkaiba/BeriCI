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
        FADE
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
        private void Start()
        {
            haveFog = GetComponent<HaveFog>();
        }



        public void runAppearEffect(GuiEff eff)
        {
           

            switch(eff)
            {
                case GuiEff.ZOOM:
                    {
                        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), appearTime, 2, 0);
                    }
                    break;
                case GuiEff.NONE:
                    break;
                case GuiEff.FLY_UP:
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
                        if (haveFog)
                        {
                            haveFog.VisibleFog(false);
                        }
                        Sequence s = DOTween.Sequence();
                        s.Append(transform.DOScale(new Vector3(0, 0, 0), disappearTime))
                            .AppendCallback(() => func());
                    }
                    break;
                case GuiEff.NONE:
                    func();
                    break;
                case GuiEff.FLY_UP:
                    break;

            }
        }

        public void runDisappearEffectEff(Action func)
        {
            runDisappearEffectEff(disappearEffect, func);
        }
    }
}
