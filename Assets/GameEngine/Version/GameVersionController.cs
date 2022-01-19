using Newtonsoft.Json.Linq;
using Piratera.Build;
using Piratera.Constance;
using Piratera.Log;
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
#if PIRATERA_QC || PIRATERA_QC_DEV
     private static string URL = "https://api.piratera.io/v1/game/version/" + (int)BuildType.WINDOW_DEV;
#else
     private static string URL = "https://api.piratera.io/v1/game/version/" + (int)BuildType.WINDOW_LIVE;
#endif
#elif UNITY_ANDROID

#if PIRATERA_QC || PIRATERA_QC_DEV
     private static string URL = "https://api.piratera.io/v1/game/version/" +  (int)BuildType.ANDROID_DEV;
#else
     private static string URL = "https://api.piratera.io/v1/game/version/" +  (int)BuildType.ANDROID_LIVE;
#endif
#elif UNITY_STANDALONE_OSX
#if PIRATERA_QC || PIRATERA_QC_DEV
     private static string URL = "https://api.piratera.io/v1/game/version/" + (int)BuildType.MACOS_DEV;
#else
     private static string URL = "https://api.piratera.io/v1/game/version/" + (int)BuildType.MACOS_LIVE;
#endif

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
#if UNITY_EDITOR
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
            using (UnityWebRequest www = UnityWebRequest.Get(URL))
            {
                www.certificateHandler = new CustomCertificateHandler();
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                    OnError.Invoke(www.error);
                    LogServiceManager.Instance.SendLog(LogEvent.GET_VERSION_INFO_FAIL);
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);

                    JObject o = JObject.Parse(www.downloadHandler.text);

                    Version version1 = new Version(Application.version);
                    Version version2 = new Version(((string)o["min_version"]).Split('_')[1]);
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
}