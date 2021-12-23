using UnityEngine;
using UnityEngine.UI;
using Piratera.Constance;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class IconSailor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
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
    private Slider qualitySlider;
    [SerializeField]
    private GameObject nodeClass;
    [SerializeField]
    private GameObject GOClass;
    [SerializeField]
    private GameObject GOFocus;
    public Action<SailorModel> OnClick;
    private SailorModel sailorModel;
    public bool ShowClass = false;
    public bool ShowRank = false;
    private bool ShowToolTip = true;

    private Vector2 mouseBeginPos;
    private const float MOVE_OFFSET = 50;
    private bool selected = false;
    private bool moved = false;

    public void PresentData(SailorModel model)
    {
        gameObject.name = model.id;
        icon.sprite = model.config_stats.avatar;
        border.color = rankColor[(int)model.config_stats.rank];
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
            GO.transform.localPosition -= new Vector3(0, i * 52, 0);
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

    public void OnPointerUp(PointerEventData eventData)
    {
        if (selected && !moved)
        {
            if (TooltipSailorInfo.Instance != null && ShowToolTip)
            {
                Canvas canvas = FindObjectOfType<Canvas>();
                TooltipSailorInfo.Instance.ShowStaticTooltip(sailorModel, transform.position + new Vector3(0, icon.sprite.rect.height * canvas.transform.localScale.x));

            }
        }
        selected = false;
        moved = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Vector3.Distance(mouseBeginPos, eventData.position) >= MOVE_OFFSET)
        {
            moved = true;
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        mouseBeginPos = eventData.position;
        selected = true;
        moved = false;
    }
}
