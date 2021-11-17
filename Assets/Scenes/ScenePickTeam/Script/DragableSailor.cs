using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragableSailor : MonoBehaviour
{
    protected BoxCollider boxAround;
    public SquadSlot[] slots { get; set; }
    [SerializeField]
    protected short selectingIndex = -1;
    protected Sailor sailor;
    [SerializeField]
    protected short originIndex = -1;
    

    private Vector3 delta;
    private float originZ;

    private void Start()
    {
        boxAround = GetComponent<BoxCollider>();
        sailor = GetComponent<Sailor>();
    }

    protected void OnMouseDown()
    {
        SetSailorOpacity(0.8f);
        for (short i = 0; i < slots.Length; i++)
        {
            slots[i].selectable = true;
            if (slots[i].GetOwner() == sailor)
            {
                originIndex = i;
            }
        }
        selectingIndex = originIndex;
        slots[originIndex].OnSelecting();

        Vector3 movePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        originZ = transform.position.z;
        delta = transform.position - movePos;

    }

  
    protected void OnMouseDrag()
    {

        Vector3 movePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = movePos + delta;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);


        CheckNewSelecting(transform.position);

    }

    protected void OnMouseUp()
    {
        SetSailorOpacity(1f);
        for (short i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetOwner() == sailor)
            {
                originIndex = i;
            }
        }
        if (selectingIndex >= 0)
        {
            OnMouseUpWithSlot();
        } else
        {
            OnMouseUpEmpty();
        }
    }

    protected void OnMouseUpWithSlot()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, originZ);

        if (CrewData.Instance.FightingTeam.IsSlotEmpty(selectingIndex)) {
            CrewData.Instance.Occupie(sailor.Model.id, selectingIndex);
        } else
        {
           
            CrewData.Instance.Swap(sailor.Model.id, CrewData.Instance.FightingTeam.OwnerOf(selectingIndex));
        }

        slots[selectingIndex].SetSelectedSailer(sailor);
        originIndex = selectingIndex;
        selectingIndex = -1;
    }

    protected void OnMouseUpEmpty()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, originZ);
    }


    private void CheckNewSelecting(Vector3 mousePositon)
    {
        for (short i = 0; i < slots.Length; i++)
        {
            SquadSlot slot = slots[i];
            if (slot.boxAround.Contains(new Vector3(mousePositon.x, mousePositon.y, slot.transform.position.z)) && slot.selectable)
            {
                if (i != selectingIndex)
                {
                    UpdateSlots(i);
                }

            }
        }
    }

    protected virtual void UpdateSlots(short newSelecting)
    {
        if (selectingIndex >= 0)
        {
            slots[selectingIndex].Swap(slots[originIndex]);
        }
        slots[originIndex].Swap(slots[newSelecting]);
        slots[newSelecting].OnSelecting();
        selectingIndex = newSelecting;
    }

    protected void SetSailorOpacity(float opacity)
    {
        try
        {
            Spine.Skeleton skeleton = transform.GetComponentInChildren<SkeletonMecanim>().skeleton;
            Color color = skeleton.GetColor();
            skeleton.SetColor(new Color(color.r, color.g, color.b, opacity));
        } catch (Exception e)
        {

        }
    }
}
