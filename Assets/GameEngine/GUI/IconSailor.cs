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
    private Text textLockTime;
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
    [SerializeField]
    private Sprite starSpr, starSprRed, starSprPurple;
    [SerializeField]
    private Transform nodeStar;
    [SerializeField]
    private GameObject iconTrial;
    public Action<SailorModel> OnClick;
    public SailorModel sailorModel;
    public bool ShowClass = false;
    public bool ShowRank = false;

    public void PresentData(SailorModel model)
    {
        gameObject.name = model.id;
        icon.sprite = GameUtils.GetSailorAvt(model.config_stats.root_name);
        border.color = rankColor[(int)model.config_stats.rank];
        iconLock.gameObject.SetActive(!model.IsAvaiable());

        if (!model.IsAvaiable())
        {
            textLockTime.text = "<" + MathF.Ceiling(model.GetRemainingLockTime() / (60f * 60 * 1000)) + "h";
        }
        else
        {
            textLockTime.text = "";
        }
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

        foreach (Transform child in nodeStar.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        ShowStar(model.star);
        
        iconTrial.SetActive(model.IsTrial());
    }
    private void ShowStar(int star)
    {
        if (star <= 5)
        {
            for (int i = 0; i < star; i++)
            {
                GameObject starObj = new GameObject();
                Image starImg = starObj.AddComponent<Image>();
                starImg.sprite = starSpr;
                starObj.GetComponent<RectTransform>().SetParent(nodeStar);
                starObj.SetActive(true);
                starObj.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
                starObj.transform.localPosition = new Vector3(0, -23 * i);
            }
        }
        else if (star <= 8)
        {
            for (int i = 5; i < star; i++)
            {
                GameObject starObj = new GameObject();
                Image starImg = starObj.AddComponent<Image>();
                starImg.sprite = starSprRed;
                starObj.GetComponent<RectTransform>().SetParent(nodeStar);
                starObj.SetActive(true);
                starObj.transform.localScale = new Vector3(0.35f, 0.35f, 1f);
                starObj.transform.localPosition = new Vector3(-4, -30 * (i - 5) - 4);
            }
        }
        else
        {
            for (int i = 8; i < star; i++)
            {
                GameObject starObj = new GameObject();
                Image starImg = starObj.AddComponent<Image>();
                starImg.sprite = starSprPurple;
                starObj.GetComponent<RectTransform>().SetParent(nodeStar);
                starObj.SetActive(true);
                starObj.transform.localScale = new Vector3(0.45f, 0.45f, 1f);
                starObj.transform.localPosition = new Vector3(-8, -32 * (i - 8) - 8);
            }
        }
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
