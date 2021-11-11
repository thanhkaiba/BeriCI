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
using Sfs2X.Requests;

public class LoginData {
	public LoginData(string username, string password)
	{
		this.username = username;
		this.password = password;
	}
	public string username = "";
	public string password = "";
};

public class NetworkController : MonoBehaviour
{
	public static NetworkController Instance;
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

	public const string CLIENT_REQUEST = "clrq";
	public static string ACTION_INCORE = "acc";
	public static string ERROR_CODE = "error_code";
	//----------------------------------------------------------
	// Private properties
	//----------------------------------------------------------

	protected SmartFox sfs;
	private LoginData loginData;
	private bool shuttingDown;

	//----------------------------------------------------------
	// Unity callback methods
	//----------------------------------------------------------

	protected virtual void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);

			Application.runInBackground = true;
			SetupSFS();
			//ForceStartScene();
		}
		else Destroy(gameObject);
	}
	private void SetupSFS()
	{
		// Initialize SFS2X client and add listeners
		#if !UNITY_WEBGL
			sfs = new SmartFox();
		#else
			sfs = new SmartFox(UseWebSocket.WS_BIN);
		#endif
		// Register event listeners
		sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
		sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
		//sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
		sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnJoinRoomError);
		sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtentionResponse);

	}
	protected virtual void Update()
	{
		if (sfs != null) sfs.ProcessEvents();
	}
	void OnApplicationQuit()
	{
		shuttingDown = true;
	}
	private void ForceStartScene()
	{
		if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("SceneLogin")
			&& SceneManager.GetActiveScene() != SceneManager.GetSceneByName("SceneLoading"))
		{
			SceneManager.LoadScene("Scenes/SceneLogin/SceneLogin");
		}
	}
	//----------------------------------------------------------
	// Private helper methods
	//----------------------------------------------------------
	public void LoginToServer(LoginData data)
	{
		loginData = data;

		ConfigData cfg = new ConfigData();
		cfg.Host = Host;
		#if !UNITY_WEBGL
			cfg.Port = TcpPort;
		#else
			cfg.Port = WSPort;
		#endif
		cfg.Zone = Zone;

		sfs.Connect(cfg);
	}
	public void Logout()
	{
		sfs.Disconnect();
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

			SFSObject sfso = SFSObject.NewInstance();
			sfso.PutUtfString("passwd", "test");
			sfso.PutInt("loginType", 1);
			sfs.Send(new Sfs2X.Requests.LoginRequest(loginData.username, "", Zone, sfso));
			Debug.Log("Send Login");
		}
		else Debug.Log("Connection failed; is the server running at all?");
	}

	protected void OnConnectionLost(BaseEvent evt)
	{
		string reason = (string)evt.Params["reason"];

		if (reason != ClientDisconnectionReason.MANUAL)
		{
			// Show error message
			//errorText.text = "Connection was lost; reason is: " + reason;
		}

		// do something (popup disconnect,...)
		if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("SceneLogin"))
		{
			SceneManager.LoadScene("Scenes/SceneLogin/SceneLogin");
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
	private void OnJoinRoom(BaseEvent evt)
	{
		//Application.LoadLevel("Lobby");
		Debug.Log("Join Room Success" + ((Room)evt.Params["room"]).Name);
		// SceneManager.LoadScene("Scenes/SceneLobby/SceneLobby");
	}
	private void OnJoinRoomError(BaseEvent evt)
	{
		OnError("Join Room failed: " + (string)evt.Params["errorMessage"]);
	}
	public void OnError(string message)
	{
		Debug.Log("Network Error: " + message);
		sfs.Disconnect();
	}

	protected virtual void OnExtentionResponse(BaseEvent evt)
	{

		ISFSObject packet = (ISFSObject)evt.Params["params"];

		string cmd = (string)evt.Params["cmd"];
		if (cmd == CLIENT_REQUEST)
		{
			Debug.Log("response:" + packet.GetDump());

			SFSAction action = (SFSAction)packet.GetInt(ACTION_INCORE);
			SFSErrorCode errorCode = (SFSErrorCode)packet.GetByte(ERROR_CODE);

			if (errorCode != SFSErrorCode.SUCCESS)
			{
				Debug.LogError($"Packet {action} Fail, Error Code: {errorCode}");
			}
			OnReceiveServerAction(action, errorCode, packet);
		}
	}
	public void Send(SFSAction action, ISFSObject data)
	{
		data.PutInt(ACTION_INCORE, (int)action);
		ExtensionRequest extensionRequest = new ExtensionRequest(CLIENT_REQUEST, data);
		sfs.Send(extensionRequest);
	}
	public void Send(SFSAction action)
	{
		Send(action, new SFSObject());
	}
	protected virtual void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
	{
		if (LobbyController.Instance) LobbyController.Instance.OnReceiveServerAction(action, errorCode, packet);
		if (LoginController.Instance) LoginController.Instance.OnReceiveServerAction(action, errorCode, packet);
		switch (action)
		{
			case SFSAction.JOIN_ZONE_SUCCESS:
				{
					if (LoginController.Instance) LoginController.Instance.ReceiveJoinZoneSuccess(sfs, errorCode, packet);
					break;
				}
			case SFSAction.COMBAT_BOT:
				{
					TempCombatData.Instance.LoadCombatDataFromSfs(packet);
					SceneManager.LoadScene("SceneCombat2D");
					break;
				}
		}
	}

}
