using UnityEngine;
using UnityEngine.UI;
using Piratera.Constance;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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
    [SerializeField]
    private GameObject nodeClass;
    public Action<SailorModel> OnClick;
    private SailorModel sailorModel;
    public bool ShowClass = false;



    public void PresentData(SailorModel model)
    {
        gameObject.name = model.id;
        icon.sprite = model.config_stats.avatar;
        background.sprite = rankSprites[(int)model.config_stats.rank];
        qualitySlider.value = (model.quality * 1f) / GlobalConfigs.SailorGeneral.MAX_QUALITY;
        sailorModel = model;
        if (ShowClass)
        {
            RenderClass(model.config_stats.classes);
        }
    }

    private void RenderClass(List<SailorClass> classes)
    {

        foreach (Transform child in nodeClass.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < classes.Count; i++)
        {
            GameObject GO = new GameObject();
            GO.AddComponent<Image>();
            GO.transform.SetParent(nodeClass.transform);
            Image image = GO.GetComponent<Image>();
            Sprite s = Resources.Load<Sprite>("Icons/SailorType/" + classes[i]);
            image.sprite = s;
            image.rectTransform.sizeDelta = new Vector2(30, 30);
        }
    }

    public void SetVisible(bool visible)
    {
        icon.enabled = visible;
        background.enabled = visible;
        qualitySlider.transform.localScale = visible ? Vector3.one : Vector3.zero;
        nodeClass.SetActive(visible);


    }
    public void ClickSailor()
    {
       
        if (OnClick != null)
        {
            OnClick(sailorModel);
        }
    }
 
}
