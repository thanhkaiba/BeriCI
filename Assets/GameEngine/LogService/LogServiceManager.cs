using Newtonsoft.Json;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Piratera.Log
{
    class LogServiceManager : Singleton<LogServiceManager>
    {
        private const string URL = "http://crash-log.piratera.io/crash-log";


        public void SendLog(LogEvent _event, string _params = "")
        {
            var myData = new
            {
                Header = getHeaderFormat(),
                Params = _params,
                Event = _event
            };

            StartCoroutine(Post(JsonConvert.SerializeObject(myData)));
          
        }

        public string getHeaderFormat() 
        {
            return $"{SystemInfo.deviceUniqueIdentifier};{SystemInfo.deviceName};{SystemInfo.operatingSystem};{Application.version};{UserData.Instance.UID}";
        }

        IEnumerator Post(string text)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(URL, text))
            {
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
