using UnityEngine;
using UnityEngine.UI;
using Piratera.Constance;
using System;
using UnityEngine.EventSystems;

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
    public Action<SailorModel> OnClick;
    private SailorModel sailorModel;


    public void PresentData(SailorModel model)
    {
        gameObject.name = model.id;
        icon.sprite = model.config_stats.avatar;
        background.sprite = rankSprites[(int)model.config_stats.rank];
        qualitySlider.value = (model.quality * 1f) / GlobalConfigs.SailorGeneral.MAX_QUALITY;
        sailorModel = model;

    }

    public void SetVisible(bool visible)
    {
        icon.enabled = visible;
        background.enabled = visible;
        qualitySlider.transform.localScale = visible ? Vector3.one : Vector3.zero;


    }
    public void ClickSailor()
    {
       
        if (OnClick != null)
        {
            OnClick(sailorModel);
        }
    }
 
}
