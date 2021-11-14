using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HaveFog : MonoBehaviour
{
    [SerializeField]
    private float fogOpacity = 0.5f;
    [SerializeField]
    private bool blockTouchBehind = false;
    // Start is called before the first frame update
    void Start()
    {
        GameObject panel = new GameObject("PanelFog");
        panel.transform.SetParent(transform);
        panel.transform.localPosition = Vector3.zero;
        panel.AddComponent<CanvasRenderer>();
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        panel.transform.SetAsFirstSibling();
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0, 0, 0, fogOpacity);
        image.raycastTarget = blockTouchBehind;

    }
}
