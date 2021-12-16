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

    private GameObject fog;
    void Awake()
    {
        fog = new GameObject("PanelFog");
        fog.transform.SetParent(transform);
        fog.transform.localPosition = Vector3.zero;
        fog.AddComponent<CanvasRenderer>();
        RectTransform rectTransform = fog.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        fog.transform.SetAsFirstSibling();
        Image image = fog.AddComponent<Image>();
        image.color = new Color(0, 0, 0, fogOpacity);
        image.raycastTarget = blockTouchBehind;
    }
    public void VisibleFog(bool visible)
    {
        fog.SetActive(visible);
    }
    public void FadeIn(float time = 0.4f)
    {
        var image = fog.GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 0f;
        image.color = tempColor;
        fog.GetComponent<Image>().DOFade(fogOpacity, time).SetTarget(transform).SetLink(gameObject);
    }
    public void FadeOut(float time = 0.4f)
    {
        var image = fog.GetComponent<Image>();
        var tempColor = image.color;
        image.color = tempColor;
        fog.GetComponent<Image>().DOFade(0, time).SetTarget(transform).SetLink(gameObject);
    }
}
