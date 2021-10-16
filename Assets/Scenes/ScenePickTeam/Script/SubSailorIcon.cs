using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SubSailorIcon : MonoBehaviour
{
    private string sailorId;
    private Image image;
    private SquadContainer squadContainer;

    void Start()
    {
        image = GetComponent<Image>();
        squadContainer = FindObjectOfType<SquadContainer>();

        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(data => { OnSelectSubSailor((PointerEventData)data); });

        trigger.triggers.Add(entry);
    }

    public void OnSelectSubSailor(PointerEventData data)
    {
        Sailor sailor = squadContainer.AddSubSailor(sailorId);
        DragableSubsailor drag = sailor.GetComponent<DragableSubsailor>();

        Image dragImage = CreateDragSailorImage(sailorId);
        image.enabled = false;
        dragImage.transform.position = image.transform.position;
        drag.SetStartPosition(data.position, dragImage, image);
    }

   
    private Image CreateDragSailorImage(string sailorId)
    {
        GameObject g = new GameObject("drag-" + sailorId);
        RectTransform trans = g.AddComponent<RectTransform>();
        trans.SetParent(GetComponent<Transform>().parent.parent);
        trans.sizeDelta = new Vector2(110, 110);
        Image flyImage = g.AddComponent<Image>();
        flyImage.sprite = Resources.Load<Sprite>("Icons/IconSailor/" + sailorId);
        return flyImage;
    }

    public void CreateSailorImage(string sailorId)
    {
        this.sailorId = sailorId;
        image.sprite = Resources.Load<Sprite>("Icons/IconSailor/" + sailorId);

    }

    public SubSailorIcon Init(Transform parent)
    {
        RectTransform trans = gameObject.AddComponent<RectTransform>();
        trans.sizeDelta = new Vector2(110, 110);
        trans.SetParent(parent);
        image = gameObject.AddComponent<Image>();
        return this;
    }
}
