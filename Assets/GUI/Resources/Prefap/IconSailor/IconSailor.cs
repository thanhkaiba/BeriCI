using UnityEngine;
using UnityEngine.UI;
using Piratera.Constance;

public class IconSailor : MonoBehaviour
{
   
    [SerializeField]
    private Sprite[] rankSprites;
    [SerializeField]
    public Image icon;
    [SerializeField]
    private Image background;
    [SerializeField]
    private Slider qualitySlider;


    public void PresentData(SailorModel sailorModel)
    {
        gameObject.name = sailorModel.id;
        icon.sprite = sailorModel.config_stats.avatar;
        background.sprite = rankSprites[(int)sailorModel.config_stats.rank];
        qualitySlider.value = (sailorModel.quality * 1f) / GameConst.MAX_QUALITY;

    }

    public void SetVisible(bool visible)
    {
        icon.enabled = visible;
        background.enabled = visible;
        qualitySlider.transform.localScale = visible ? Vector3.one : Vector3.zero;


    }
}
