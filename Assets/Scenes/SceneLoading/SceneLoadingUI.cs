using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Facebook.Unity;
using Piratera.Engine;
using Piratera.GUI;
using Piratera.Log;
using Piratera.Network;
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
        SceneManager.LoadScene("SceneLogin");
    }

    public void OnLoadError(string error)
    {
        // SceneManager.LoadScene("SceneLogin");
        GuiManager.Instance.ShowPopupNotification("Check New Version Fail!", "Try Again", () => gameVersionController.GetVersionInfo());
        ;
    }

    public void OnNeedUpdate(string url)
    {
        DOTween.Kill(textInfo.transform);
        textInfo.gameObject.SetActive(false);
        PopupNewVersion popup = GuiManager.Instance.AddGui<PopupNewVersion>("Prefap/PopupNewVersion", LayerId.IMPORTANT).GetComponent<PopupNewVersion>();
        Debug.Log(url);
        popup.SetData(() => Application.OpenURL(GameVersionController.DownloadUrl));
    }

    IEnumerator GetUrlAndPort()
    {
        Debug.Log("url_get_host_and_port: " + Application.version);
        //yield return new WaitForSeconds(1f);
        var formContent = new Dictionary<string, string>();
        formContent.Add("app_version", Application.version);
        using (UnityWebRequest www = UnityWebRequest.Post("url_get_host_and_port", formContent))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("url_get_host_and_port" + www.error);
            }
            else
            {
                GAME_NETWORK_ADDRESS.PROD_HOST = "";
                GAME_NETWORK_ADDRESS.PROD_PORT = 0;
            }
        }
    }
}
