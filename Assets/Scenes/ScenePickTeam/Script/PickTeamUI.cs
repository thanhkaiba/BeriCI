
using UnityEngine;
using UnityEngine.UI;
using Piratera.Utils;
using DG.Tweening;

public class PickTeamUI : MonoBehaviour
{
    [SerializeField]
    private Image title;
    [SerializeField]
    private Image backgroundSailorList;
    private Canvas canvas;
    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        RunAppearAction();

    }

    private void RunAppearAction()
    {
        float scale = canvas.transform.localScale.x;
        float titleHeight = ((RectTransform)title.transform).sizeDelta.y * scale;
        DoTweenUtils.FadeAppearY(title, titleHeight + 20, 0.5f, Ease.OutFlash);

        float slotHeight = ((RectTransform)backgroundSailorList.transform).sizeDelta.y * 2 * scale;
        DoTweenUtils.FadeAppearY(backgroundSailorList, -slotHeight*1.5f, 0.5f, Ease.OutFlash);
    }
}
