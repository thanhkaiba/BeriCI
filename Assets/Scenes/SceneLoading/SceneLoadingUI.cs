using DG.Tweening;
using Piratera.Engine;
using Piratera.GUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Piratera.Log;

public class SceneLoadingUI : MonoBehaviour
{
    [SerializeField]
    private Text textInfo;

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        LogServiceManager.Instance.SendLog(LogEvent.OPEN_GAME);

    }

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
        seq.SetLink(textInfo.gameObject).SetTarget(textInfo.transform);
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

    public void OnNeedUpdate(string url)
    {
        DOTween.Kill(textInfo.transform);
        textInfo.gameObject.SetActive(false);
        PopupNewVersion popup = GuiManager.Instance.AddGui<PopupNewVersion>("Prefap/PopupNewVersion", LayerId.IMPORTANT).GetComponent<PopupNewVersion>();
        Debug.Log(url);
        popup.SetData(() => Application.OpenURL(GameVersionController.DownloadUrl));
    }
}
