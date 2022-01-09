using DG.Tweening;
using Piratera.Engine;
using Piratera.GUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadingUI : MonoBehaviour
{
    [SerializeField]
    private Text textInfo;

    private void Awake()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
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
