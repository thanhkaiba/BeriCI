using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Piratera.GUI;
using Newtonsoft.Json.Linq;
using UnityEngine.Events;

public class GameVersionController : MonoBehaviour
{
    private const string URL = "https://api1.piratera.io/v1/users/nonce?ethAddress=0x5480a34c99a78ab25a15667c3c64a07aec244cbe";
    private const string TEST_URL = "localhost:3001";

    [SerializeField]
    public UnityEvent OnCheckSuccess;

    [SerializeField]
    public UnityEvent<string> OnError;

    void Start()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        yield return new WaitForSeconds(1f);
        UnityWebRequest www = UnityWebRequest.Get(TEST_URL);
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
            if (float.Parse(Application.version) < float.Parse((string)o["min_version"]))
            {
                PopupNewVersion popup = GuiManager.Instance.AddGui<PopupNewVersion>("Prefap/PopupNewVersion", LayerId.IMPORTANT).GetComponent<PopupNewVersion>();
                popup.SetData(() => Application.OpenURL((string)o["download_url"]));
            }  else
            {
                OnCheckSuccess.Invoke();
            }
  
        }
    }
}