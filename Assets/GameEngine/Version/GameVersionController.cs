using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class GameVersionController : MonoBehaviour
{
#if UNITY_STANDALONE_WIN
    private const string URL = "https://api1.piratera.io/v1/game/version/8";
#elif UNITY_ANDROID
    private const string URL = "https://api1.piratera.io/v1/game/version/9";
#endif
    private const string TEST_URL = "localhost:3001";

    [SerializeField]
    public UnityEvent OnCheckSuccess;

    [SerializeField]
    public UnityEvent<string> OnNeedUpdate;

    [SerializeField]
    public UnityEvent<string> OnError;

    public static string DownloadUrl;

    void Start()
    {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        StartCoroutine(GetText());
#else
        OnCheckSuccess.Invoke();
#endif
    }

    IEnumerator GetText()
    {
        yield return new WaitForSeconds(1f);
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            OnError.Invoke(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            JObject o = JObject.Parse(www.downloadHandler.text);
            Debug.Log(float.Parse(Application.version) + " " + (float)o["min_version"]);
            if (float.Parse(Application.version) < (float)o["min_version"])
            {
                DownloadUrl = (string)o["download_url"];
                OnNeedUpdate.Invoke((string)o["download_url"]);
            }
            else
            {
                OnCheckSuccess.Invoke();
            }

        }
    }
}