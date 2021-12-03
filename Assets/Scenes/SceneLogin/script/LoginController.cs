using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;

public class LoginController: MonoBehaviour
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
	private Text errorText;

	[SerializeField]
	private string signupLink = "https://piratera.io/";


	//----------------------------------------------------------
	// Unity callback methods
	//----------------------------------------------------------
	void Awake()
	{
		NetworkController.AddServerActionListener(OnReceiveServerAction);
		enableLoginUI(true);

		nameInput.text = PlayerPrefs.GetString("UserName");

	}

	private void OnConnection(BaseEvent evt)
	{
		if (!(bool)evt.Params["success"])
		{
			
			OnError("DNS Server Not Responding");

		}
	}

	private void OnLoginFail(BaseEvent evt)
    {
		OnError("Your username or password is incorrect");
		Debug.Log("Login failed: " + (string)evt.Params["errorMessage"]);
	}

	private void OnError(string error)
    {
		Reset();
		enableLoginUI(true);
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

	public void OnLoginButtonClick()
	{
		if (string.IsNullOrEmpty(nameInput.text))
        {
			errorText.text = "Username field is empty";
			return;

		}

		if (string.IsNullOrEmpty(passwordInput.text))
		{
			errorText.text = "Password field is empty";
			return;

		}

		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			errorText.text = "Error. Check Internet connection!";
			return;
		}
		enableLoginUI(false);
		NetworkController.LoginToServer(new LoginData(nameInput.text, passwordInput.text));
		NetworkController.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginFail);
		NetworkController.AddEventListener(SFSEvent.CONNECTION, OnConnection);
	}

	public void OnButtonCreateOneClick()
    {
		Application.OpenURL(signupLink);
	}

	public void ReceiveJoinZoneSuccess(SFSErrorCode errorCode, ISFSObject packet)
	{
		if (errorCode == SFSErrorCode.SUCCESS)
		{
			PlayerPrefs.SetString("UserName", nameInput.text);
			User user = NetworkController.Connection.MySelf;
			GameTimeMgr.SetLoginTime((long)user.GetVariable("login_time").GetDoubleValue());
			UserData.Instance.OnUserVariablesUpdate(user);
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
		nameInput.interactable = enable;
		loginButton.interactable = enable;
		errorText.text = "";
	}

    private void Reset()
    {
		NetworkController.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnLoginFail);
		NetworkController.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);
	}


}
