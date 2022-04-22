    using DG.Tweening;
using Piratera.GUI;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        seq.Append(icon.DOFade(opacity, 0.5f));
        seq.Join(icon.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 1, 0));
        seq.AppendInterval(15f);
        //seq.AppendCallback(() =>
        //{
        //    SceneManager.LoadScene("SceneLogin");
        //    Destroy(gameObject);
        //});
        seq.SetLink(gameObject);

        icon.transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.WorldAxisAdd).SetRelative().SetLoops(-1, LoopType.Incremental).SetLink(gameObject);
    }
    public void SetTransparent(bool visible)
    {
        GetComponent<CanvasGroup>().alpha = visible ? 1 : 0;
    }
}
