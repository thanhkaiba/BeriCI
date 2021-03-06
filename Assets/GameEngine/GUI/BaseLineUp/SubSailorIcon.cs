using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubSailorIcon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private SailorModel model;
    public SailorModel Model
    {
        set
        {
            UpdateSailorImage(value);
        }
    }
    public IconSailor iconSailor;
    public System.Func<string, Sailor> AddSubSailor;
    private Canvas canvas;
    private bool showedSubsailor;
    private Vector2 beginPos = Vector2.zero;
    public ScrollRect scrollRect;


    void Awake()
    {
        iconSailor = GetComponent<IconSailor>();
        iconSailor.gameObject.AddComponent<TooltipIconSailor>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        iconSailor.ShowClass = true;
    }

    public void OnSelectSubSailor(PointerEventData data)
    {
        Sailor sailor = AddSubSailor(model.id);
        DragableSubsailor drag = sailor.GetComponent<DragableSubsailor>();

        Image dragImage = CreateDragSailorImage(model, canvas.transform);
        iconSailor.SetVisible(false);
        dragImage.transform.position = iconSailor.icon.transform.position;
        drag.SetStartPosition(data.position, dragImage, this);
    }


    public static Image CreateDragSailorImage(SailorModel model, Transform parent)
    {
        GameObject g = new GameObject("drag-" + model.id);
        RectTransform trans = g.AddComponent<RectTransform>();
        Image flyImage = g.AddComponent<Image>();
        flyImage.sprite = GameUtils.GetSailorAvt(model.config_stats.root_name);
        flyImage.SetNativeSize();
        trans.SetParent(parent);
        trans.localScale = new Vector3(-1, 1, 1);
        flyImage.color -= new Color(0, 0, 0, 0.2f);
        return flyImage;
    }

    public void UpdateSailorImage(SailorModel model)
    {
        this.model = model;
        iconSailor.PresentData(model);
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        showedSubsailor = false;
        beginPos = eventData.position;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        showedSubsailor = false;
    }



    public void OnBeginDrag(PointerEventData eventData)
    {

        ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.beginDragHandler);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!model.IsAvaiable())
        {
            ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.dragHandler);
            return;
        }
        Vector2 movePos = eventData.position;
        Vector2 delta = movePos - beginPos;
        if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y) && delta.y > 10 * canvas.transform.localScale.x)
        {
            if (!showedSubsailor)
            {
                showedSubsailor = true;
                OnSelectSubSailor(eventData);
            }
        }
        else if (!showedSubsailor)
        {

            ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.dragHandler);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);
    }


}
