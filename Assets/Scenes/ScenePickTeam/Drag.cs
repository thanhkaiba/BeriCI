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


        if (ConvertMousePosWorldPos(Input.mousePosition, out Vector3 postion))
        {
            dist = transform.position - postion;
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
        
        if (ConvertMousePosWorldPos(Input.mousePosition, out Vector3 postion))
        {
            transform.position = postion;
            transform.position += dist;
        }

        CheckNewSelecting();

    }

    public bool ConvertMousePosWorldPos(Vector2 mousePosition, out Vector3 worldPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (hPlane.Raycast(ray, out float distance))
        {
            worldPos = ray.GetPoint(distance);
            return true;
        }
        worldPos = Vector3.zero;
        return false;
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
