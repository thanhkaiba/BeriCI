using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Sfs2X;
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

public delegate void NetworkActionListenerDelegate(SFSAction action, SFSErrorCode errorCode, ISFSObject packet);

public class NetworkController : MonoBehaviour
{
	//----------------------------------------------------------
	// Editor public properties
	//----------------------------------------------------------
	public static NetworkController Instance;

	private static List<NetworkActionListenerDelegate> serverActionListeners = new List<NetworkActionListenerDelegate>();
	private static readonly string Host = "127.0.0.1";

	private static readonly int TcpPort = 9933;

	private static readonly int WSPort = 8080;

	[Tooltip("Name of the SmartFoxServer 2X Zone to join")]
	private static readonly string Zone = "Piratera";

	private static readonly string   CLIENT_REQUEST = "clrq";
	private static readonly string ACTION_INCORE = "acc";
	private static readonly string ERROR_CODE = "error_code";

	public static SmartFox Connection
	{
		get
		{
			return sfs;
		}
		
	}
	//----------------------------------------------------------
	// Private properties
	//----------------------------------------------------------

	private static SmartFox sfs;
	private static LoginData loginData;
	private static bool shuttingDown;

	//----------------------------------------------------------
	// Unity callback methods
	//----------------------------------------------------------


	void Awake()
    {
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			Application.runInBackground = true;
			ForceStartScene();
		}
		else Destroy(gameObject);
	

	}

	private static void reset()
	{
		// Remove SFS2X listeners
		sfs.RemoveAllEventListeners();
		sfs = null;
	}

    private static void OnUserDataUpdate(BaseEvent evt)
    {
		List<string> changedVars = (List<string>)evt.Params["changedVars"];

		SFSUser user = (SFSUser)evt.Params["user"];

		if (changedVars.Contains("stamina"))
		{
			Debug.Log("stamina:" + user.GetVariable("stamina").GetIntValue());
		}

		if (changedVars.Contains("last_count"))
		{
			Debug.Log("last_count: " + user.GetVariable("last_count").GetDoubleValue());
		}



		if (changedVars.Contains("login_time"))
		{
			Debug.Log("login_time:" + user.GetVariable("login_time").GetDoubleValue());
		}
		UserData.Instance.OnUserVariablesUpdate(user);
	}

	public static bool IsInitialized
	{
		get
		{
			return (sfs != null);
		}
	}

	void Update()
	{
		if (sfs != null) sfs.ProcessEvents();
	}
	void OnApplicationQuit()
	{
		shuttingDown = true;
		if (sfs != null && sfs.IsConnected)
		{
			sfs.Disconnect();
		}
	}
	private static void ForceStartScene()
	{

		if (SceneManager.GetActiveScene().name != "SceneLogin" && SceneManager.GetActiveScene().name != "SceneLoading")
		{
			SceneManager.LoadScene("SceneLogin");
		}

	}
	//----------------------------------------------------------
	// Private helper methods
	//----------------------------------------------------------
	public static void LoginToServer(LoginData data)
	{
		loginData = data;
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
		// Register event listeners
		AddEventListener(SFSEvent.CONNECTION, OnConnection);
		AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		AddEventListener(SFSEvent.LOGIN, OnLogin);
		AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
		AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtentionResponse);
		AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserDataUpdate);

		sfs.Connect(cfg);
	}
	public static void Logout()
	{
		if (sfs != null)
        {
			sfs.Disconnect();
		}
	}
	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------
	private static void OnConnection(BaseEvent evt)
	{
		if ((bool)evt.Params["success"])
		{
			Debug.Log("SFS2X API version: " + sfs.Version);
			Debug.Log("Connection mode is: " + sfs.ConnectionMode);

			SFSObject sfso = new SFSObject();
			sfso.PutUtfString("passwd", "test");
			sfso.PutInt("loginType", 1);
			sfs.Send(new LoginRequest(loginData.username, "", Zone, sfso));
			Debug.Log("Send Login");
		}
		else Debug.Log("Connection failed; is the server running at all?");
	}

	protected static void OnConnectionLost(BaseEvent evt)
	{
	
	
		reset();
		if (shuttingDown == true)
        {
			return;
		}
		string reason = (string)evt.Params["reason"];

		if (reason != ClientDisconnectionReason.MANUAL)
		{
			// Show error message
			//errorText.text = "Connection was lost; reason is: " + reason;
		}

		// do something (popup disconnect,...)
		ForceStartScene();
	}

	private static void OnLogin(BaseEvent evt)
	{
		if (sfs.RoomList.Count > 0)
		{
			// sfs.Send(new Sfs2X.Requests.JoinRoomRequest("The Lobby"));
			Debug.Log("Request Join Room Lobby");
		}

	
		Debug.Log("Login success as " + sfs.MySelf.Name);
	}

	private static void OnLoginError(BaseEvent evt)
	{
		string errorText = "Login failed: " + (string)evt.Params["errorMessage"];
		OnError(errorText);
	}

	public static void  OnError(string message)
	{
		Debug.Log("Network Error: " + message);
		sfs.Disconnect();
	}

	protected static void OnExtentionResponse(BaseEvent evt)
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
	public static void Send(SFSAction action, ISFSObject data)
	{
		data.PutInt(ACTION_INCORE, (int)action);
		ExtensionRequest extensionRequest = new ExtensionRequest(CLIENT_REQUEST, data);
		sfs.Send(extensionRequest);
	}
	public static void Send(SFSAction action)
	{
		Send(action, new SFSObject());
	}
	protected static void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
	{
		foreach (NetworkActionListenerDelegate listener in serverActionListeners) {
			listener(action, errorCode, packet);
        }
		switch (action)
		{
		
			case SFSAction.COMBAT_BOT:
				{
					TempCombatData.Instance.LoadCombatDataFromSfs(packet);
					SceneManager.LoadScene("SceneCombat2D");
					break;
				}
			case SFSAction.LOAD_LIST_HERO_INFO:
				{
					if (errorCode == SFSErrorCode.SUCCESS)
					{
						Debug.Log("Load List hero success");
						CrewData.Instance.NewFromSFSObject(packet);
					}
					break;
				}
		}
	}

	public static void AddServerActionListener(NetworkActionListenerDelegate listener)
    {
		serverActionListeners.Add(listener);

	}

	public static void RemoveServerActionListener(NetworkActionListenerDelegate listener)
	{
		serverActionListeners.Remove(listener);

	}


	//
	// Summary:
	//     Adds a delegate to a given API event type that will be used for callbacks.
	//
	// Parameters:
	//   eventType:
	//     The name of the Sfs2X.Core.SFSEvent to get callbacks on.
	//
	//   listener:
	//     The delegate method to register.
	public static void AddEventListener(string eventType, EventListenerDelegate listener)
    {
		if (sfs != null)
        {
			sfs.AddEventListener(eventType, listener);
        } else
        {
			Debug.LogError("Smart Fox Connection is NULL");
        }
    }

	//
	// Summary:
	//     Removes a delegate registration for a given API event.
	//
	// Parameters:
	//   eventType:
	//     The SFSEvent to remove callbacks on.
	//
	//   listener:
	//     The delegate method to unregister.
	public static void RemoveEventListener(string eventType, EventListenerDelegate listener)
    {
		if (sfs != null)
		{
			sfs.RemoveEventListener(eventType, listener);
		}
	}

}
