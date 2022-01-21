using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralInfoBar : MonoBehaviour
{
    [SerializeField]
    private Text level;
    [SerializeField]
    private Text quality;
    [SerializeField]
    private Slider qualitySlider;
    public void PresentData(SailorModel model)
    {
        level.text = "" + model.level;
        quality.text = "" + model.quality;
        qualitySlider.value = model.quality / GlobalConfigs.SailorGeneral.MAX_QUALITY;
    }
}
