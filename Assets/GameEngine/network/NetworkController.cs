using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities; 
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Piratera.GUI;
using System;

namespace Piratera.Network
{

	public static class GAME_NETWORK_ADDRESS
    {
		public const string QC_HOST = "dev-game1.piratera.local";
		public const int QC_PORT = 9933;


		public const string DEV_HOST = "dev-game1.piratera.local";
		public const int DEV_PORT = 9933;
	}

	public delegate void NetworkActionListenerDelegate(SFSAction action, SFSErrorCode errorCode, ISFSObject packet);

	public class NetworkController : MonoBehaviour
	{
		//----------------------------------------------------------
		// Editor public properties
		//----------------------------------------------------------
		public static NetworkController Instance;

		private static List<NetworkActionListenerDelegate> serverActionListeners = new List<NetworkActionListenerDelegate>();

        internal static void AddServerActionListener(object onReceiveServerAction)
        {
            throw new NotImplementedException();
        }

#if PIRATERA_QC
		private static readonly string Host = GAME_NETWORK_ADDRESS.QC_HOST;
		private static readonly int TcpPort = GAME_NETWORK_ADDRESS.QC_PORT;
#elif PIRATERA_DEV
			private static readonly string Host = GAME_NETWORK_ADDRESS.DEV_HOST;
		private static readonly int TcpPort = GAME_NETWORK_ADDRESS.DEV_PORT;
#endif



		private static readonly int WSPort = 8080;



		private static readonly string Zone = "Piratera";
		private static readonly string CLIENT_REQUEST = "clrq";
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

			UserData.Instance.OnUserVariablesUpdate(user, changedVars);
			StaminaData.Instance.OnUserVariablesUpdate(user);
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

			Debug.Log("Connect to: " + cfg.Host + ":" + cfg.Port);

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
				sfso.PutUtfString("passwd", loginData.Password);
#if PIRATERA_DEV
				
				sfso.PutInt("loginType", (int)loginData.Type);
#elif PIRATERA_QC
                sfso.PutInt("loginType", (int)GameLoginType.DUMMY);
#else
                sfso.PutInt("loginType", (int)GameLoginType.AUTHENTICATON);
#endif
				sfso.PutUtfString("client_info", new LoginLogData().ToJson());
				sfs.Send(new LoginRequest(loginData.Username, "", Zone, sfso));
				Debug.Log("Send Login " + loginData.Username + "-" + loginData.Password + "-" + loginData.Type);
			}
			else Debug.Log("Connection failed; is the server running at all?");
		}

		protected static void OnConnectionLost(BaseEvent evt)
		{
			Debug.Log("Disconnect");
			reset();
			GuiManager.Instance.ShowGuiWaiting(false);
			if (shuttingDown == true)
			{
				return;
			}
			string reason = (string)evt.Params["reason"];

			if (reason != ClientDisconnectionReason.MANUAL)
			{
				string text = "Server Disconnected";
				if (reason == ClientDisconnectionReason.BAN)
				{
					text = "You are banned from Server";
				}

				if (reason == ClientDisconnectionReason.KICK)
				{
					text = "You have been kicked by Server";
				}

				GuiManager.Instance.ShowPopupNotification(text, ForceStartScene);
			}
			else
			{
				ForceStartScene();
			}


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

		public static void OnError(string message)
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
					GameUtils.ShowPopupPacketError(errorCode);
				}
				OnReceiveServerAction(action, errorCode, packet);
			}
		}
		public static void Send(SFSAction action, ISFSObject data)
		{

			if (sfs != null)
			{
				Debug.Log("Send Action To Server: " + action + $" ({(int)action})");
				data.PutInt(ACTION_INCORE, (int)action);
				ExtensionRequest extensionRequest = new ExtensionRequest(CLIENT_REQUEST, data);
				sfs.Send(extensionRequest);
			}
			else
			{
				GuiManager.Instance.ShowPopupNotification("Server Disconnected", ForceStartScene);
			}

		}
		public static void Send(SFSAction action)
		{
			Debug.Log(action.ToString());
			Send(action, new SFSObject());
		}
		protected static void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
		{
			if (errorCode != SFSErrorCode.SUCCESS)
			{
				Debug.LogWarning($"Packet {action} Fail, Error Code: {errorCode}");
			}
			foreach (NetworkActionListenerDelegate listener in new List<NetworkActionListenerDelegate>(serverActionListeners))
			{
				listener(action, errorCode, packet);
			}


			switch (action)
			{

				case SFSAction.COMBAT_BOT:
					{
						if (errorCode == SFSErrorCode.SUCCESS)
						{
							TempCombatData.Instance.LoadCombatDataFromSfs(packet);

							SceneManager.LoadScene("SceneCombat2D");
						}
						break;
					}
				case SFSAction.COMBAT_DATA:
					{
						if (errorCode == SFSErrorCode.SUCCESS)
						{
							TempCombatData.Instance.LoadCombatDataFromSfs(packet);
							SceneManager.LoadScene("SceneCombat2D");
						}
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

				case SFSAction.COMBAT_PREPARE:
					{
						if (errorCode == SFSErrorCode.SUCCESS)
						{
							TeamCombatPrepareData.Instance.NewFromSFSObject(packet);
							SceneManager.LoadScene("ScenePickTeamBattle");
						}
						break;
					}
				case SFSAction.PVE_PLAY:
					{
						if (errorCode == SFSErrorCode.SUCCESS)
						{
							TeamCombatPrepareData.Instance.NewFromSFSObject(packet);
							SceneManager.LoadScene("ScenePickTeamBattle");
						}
						break;
					}
				case SFSAction.PVE_SURRENDER:
					{
						if (errorCode == SFSErrorCode.SUCCESS)
						{
							SceneManager.LoadScene("SceneLobby");
						}
						break;
					}
				case SFSAction.GET_STAMINA_PACK:
					{
						GuiManager.Instance.ShowGuiWaiting(false);
						if (errorCode == SFSErrorCode.SUCCESS)
						{
							GameObject GO = GuiManager.Instance.AddGui<GuiBuyStamina>("Prefap/GuiBuyStamina", LayerId.GUI);
							GuiBuyStamina popup = GO.GetComponent<GuiBuyStamina>();
							popup.InitPackData(packet.GetLong("cost"), packet.GetLong("quantity"));
						}
						break;
					}
				case SFSAction.GET_LINEUP_SLOT_PACK:
					{
						GuiManager.Instance.ShowGuiWaiting(false);
						if (errorCode == SFSErrorCode.SUCCESS)
						{
							GameObject GO = GuiManager.Instance.AddGui<GuiBuySlot>("Prefap/GuiBuySlot", LayerId.GUI);
							GuiBuySlot popup = GO.GetComponent<GuiBuySlot>();
							popup.InitPackData(packet.GetLong("cost"));
						}
						break;
					}
				case SFSAction.CHEAT_RANK:
					{
						if (errorCode == SFSErrorCode.SUCCESS)
						{
							GuiManager.Instance.ShowPopupNotification($"Cheat Rank Success, Rank {packet.GetInt("rank")}");
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
			}
			else
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
}
