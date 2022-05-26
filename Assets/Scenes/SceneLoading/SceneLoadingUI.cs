using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Facebook.Unity;
using Piratera.Engine;
using Piratera.GUI;
using Piratera.Log;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadingUI : MonoBehaviour
{
    [SerializeField]
    private Text textInfo;

    [SerializeField]
    private GameVersionController gameVersionController;

    private void Awake()
    {
#if UNITY_EDITOR || !PIRATERA_LIVE
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        LogServiceManager.Instance.SendLog(LogEvent.OPEN_GAME);

        StartCoroutine(GetUrlAndPort());

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }

    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            Debug.Log("Success to Initialize the Facebook SDK");
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
    private bool haveGetServerHost = false;
    private bool haveCheckGameVersion = false;
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

        gameVersionController.GetVersionInfo();
    }
    public void OnLoadSuccess()
    {
        haveCheckGameVersion = true;
        CheckCompleteAndRunSceneLogin();
    }
    public void OnLoadError(string error)
    {
        // SceneManager.LoadScene("SceneLogin");
        GuiManager.Instance.ShowPopupNotification("Check New Version Fail!", "Try Again", () => gameVersionController.GetVersionInfo());
    }

    public void OnNeedUpdate(string url)
    {
        DOTween.Kill(textInfo.transform);
        textInfo.gameObject.SetActive(false);
        PopupNewVersion popup = GuiManager.Instance.AddGui("Prefap/PopupNewVersion").GetComponent<PopupNewVersion>();
        Debug.Log(url);
        popup.SetData(() => Application.OpenURL(GameVersionController.DownloadUrl));
    }

    IEnumerator GetUrlAndPort()
    {
        Debug.Log("url_get_host_and_port: " + Application.version);
        string url = "https://crash-log.piratera.io/api/check-version?version=" + Application.version;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // request.certificateHandler = new CustomCertificateHandler(); // certificate cho android  cu~
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("url_get_host_and_port: " + request.error);
            }
            else
            {
                ResponseLiveServerURL response = JsonUtility.FromJson<ResponseLiveServerURL>(request.downloadHandler.text);
                GAME_NETWORK_ADDRESS.PROD_HOST = response.uri;
                GAME_NETWORK_ADDRESS.PROD_PORT = response.port;
                response.Log();
                Debug.Log("GAME_NETWORK_ADDRESS.PROD_HOST: " + GAME_NETWORK_ADDRESS.PROD_HOST);
                Debug.Log("GAME_NETWORK_ADDRESS.PROD_PORT: " + GAME_NETWORK_ADDRESS.PROD_PORT);
            }
            haveGetServerHost = true;
            CheckCompleteAndRunSceneLogin();
        }
    }
    private void CheckCompleteAndRunSceneLogin()
    {
        if (haveCheckGameVersion && haveGetServerHost)
            SceneManager.LoadScene("SceneLogin");
    }
}

public class ResponseLiveServerURL {
    public string environment;
    public string uri;
    public int port;
    public void Log()
    {
        Debug.Log("ResponseLiveServerURL environment: " + environment + " uri: " + uri + " port: " + port);
    }
}