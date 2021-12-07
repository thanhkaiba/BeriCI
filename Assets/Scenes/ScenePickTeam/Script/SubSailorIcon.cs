using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SubSailorIcon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public SailorModel model;
    public IconSailor iconSailor;
    public System.Func<string, Sailor> AddSubSailor;
    private Canvas canvas;
    private bool draggingSlot;
    private bool showedSubsailor;
    private Vector2 beginPos = Vector2.zero;
    public ScrollRect scrollRect;


    void Start()
    {
        iconSailor = GetComponent<IconSailor>();
        canvas = FindObjectOfType<Canvas>();
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
        flyImage.sprite = model.config_stats.avatar;
        flyImage.SetNativeSize();
        trans.SetParent(parent);
        trans.localScale = Vector3.one;
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
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        beginPos = Vector2.zero;
        StopAllCoroutines();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        showedSubsailor = false;
        StopAllCoroutines();
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(0.25f);
        draggingSlot = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
   
        ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.beginDragHandler);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if ((draggingSlot && Vector2.Distance(eventData.position, beginPos) < 130 * canvas.transform.localScale.x))
        {
            showedSubsailor = true;
            OnSelectSubSailor(eventData);
        }
        else if (!showedSubsailor)
        {
            StopAllCoroutines();
            ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.dragHandler);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);
        draggingSlot = false;
        beginPos = Vector2.zero;
    }


}
