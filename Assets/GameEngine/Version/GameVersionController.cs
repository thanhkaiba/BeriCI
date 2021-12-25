using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Piratera.GUI;
using Newtonsoft.Json.Linq;
using UnityEngine.Events;

public class GameVersionController : MonoBehaviour
{
    private const string URL = "https://api1.piratera.io/v1/game/version/8";
    private const string TEST_URL = "localhost:3001";

    [SerializeField]
    public UnityEvent OnCheckSuccess;

    [SerializeField]
    public UnityEvent<string> OnNeedUpdate;

    [SerializeField]
    public UnityEvent<string> OnError;

    void Start()
    {
#if UNITY_EDITOR
        OnCheckSuccess.Invoke();
#else
        StartCoroutine(GetText());
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
                OnNeedUpdate.Invoke((string)o["download_url"]);
            }  else
            {
                OnCheckSuccess.Invoke();
            }
  
        }
    }
}