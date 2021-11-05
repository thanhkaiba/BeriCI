using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragableSubsailor : DragableSailor
{
    private const int TransformHeight = 100;
    private Image dragImage;
    private Image originImage;
    private Sailor swapSailor;
    private GameObject model;

    private void Start()
    {
        boxAround = GetComponent<BoxCollider>();
        sailor = GetComponent<Sailor>();

    }

    public void SetStartPosition(Vector2 mousePosition2D, Image d, Image o)
    {

        Transform t = transform.Find("model");
        if (t != null)
        {
            model = t.gameObject;
        }

        dragImage = d;
        originImage = o;

        Vector3 startPos = Camera.main.ScreenToWorldPoint(mousePosition2D);
        startPos.z = transform.position.z;
        transform.position = startPos;

        if (boxAround == null)
        {
            boxAround = GetComponent<BoxCollider>();
        }
      

        foreach (SquadSlot slot in slots)
        {
            if (slot.GetOwner() == null)
            {
                slot.selectable = !SquadData.Instance.IsSquadFull();
            }
            else
            {
                slot.selectable = true;
            }
        }
    }

    new void OnMouseUpEmpty()
    {
     
        originImage.enabled = true;
        Destroy(gameObject);
    }

    new void OnMouseUp()
    {

       
        Destroy(dragImage.gameObject);

        if (selectingIndex >= 0 && !dragImage.enabled)
        {
            OnMouseUpWithSlot();
        }
        else
        {
            OnMouseUpEmpty();
        }
    }

    new void OnMouseUpWithSlot()
    {

        SquadData.Instance.Replace(sailor.Model.id, selectingIndex);

        if (swapSailor != null)
        {
            originImage.GetComponent<SubSailorIcon>().CreateSailorImage(swapSailor.Model.id, swapSailor.Model.name);
            originImage.enabled = true;
            Destroy(swapSailor.gameObject);
        } else
        {
            Destroy(originImage.gameObject);
        }

        DragableSailor dragable = gameObject.AddComponent<DragableSailor>();
        dragable.slots = slots;
        slots[selectingIndex].SetSelectedSailer(sailor);
        Destroy(this);

    }

    private void Update()
    {

        if (Input.GetMouseButton(0))
        {

            if (Input.mousePosition.y > TransformHeight)
            {
                dragImage.enabled = false;
                model.SetActive(true);
                OnMouseDrag();

            }
            else
            {
                if (selectingIndex >= 0)
                {
                    if (swapSailor != null)
                    {
                        swapSailor.gameObject.SetActive(true);
                    }
                    slots[selectingIndex].SetSelectedSailer(swapSailor);
                    selectingIndex = -1;
                }
                dragImage.enabled = true;
                model.SetActive(false);
                dragImage.transform.position = Input.mousePosition;
            } 
        } else if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp();
        }
    }

    protected override void UpdateSlots(short newSelecting)
    {
        if (selectingIndex >= 0)
        {
            if (swapSailor != null)
            {
                swapSailor.gameObject.SetActive(true);
            }
            slots[selectingIndex].SetSelectedSailer(swapSailor);
        }
        swapSailor = slots[newSelecting].GetOwner();
        if (swapSailor != null)
        {
            swapSailor.gameObject.SetActive(false);
        }
        slots[newSelecting].OnSelecting();
        selectingIndex = newSelecting;
    }



}
