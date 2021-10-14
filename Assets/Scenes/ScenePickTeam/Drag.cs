using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    private Vector3 dist;
    private Plane hPlane;
    public Slot[] slots;
    private Collider boxAround;
    [SerializeField]
    private int selectingIndex = -1;
    private Sailor sailor;
    [SerializeField]
    private int originIndex;

    private void Start()
    {
        hPlane = new Plane(new Vector3(0, 1, 0), Vector3.up * (transform.position.y));
        boxAround = GetComponent<Collider>();
        sailor = GetComponent<Sailor>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetOwner() == sailor)
            {
                originIndex = i;
            }
        }

    }

    private void OnMouseDown()
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


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance = 0;
        if (hPlane.Raycast(ray, out distance))
        {
            dist = transform.position - ray.GetPoint(distance);

        }
    }

    private void OnMouseUp()
    {
       
        if (selectingIndex >= 0)
        {
            slots[selectingIndex].SetSelectedSailer(sailor);
            originIndex = selectingIndex;
            selectingIndex = -1;
            
        }
    }


    void OnMouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance = 0;
        if (hPlane.Raycast(ray, out distance))
        {
            transform.position = ray.GetPoint(distance) + dist;
        }

        CheckNewSelecting();

    }

    private void CheckNewSelecting()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Slot slot = slots[i];
            if (slot.boxAround.bounds.Intersects(boxAround.bounds))
            {
                if (i != selectingIndex)
                {
                    UpdateSlots(i);
                }
               
            }
        }
    }

    private void UpdateSlots(int newSelecting)
    {
        if (selectingIndex >= 0)
        {
            Debug.Log("Swap: " + selectingIndex + " " + originIndex);
            slots[selectingIndex].Swap(slots[originIndex]);
        }
        slots[originIndex].Swap(slots[newSelecting]);
        slots[newSelecting].OnSelecting();
        selectingIndex = newSelecting;
    }
}
