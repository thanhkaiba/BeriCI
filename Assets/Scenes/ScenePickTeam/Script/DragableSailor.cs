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

    private void Start()
    {
        boxAround = GetComponent<BoxCollider>();
        sailor = GetComponent<Sailor>();
    }

    protected void OnMouseDown()
    {
       
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
        Debug.Log("on Touch");
    }

  



    protected void OnMouseDrag()
    {

        Debug.Log("on onDrag");
        Vector3 movePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        movePos.z = transform.position.z;
        transform.position = movePos;

        CheckNewSelecting(movePos);

    }

    protected void OnMouseUp()
    {
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
        } 
    }

    protected void OnMouseUpWithSlot()
    {
        

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

}
