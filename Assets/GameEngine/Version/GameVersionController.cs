using Newtonsoft.Json.Linq;
using Piratera.Constance;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Piratera.Engine
{
    public class GameVersionController : MonoBehaviour
    {
#if UNITY_STANDALONE_WIN
    private const string URL = GameConst.WINDOW_VERSION_URL;
#elif UNITY_ANDROID
    private const string URL = GameConst.ANDROID_VERSION_URL;
#else
    private const string URL = "";
#endif

        [SerializeField]
        public UnityEvent OnCheckSuccess;

        [SerializeField]
        public UnityEvent<string> OnNeedUpdate;

        [SerializeField]
        public UnityEvent<string> OnError;

        public static string DownloadUrl;

        void Start()
        {
#if !UNITY_EDITOR
        if (!string.IsNullOrEmpty(URL))
        {
            StartCoroutine(GetText());
        } else
        {
            OnCheckSuccess.Invoke();
        }
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

                Version version1 = new Version(Application.version);
                Version version2 = new Version((string)o["min_version"]);
                if (version2.CompareTo(version1) > 0)
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
}