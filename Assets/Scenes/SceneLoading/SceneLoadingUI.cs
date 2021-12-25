using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneLoadingUI : MonoBehaviour
{
    [SerializeField]
    private Text textInfo;

    private void Start()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => textInfo.text = "Checking Game Version");
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => textInfo.text = "Checking Game Version.");
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => textInfo.text = "Checking Game Version..");
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => textInfo.text = "Checking Game Version...");
        seq.SetLink(gameObject).SetTarget(transform);
        seq.SetLoops(-1);
    }
    public void OnLoadSuccess()
    {
        SceneManager.LoadScene("SceneLogin");
    }

    public void OnLoadError(string error)
    {
        SceneManager.LoadScene("SceneLogin");
    }
}
