using System;
using UnityEngine;
using UnityEngine.UI;

public class DragableSubsailor : DragableSailor
{

    private SubSailorIcon subSailorIcon;
    private Sailor swapSailor;
    public Action<string, short> ReplaceSailorAction;
    public Func<bool> IsSquadFull;
    public Action<GameObject> OnTransfromSailor;

    public void SetStartPosition(Vector2 mousePosition2D, Image d, SubSailorIcon subSailorIcon)
    {
        draging = true;
        this.subSailorIcon = subSailorIcon;
        SetSailorOpacity(0.8f);
        dragImage = d;

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
                slot.Selectable = IsSquadFull();
            }
            else
            {
                slot.Selectable = true;
            }
        }
    }

    new void OnMouseUpEmpty()
    {

        subSailorIcon.iconSailor.SetVisible(true);
        Destroy(gameObject);
    }

    new void OnMouseUp()
    {
        SetSailorOpacity(1f);
        Destroy(dragImage.gameObject);

        if (selectingIndex >= 0 && !dragImage.enabled)
        {
            OnMouseUpWithSlot();
        }
        else
        {
            OnMouseUpEmpty();
        }

        foreach (SquadSlot slot in slots)
        {
            slot.Selectable = true;
        }
    }

    new void OnMouseUpWithSlot()
    {
        if (swapSailor != null)
        {
            subSailorIcon.UpdateSailorImage(swapSailor.Model);
            subSailorIcon.iconSailor.SetVisible(true);
            Destroy(swapSailor.gameObject);
        }
        else
        {
            DestroyImmediate(subSailorIcon.gameObject);
        }

        OnTransfromSailor(gameObject);
        slots[selectingIndex].SetSelectedSailer(sailor);


        ReplaceSailorAction(sailor.Model.id, selectingIndex);
        Destroy(this);
        TutorialMgr.Instance.CheckRunTutStartUp_StepLineUpBack2Lobby();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            if (mousePosition.y > (TransformHeight * canvas.transform.localScale.y))
            {
                dragImage.enabled = false;
                ShowChild(true);
                OnDragSailor(mousePosition);

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
                ShowChild(false);
                dragImage.transform.position = mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp();
        }
    }
    private void ShowChild(bool isShow)
    {
        int children = transform.childCount;
        for (int i = 0; i < children; ++i)
            transform.GetChild(i).gameObject.SetActive(isShow);
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
