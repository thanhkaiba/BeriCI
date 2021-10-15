using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragableSubsailor : DragableSailor
{
    private const int TransformHeight = 250;
    private Image dragImage;
    private Image originImage;

    private void Start()
    {
       
    }
    public void SetStartPosition(Vector2 mousePosition2D, Image d, Image o)
    {
        dragImage = d;
        d.raycastTarget = false;
        o.raycastTarget = false;
        originImage = o;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition2D);
        if (boxAround == null)
        {
            boxAround = GetComponent<BoxCollider>();
        }
        Plane startPlane = new Plane(Vector3.up, Vector3.zero);

        if (startPlane.Raycast(ray, out float distance))
        {
            Vector3 startPos = ray.GetPoint(distance);
            startPos.y = 0;
            transform.position = startPos;

        }
    }


    private void Update()
    {
        OnMouseDrag();
        if (Input.mousePosition.y > TransformHeight)
        {
            dragImage.enabled = false;
        }
        else
        {
            dragImage.enabled = true;
            dragImage.transform.position = Input.mousePosition;
        }
    }



}
