using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GuiWaiting : MonoBehaviour
{
    [SerializeField]
    private Image icon;

    private void Start()
    {
        float opacity = icon.color.a;
        icon.color -= new Color(0, 0, 0, icon.color.a);
        Sequence seq = DOTween.Sequence();
        seq.Append(icon.DOFade(opacity, 0.2f));
        seq.Join(icon.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 1, 0));
        seq.SetLink(gameObject);

        icon.transform.DORotate(new Vector3(0, 0, 360), 1f).SetRelative().SetLoops(-1, LoopType.Incremental).SetLink(gameObject).SetEase(Ease.Linear);
    }
}
