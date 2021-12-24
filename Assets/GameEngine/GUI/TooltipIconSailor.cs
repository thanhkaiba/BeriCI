using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipIconSailor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private bool ShowToolTip = true;

    private Vector2 mouseBeginPos;
    private const float MOVE_OFFSET = 50;
    private bool selected = false;
    private bool moved = false;
    private IconSailor iconSailor;

    private void Awake()
    {
        iconSailor = GetComponent<IconSailor>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (selected && !moved)
        {
            if (TooltipSailorInfo.Instance != null && ShowToolTip)
            {
                Canvas canvas = FindObjectOfType<Canvas>();
                TooltipSailorInfo.Instance.ShowStaticTooltip(iconSailor.sailorModel, transform.position + new Vector3(0, iconSailor.icon.sprite.rect.height * canvas.transform.localScale.x));

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