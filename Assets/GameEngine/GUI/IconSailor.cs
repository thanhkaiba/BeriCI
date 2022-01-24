using Piratera.Config;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconSailor : MonoBehaviour
{
    [SerializeField]
    private Color[] rankColor;
    [SerializeField]
    private Color[] rankColorInside;
    [SerializeField]
    private Sprite[] spriteRanks;
    [SerializeField]
    public Image icon;
    [SerializeField]
    public Image iconRank;
    [SerializeField]
    private Image background;
    [SerializeField]
    private Image border;
    [SerializeField]
    private Image iconLock;
    [SerializeField]
    private Slider qualitySlider;
    [SerializeField]
    private GameObject nodeClass;
    [SerializeField]
    private GameObject GOClass;
    [SerializeField]
    private GameObject GOFocus;
    [SerializeField]
    private Text level, quality;
    public Action<SailorModel> OnClick;
    public SailorModel sailorModel;
    public bool ShowClass = false;
    public bool ShowRank = false;
   


    public void PresentData(SailorModel model)
    {
        gameObject.name = model.id;
        icon.sprite = GameUtils.GetSailorAvt(model.config_stats.root_name);
        border.color = rankColor[(int)model.config_stats.rank];
        iconLock.gameObject.SetActive(!model.isAvaiable());
        background.color = rankColorInside[(int)model.config_stats.rank];

        iconRank.gameObject.SetActive(ShowRank);
        if (ShowRank)
        {
            iconRank.sprite = spriteRanks[(int)model.config_stats.rank];
        }
        qualitySlider.value = (model.quality * 1f) / GlobalConfigs.SailorGeneral.MAX_QUALITY;
        sailorModel = model;
        if (ShowClass)
        {
            RenderClass(model.config_stats.classes);
        }
        level.text = "" + model.level;
        quality.text = "Q:" + model.quality;
    }
    public void ShowFocus(bool b)
    {
        GOFocus.SetActive(b);
    }
    private void RenderClass(List<SailorClass> classes)
    {

        foreach (Transform child in nodeClass.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < classes.Count; i++)
        {
            GameObject GO = Instantiate(GOClass, nodeClass.transform);
            GO.GetComponent<IconClassInAvt>().SetClass(classes[i]);
            GO.transform.localPosition -= new Vector3(0, i * 45, 0);
        }
    }

    public void SetVisible(bool visible)
    {
        icon.enabled = visible;
        background.enabled = visible;
        qualitySlider.transform.localScale = visible ? Vector3.one : Vector3.zero;
        nodeClass.SetActive(visible && ShowClass);
        iconRank.gameObject.SetActive(visible && ShowRank);


    }
    public void ClickSailor()
    {

        if (OnClick != null)
        {
            OnClick(sailorModel);
        }
    }


}
