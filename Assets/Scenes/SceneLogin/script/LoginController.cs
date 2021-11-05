using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Logging;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;

public class LoginController : BaseController
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



	private class UserInforPropertiesKey
	{

		public const string UID = "uid";
		public const string USERNAME = "username";
		public const string BERI = "beri";
		public const string STAMINA = "stamina";
		public const string LAST_COUNT = "last_count";
		public const string EXP = "exp";
		public const string LEVEL = "level";
		public const string AVATAR = "avatar";
	}


	//----------------------------------------------------------
	// Unity callback methods
	//----------------------------------------------------------

	protected override void Awake()
	{
		Application.runInBackground = true;

		// Enable interface
		enableLoginUI(true);
	}

	//----------------------------------------------------------
	// Public interface methods for UI
	//----------------------------------------------------------

	public void OnLoginButtonClick()
	{
		enableLoginUI(false);

		// Set connection parameters
		ConfigData cfg = new ConfigData();
		cfg.Host = Host;
#if !UNITY_WEBGL
		cfg.Port = TcpPort;
#else
		cfg.Port = WSPort;
#endif
		cfg.Zone = Zone;

		// Initialize SFS2X client and add listeners
#if !UNITY_WEBGL
		sfs = new SmartFox();
#else
		sfs = new SmartFox(UseWebSocket.WS_BIN);
#endif

		sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
		sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
		sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnJoinRoomError);
		sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
		sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
		sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtentionResponse);

		// Connect to SFS2X
		sfs.Connect(cfg);
	}


    protected override void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {


		switch (action)
		{
			case SFSAction.JOIN_ZONE_SUCCESS:
				{
					if (errorCode == SFSErrorCode.SUCCESS)
                    {
						User user = sfs.MySelf;
						UserData.Instance.Avatar = user.GetVariable(UserInforPropertiesKey.AVATAR).GetStringValue();
						UserData.Instance.Username = user.Name;
						UserData.Instance.Beri = (long)user.GetVariable(UserInforPropertiesKey.BERI).GetDoubleValue();
						UserData.Instance.Stamina = user.GetVariable(UserInforPropertiesKey.STAMINA).GetIntValue();
						UserData.Instance.Exp = (long)user.GetVariable(UserInforPropertiesKey.EXP).GetDoubleValue();
						UserData.Instance.Level = user.GetVariable(UserInforPropertiesKey.LEVEL).GetIntValue();
						UserData.Instance.LastCountStamina = (long)user.GetVariable(UserInforPropertiesKey.LAST_COUNT).GetDoubleValue();
						OpenLobby();
					} else
                    {
						OnError("Error Code: " + errorCode);
                    }
					
					break;
				}
		}
	}

	private void OpenLobby()
    {
		reset();
		SceneManager.LoadScene("Scenes/SceneLobby/SceneLobby");
	}

    private void OnJoinRoom(BaseEvent evt)
    {
		
		// Load lobby scene
		//Application.LoadLevel("Lobby");
		Debug.Log("Join Room Success" + ((Room)evt.Params["room"]).Name);
		// SceneManager.LoadScene("Scenes/SceneLobby/SceneLobby");
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

	protected override void reset()
	{
		// Remove SFS2X listeners
		// This should be called when switching scenes, so events from the server do not trigger code in this scene
		sfs.RemoveAllEventListeners();

		// Enable interface
		enableLoginUI(true);
	}

	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------

	private void OnConnection(BaseEvent evt)
	{
		if ((bool)evt.Params["success"])
		{
			Debug.Log("SFS2X API version: " + sfs.Version);
			Debug.Log("Connection mode is: " + sfs.ConnectionMode);

			// Save reference to SmartFox instance; it will be used in the other scenes
			SmartFoxConnection.Connection = sfs;

			SFSObject sfso = SFSObject.NewInstance();
			sfso.PutUtfString("passwd", "test");
			sfso.PutInt("loginType", 1);
			sfs.Send(new Sfs2X.Requests.LoginRequest(nameInput.text, "", Zone, sfso));

			// Login
		}
		else
		{
			// Remove SFS2X listeners and re-enable interface
			reset();

			// Show error message
			errorText.text = "Connection failed; is the server running at all?";
		}
	}

	protected override void OnConnectionLost(BaseEvent evt)
	{
		// Remove SFS2X listeners and re-enable interface
		reset();

		string reason = (string)evt.Params["reason"];

		if (reason != ClientDisconnectionReason.MANUAL)
		{
			// Show error message
			errorText.text = "Connection was lost; reason is: " + reason;
		}
	}

	private void OnLogin(BaseEvent evt)
	{

		if (sfs.RoomList.Count > 0)
		{
			// sfs.Send(new Sfs2X.Requests.JoinRoomRequest("The Lobby"));
			Debug.Log("Request Join Room Lobby");
		}

		Debug.Log("Login success as " + sfs.MySelf.Name);
	}

	private void OnLoginError(BaseEvent evt)
	{		
		OnError("Login failed: " + (string)evt.Params["errorMessage"]);
	}

	private void OnJoinRoomError(BaseEvent evt)
	{
	
		OnError("Join Room failed: " + (string)evt.Params["errorMessage"]);
	}


	private void OnError(string message)
    {
		// Disconnect
		sfs.Disconnect();

		// Remove SFS2X listeners and re-enable interface
		reset();

		// Show error message
		errorText.text = message;
	}

}
