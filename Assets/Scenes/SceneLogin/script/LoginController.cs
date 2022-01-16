using Piratera.Constance;
using Piratera.GUI;
using Piratera.Lib;
using Piratera.Network;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Util;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{



    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------

    [SerializeField]
    private InputField nameInput;
    [SerializeField]
    private InputField passwordInput;
    [SerializeField]
    private Button loginButton;
    [SerializeField]
    private Button signupButton;
    [SerializeField]
    private Text signupText;
    [SerializeField]
    private Text errorText;
    [SerializeField]
    private Toggle loginTypeToggle;




    //----------------------------------------------------------
    // Unity callback methods
    //----------------------------------------------------------
    void Awake()
    {
        NetworkController.AddServerActionListener(OnReceiveServerAction);
        enableLoginUI(true);

        nameInput.text = PlayerPrefs.GetString("UserName");

#if PIRATERA_DEV || PIRATERA_QC
        loginTypeToggle.gameObject.SetActive(true);
        loginTypeToggle.isOn = PlayerPrefs.GetInt("loginTypeToggle", 1) == 1;
        signupText.text = "Guest";

#else
         
        loginTypeToggle.isOn = false;
		loginTypeToggle.gameObject.SetActive(false);
#endif

        signupText.text = "Create One";
#if PIRATERA_QC || PIRATERA_DEV || (PIRATERA_LIVE && UNITY_ANDROID)
        signupText.text = "Guest";
#endif

    }

    private void OnConnection(BaseEvent evt)
    {
        if (!(bool)evt.Params["success"])
        {

            OnError("Server Not Responding");

        }
    }

    private void OnLoginFail(BaseEvent evt)
    {
        string description = "There was a problem.";
        if (evt.Params.ContainsKey("errorCode"))
        {
            int err = int.Parse(evt.Params["errorCode"].ToString());
            description = EnumHelper.GetDescription((SFSErrorCode)err);
        }
        OnError(description);
    }

    private void OnError(string error)
    {
        Reset();
        enableLoginUI(true);
        GuiManager.Instance.ShowGuiWaiting(false);
        errorText.text = error;

    }

    void OnDestroy()
    {
        NetworkController.RemoveServerActionListener(OnReceiveServerAction);
        Reset();
    }

    //----------------------------------------------------------
    // Public interface methods for UI
    //----------------------------------------------------------
    public IEnumerator CheckInternetConnection(Action<bool> syncResult)
    {
        GuiManager.Instance.ShowGuiWaiting(true);
        const string echoServer = "http://google.com";
        bool result;
        using (var request = UnityWebRequest.Head(echoServer))
        {
            request.timeout = 5;
            yield return request.SendWebRequest();
            result = (request.result == UnityWebRequest.Result.Success) && request.responseCode == 200;
        }
        syncResult(result);
    }
    public void OnLoginButtonClick()
    {
        if (string.IsNullOrEmpty(nameInput.text) && string.IsNullOrEmpty(passwordInput.text))
        {
            errorText.text = "Please enter your email and password";
            return;
        }
        if (string.IsNullOrEmpty(nameInput.text))
        {
            errorText.text = "Email field is empty";
            return;

        }

        if (string.IsNullOrEmpty(passwordInput.text))
        {
            errorText.text = "Password field is empty";
            return;

        }

#if PIRATERA_DEV || PIRATERA_QC
        PlayerPrefs.SetInt("loginTypeToggle", loginTypeToggle.isOn ? 1 : 0);
#endif

#if PIRATERA_LIVE
        loginTypeToggle.isOn = false;
#endif

        SendRequestLogin(nameInput.text, passwordInput.text, loginTypeToggle.isOn ? GameLoginType.DUMMY : GameLoginType.AUTHENTICATON);
    }

    private void SendRequestLogin(string username, string password, GameLoginType loginType)
    {
        StartCoroutine(CheckInternetConnection(isConnected =>
        {
            if (isConnected)
            {
                enableLoginUI(false);
                NetworkController.LoginToServer(new LoginData(username, password, loginType));
                NetworkController.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginFail);
                NetworkController.AddEventListener(SFSEvent.CONNECTION, OnConnection);
                NetworkController.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
                NetworkController.AddEventListener(SFSEvent.CRYPTO_INIT, OnCryptoInit);
            }
            else
            {
                GuiManager.Instance.ShowGuiWaiting(false);
                errorText.text = "Error. Check Internet connection!";
            }
        }));
    }

    private void OnCryptoInit(BaseEvent evt)
    {
      
        if (!(bool)evt.Params["success"])
        {
            // Send a login request
            OnError((string)evt.Params["errorMessage"]);
        }
     
        
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        string reason = (string)evt.Params["reason"];
        if (reason != ClientDisconnectionReason.MANUAL)
        {
            enableLoginUI(true);
        }

    }

    public void OnButtonCreateOneClick()
    {
#if PIRATERA_QC || PIRATERA_DEV || (PIRATERA_LIVE && UNITY_ANDROID)
        SendRequestLogin(SystemInfo.deviceUniqueIdentifier, "guest", GameLoginType.DUMMY);
#else
        Application.OpenURL(GameConst.ACCOUNT_URL);
#endif
    }

    public void ReceiveJoinZoneSuccess(SFSErrorCode errorCode, ISFSObject packet)
    {
        if (errorCode == SFSErrorCode.SUCCESS)
        {
            PlayerPrefs.SetString("UserName", nameInput.text);
            User user = NetworkController.Connection.MySelf;
            GameTimeMgr.SetServerTime((long)user.GetVariable("login_time").GetDoubleValue());
            UserData.Instance.OnUserVariablesUpdate(user);
            StaminaData.Instance.OnUserVariablesUpdate(user);
            OpenLobby();
        }
        else
        {
            errorText.text = "There was a problem. \nError code: " + errorCode;
        }
    }

    public void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        switch (action)
        {
            case SFSAction.JOIN_ZONE_SUCCESS:
                {
                    ReceiveJoinZoneSuccess(errorCode, packet);
                    break;
                }

        }
    }

   
    private void OpenLobby()
    {
        SceneManager.LoadScene("SceneLoadServerData");
    }
    //----------------------------------------------------------
    // Private helper methods
    //----------------------------------------------------------
    private void enableLoginUI(bool enable)
    {
        /*nameInput.interactable = enable;
		passwordInput.interactable = enable;
		loginButton.interactable = enable;
		signupButton.interactable = enable;*/
        errorText.text = "";
    }

    private void Reset()
    {
        NetworkController.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnLoginFail);
        NetworkController.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);
        NetworkController.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        NetworkController.RemoveEventListener(SFSEvent.CRYPTO_INIT, OnCryptoInit);
    }


}
