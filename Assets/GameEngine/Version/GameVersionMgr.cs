using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Piratera.GUI;

public class GameVersionMgr : MonoBehaviour
{
    private const string URL = "https://api1.piratera.io/v1/users/nonce?ethAddress=0x5480a34c99a78ab25a15667c3c64a07aec244cbe";
    private const string DOWNLOAD_SITE = "https://drive.google.com/drive/folders/18GS8ss4sj_NYBSNa2kpLby82WFFZpjWL?usp=sharing";

    void Start()
    {

        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        GuiManager.Instance.ShowGuiWaiting(true);

        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        GuiManager.Instance.ShowGuiWaiting(false);

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            PopupNewVersion popup = GuiManager.Instance.AddGui<PopupNewVersion>("Prefap/PopupNewVersion", LayerId.IMPORTANT).GetComponent<PopupNewVersion>();

            popup.SetData(() => Application.OpenURL(DOWNLOAD_SITE));
            // Show results as text
            Debug.Log(www.downloadHandler.text);
        }
    }
}