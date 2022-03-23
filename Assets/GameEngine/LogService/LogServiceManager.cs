using Newtonsoft.Json;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Piratera.Log
{
    class LogServiceManager : Singleton<LogServiceManager>
    {
#if PIRATERA_LIVE
        private const string URL = "https://crash-log.piratera.io/crash-log";
#else
        private const string URL = "http://dev-game1.piratera.local:7676/crash-log";
#endif

        public void SendLog(LogEvent _event, string _params = "")
        {
            var myData = new
            {
                Header = getHeaderFormat(),
                Params = _params,
                Event = _event.ToString()
            };

            StartCoroutine(Post(JsonConvert.SerializeObject(myData)));

        }

        public string getHeaderFormat()
        {
            return $"{SystemInfo.deviceUniqueIdentifier};{SystemInfo.deviceName};{SystemInfo.operatingSystem};{Application.version};{UserData.Instance.UID}";
        }

        IEnumerator Post(string text)
        {

            using (UnityWebRequest www = new UnityWebRequest(URL, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(text);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "text/plain");
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log("Send log success!");

                }
            }


        }
    }
}
