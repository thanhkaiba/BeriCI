using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragableSailor : MonoBehaviour
{
    protected Plane sailorPlane;
    private Vector3 diff;
    protected BoxCollider boxAround;
    public SquadSlot[] slots { get; set; }
    [SerializeField]
    protected int selectingIndex = -1;
    protected Sailor sailor;
    [SerializeField]
    protected int originIndex = -1;

    private void Start()
    {
        boxAround = GetComponent<BoxCollider>();
        sailor = GetComponent<Sailor>();

    }

    protected void OnMouseDown()
    {

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetOwner() == sailor)
            {
                originIndex = i;
            }
        }
        selectingIndex = originIndex;
        slots[originIndex].OnSelecting();

        Vector3 normal = new Vector3(0, 0, 0);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (boxAround.Raycast(ray, out RaycastHit hit, 1000))
        {
            normal = hit.point;

        }

        diff = transform.position - normal;

        normal.x = 0;
        normal.z = 0;
        sailorPlane = new Plane(Vector3.up, normal);
    }

  



    protected void OnMouseDrag()
    {


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (sailorPlane.Raycast(ray, out float distance))
        {
            transform.position = ray.GetPoint(distance) + diff;

        }

        CheckNewSelecting();

    }

    protected void OnMouseUp()
    {
        for (int i = 0; i < slots.Length; i++)
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
        slots[selectingIndex].SetSelectedSailer(sailor);
        originIndex = selectingIndex;
        selectingIndex = -1;
    }

    protected void OnMouseUpEmpty()
    {

    }


    private void CheckNewSelecting()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            SquadSlot slot = slots[i];
            if (slot.boxAround.Intersects(boxAround.bounds))
            {
                if (i != selectingIndex)
                {
                    UpdateSlots(i);
                }

            }
        }
    }

    protected virtual void UpdateSlots(int newSelecting)
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
