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
	// Editor public properties
	//----------------------------------------------------------

	[Tooltip("IP address or domain name of the SmartFoxServer 2X instance")]
	public string Host = "127.0.0.1";

	[Tooltip("TCP port listened by the SmartFoxServer 2X instance; used for regular socket connection in all builds except WebGL")]
	public int TcpPort = 9933;

	[Tooltip("WebSocket port listened by the SmartFoxServer 2X instance; used for in WebGL build only")]
	public int WSPort = 8080;

	[Tooltip("Name of the SmartFoxServer 2X Zone to join")]
	public string Zone = "Piratera";

	//----------------------------------------------------------
	// UI elements
	//----------------------------------------------------------

	[SerializeField]
	private InputField nameInput;
	[SerializeField]
	private Button loginButton;
	[SerializeField]
	private Text errorText;


	//----------------------------------------------------------
	// Unity callback methods
	//----------------------------------------------------------
	void Awake()
	{
		NetworkController.AddServerActionListener(OnReceiveServerAction);
		enableLoginUI(true);
	}

	private void OnConnection(BaseEvent evt)
	{
		if (!(bool)evt.Params["success"])
		{
			
			OnError("Connection failed; is the server running at all?");

		}
	}

	private void OnLoginFail(BaseEvent evt)
    {
		OnError("Login failed: " + (string)evt.Params["errorMessage"]);
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
		enableLoginUI(false);
		NetworkController.LoginToServer(new LoginData(nameInput.text, ""));
		NetworkController.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginFail);
		NetworkController.AddEventListener(SFSEvent.CONNECTION, OnConnection);
	}
	public void ReceiveJoinZoneSuccess(SFSErrorCode errorCode, ISFSObject packet)
	{
		if (errorCode == SFSErrorCode.SUCCESS)
		{
			User user = NetworkController.Connection.MySelf;
			UserData.Instance.OnUserVariablesUpdate(user);
			OpenLobby();
		}
		else
		{
			errorText.text = "Login error: code " + errorCode;
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
		SceneManager.LoadScene("SceneLobby");
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
