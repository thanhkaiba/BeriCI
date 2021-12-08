using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HaveFog : MonoBehaviour
{
    [SerializeField]
    private float fogOpacity = 0.5f;
    [SerializeField]
    private bool blockTouchBehind = true;

    public GameObject panel;
    void Awake()
    {
        panel = new GameObject("PanelFog");
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

    public void VisibleFog(bool visible)
    {
        panel.SetActive(visible);
    }
    public void FadeIn(float time = 0.4f)
    {
        var image = panel.GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 0f;
        image.color = tempColor;
        panel.GetComponent<Image>().DOFade(fogOpacity, time);
    }
}
