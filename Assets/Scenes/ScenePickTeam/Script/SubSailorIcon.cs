using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SubSailorIcon : MonoBehaviour
{
    public SailorModel model;
    public IconSailor iconSailor;
    private SquadContainer squadContainer;

    void Start()
    {
        iconSailor = GetComponent<IconSailor>();
        squadContainer = FindObjectOfType<SquadContainer>();

        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(data => { OnSelectSubSailor((PointerEventData)data); });

        trigger.triggers.Add(entry);
    }

    public void OnSelectSubSailor(PointerEventData data)
    {
        Sailor sailor = squadContainer.AddSubSailor(model.id);
        DragableSubsailor drag = sailor.GetComponent<DragableSubsailor>();

        Image dragImage = CreateDragSailorImage(model.id);
        iconSailor.SetVisible(false);
        dragImage.transform.position = iconSailor.icon.transform.position;
        drag.SetStartPosition(data.position, dragImage, this);
    }

   
    private Image CreateDragSailorImage(string sailorId)
    {
        GameObject g = new GameObject("drag-" + sailorId);
        RectTransform trans = g.AddComponent<RectTransform>();
        trans.SetParent(GetComponent<Transform>().parent.parent);
        trans.sizeDelta = iconSailor.icon.GetComponent<RectTransform>().sizeDelta;
        Image flyImage = g.AddComponent<Image>();
        flyImage.sprite = model.config_stats.avatar;
        return flyImage;
    }

    public void UpdateSailorImage(SailorModel model)
    {
        this.model = model;
        iconSailor.PresentData(model);
    }



}
