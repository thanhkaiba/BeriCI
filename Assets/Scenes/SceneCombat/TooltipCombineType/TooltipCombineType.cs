using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipCombineType : MonoBehaviour
{
    public static TooltipCombineType Instance;
    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        Instance = null;
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Rect size = GetComponent<RectTransform>().rect;
            Vector2 posInImage = transform.position - Input.mousePosition;
            if (posInImage.x >= -size.width/2 && posInImage.x < size.width/2 &&
                 posInImage.y >= -size.height/2 && posInImage.y < size.height/2)
            {
                // click inside
            }
            else gameObject.SetActive(false);
        }
    }
    public void ShowTooltipPassiveType(PassiveType data, Vector2 pos)
    {
        Debug.Log("ShowTooltipPassiveType " + data.type + "pos: " + pos.x + " " + pos.y);
        gameObject.SetActive(true);

        // dong thanh ham limit width height
        float width = GetComponent<RectTransform>().rect.width;
        float height = GetComponent<RectTransform>().rect.height;
        if (pos.x < 180 + width / 2) pos.x = 180 + width / 2;
        if (pos.x > Screen.width - 180 - width / 2) pos.x = Screen.width - 180 - width / 2;
        if (pos.y < 20 + height / 2) pos.y = 20 + height / 2;
        if (pos.y > Screen.height - 20 - height / 2) pos.y = Screen.height - 20 - width / 2;
        transform.position = pos;

    }
}
