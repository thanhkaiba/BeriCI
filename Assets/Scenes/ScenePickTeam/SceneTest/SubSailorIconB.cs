using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SubSailorIconB : MonoBehaviour
{
    private string sailorId;
    private string sailorName;
    private Image image;
    private SquadBContainer squadContainer;

    void Start()
    {
        image = GetComponent<Image>();
        squadContainer = FindObjectOfType<SquadBContainer>();

        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(data => { OnSelectSubSailor((PointerEventData)data); });

        trigger.triggers.Add(entry);
    }

    public void OnSelectSubSailor(PointerEventData data)
    {
        Sailor sailor = squadContainer.AddSubSailor(sailorId);
        DragableBSubsailor drag = sailor.GetComponent<DragableBSubsailor>();
        sailor.transform.localScale = new Vector3(-1, 1, 1);

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
        flyImage.sprite = Resources.Load<Sprite>("Icons/IconSailor/" + sailorName);
        return flyImage;
    }

    public void CreateSailorImage(string id, string name)
    {
        sailorId = id;
        sailorName = name;
        image.sprite = Resources.Load<Sprite>("Icons/IconSailor/" + sailorName);
    }

    public SubSailorIconB Init(Transform parent)
    {
        RectTransform trans = gameObject.AddComponent<RectTransform>();
        trans.sizeDelta = new Vector2(110, 110);
        trans.SetParent(parent);
        image = gameObject.AddComponent<Image>();
        return this;
    }
}
