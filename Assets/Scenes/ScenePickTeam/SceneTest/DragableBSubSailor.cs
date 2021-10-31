using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragableBSubsailor : DragableBSailor
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
        Ray ray = Camera.main.ScreenPointToRay(mousePosition2D);
        if (boxAround == null)
        {
            boxAround = GetComponent<BoxCollider>();
        }
        sailorPlane = new Plane(Vector3.up, Vector3.zero);

        if (sailorPlane.Raycast(ray, out float distance))
        {
            Vector3 startPos = ray.GetPoint(distance);
            startPos.y = 0;
            transform.position = startPos;

        }

        foreach (SquadSlot slot in slots)
        {
            if (slot.GetOwner() == null)
            {
                slot.selectable = !SquadBData.Instance.IsSquadFull();
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

        SquadBData.Instance.Replace(sailor.Model.id, selectingIndex);

        if (swapSailor != null)
        {
            originImage.GetComponent<SubSailorIconB>().CreateSailorImage(swapSailor.Model.id, swapSailor.Model.name);
            originImage.enabled = true;
            Destroy(swapSailor.gameObject);
        } else
        {
            Destroy(originImage.gameObject);
        }

        DragableBSailor dragable = gameObject.AddComponent<DragableBSailor>();
        sailor.transform.localScale = new Vector3(-1, 1, 1);
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
