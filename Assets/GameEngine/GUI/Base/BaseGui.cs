using UnityEngine;

namespace Piratera.GUI
{
    public class BaseGui : MonoBehaviour
    {
        private HaveBaseGuiEffect baseGuiEffect;

        protected virtual void Start()
        {
            baseGuiEffect = GetComponent<HaveBaseGuiEffect>();
            RunAppear();
        }

        protected void RunAppear()
        {
            if (baseGuiEffect != null)
            {
                baseGuiEffect.runAppearEffect();
            }
        }

        public void DestroySelf()
        {
            GuiManager.Instance.DestroyGui(GetType().Name);
        }

        public void RunDestroy()
        {
            if (baseGuiEffect != null)
            {
                baseGuiEffect.runDisappearEffectEff(DestroySelf);
            }
            else
            {
                DestroySelf();
            }
        }

        public void RunDestroy(GuiEff eff)
        {
            if (baseGuiEffect != null)
            {
                baseGuiEffect.runDisappearEffectEff(eff, DestroySelf);
            }
            else
            {
                DestroySelf();
            }
        }
    }
}
