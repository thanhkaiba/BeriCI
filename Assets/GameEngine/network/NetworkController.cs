using Facebook.Unity;
using Piratera.Config;
using Piratera.Engine;
using Piratera.GUI;
using Piratera.Log;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Piratera.Network
{

    public static class GAME_NETWORK_ADDRESS
    {
        public const string DEV_HOST = "192.168.102.2";
        public const int DEV_PORT = 9933;

        public const string QC_HOST = "dev-game.piratera.io";
        public const int QC_PORT = 9933;

        public const string STAGING_HOST = "dev-game2.piratera.io";
        public const int STAGING_PORT = 9933;

        public static string PROD_HOST = "game.piratera.io";
        public static int PROD_PORT = 9933;
    }

    public delegate void NetworkActionListenerDelegate(SFSAction action, SFSErrorCode errorCode, ISFSObject packet);

    public class NetworkController : MonoBehaviour
    {
        //----------------------------------------------------------
        // Editor public properties
        //----------------------------------------------------------
        public static NetworkController Instance;

        private static List<NetworkActionListenerDelegate> serverActionListeners = new();

        internal static void AddServerActionListener(object onReceiveServerAction)
        {
            throw new NotImplementedException();
        }
        private static readonly int WSPort = 8443;
        private static readonly string Zone = "Piratera";
        private static readonly string CLIENT_REQUEST = "clrq";
        private static readonly string ACTION_INCORE = "acc";
        private static readonly string ERROR_CODE = "error_code";
        private static readonly string MAINTAINANCE_NOTI = "maintenance_noti";
        private static bool needCheck = false;

        private static string adminMessage;
        public static bool AutoLogin = true;

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
            // Instance.CancelInvoke("GetServerTime");
            UserData.Instance.Reset();
            sfs.RemoveAllEventListeners();
            sfs = null;
            needCheck = false;
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
            if (sfs != null && sfs.IsConnected)
            {
                sfs.Disconnect();
            }
        }
        private static void ForceStartScene()
        {
            if (SceneManager.GetActiveScene().name != "SceneLoading")
            {
                SceneManager.LoadScene("SceneLoading");
            }
        }
        public static void RunSceneLogin()
        {
            if (SceneManager.GetActiveScene().name != "SceneLogin")
                SceneManager.LoadScene("SceneLogin");
        }
        //----------------------------------------------------------
        // Private helper methods
        //----------------------------------------------------------
        public static void LoginToServer(LoginData data)
        {
#if PIRATERA_QC
        string Host = GAME_NETWORK_ADDRESS.QC_HOST;
        int TcpPort = GAME_NETWORK_ADDRESS.QC_PORT;
#elif PIRATERA_DEV
		string Host = GAME_NETWORK_ADDRESS.DEV_HOST;
		int TcpPort = GAME_NETWORK_ADDRESS.DEV_PORT;
#elif PIRATERA_LIVE
        string Host = GAME_NETWORK_ADDRESS.PROD_HOST;
		int TcpPort = GAME_NETWORK_ADDRESS.PROD_PORT;
#else
        
#endif
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


            Debug.Log("Host: " + Host);
            Debug.Log("TcpPort: " + TcpPort);

            Debug.Log("GAME_NETWORK_ADDRESS.PROD_HOST: " + GAME_NETWORK_ADDRESS.PROD_HOST);
            Debug.Log("GAME_NETWORK_ADDRESS.PROD_PORT: " + GAME_NETWORK_ADDRESS.PROD_PORT);
            // Initialize SFS2X client and add listeners
#if !UNITY_WEBGL
            sfs = new SmartFox();
#else
			sfs = new SmartFox(UseWebSocket.WSS_BIN);
#endif
            // Register event listeners
            AddEventListener(SFSEvent.CONNECTION, OnConnection);
            AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
            AddEventListener(SFSEvent.LOGIN, OnLogin);
            AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
            AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtentionResponse);
            AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserDataUpdate);
            AddEventListener(SFSEvent.MODERATOR_MESSAGE, OnModMessage);
            AddEventListener(SFSEvent.ADMIN_MESSAGE, OnModMessage);
            AddEventListener(SFSEvent.SOCKET_ERROR, OnSocketError);
            AddEventListener(SFSEvent.CRYPTO_INIT, OnCryptoInit);


            MaintainManager.ResetData();
            GameConfigSync.ResetData();
            Debug.Log("Connect to: " + cfg.Host + ":" + cfg.Port);


            sfs.Connect(cfg);

        }
        // Handle encryption initialization event
        private static void OnCryptoInit(BaseEvent evt)
        {
            Debug.Log("OnCryptoInit: ");
            if ((bool)evt.Params["success"])
            {
                // Send a login request
                DoLogin();
            }
            else
            {
                LogServiceManager.Instance.SendLog(LogEvent.OPEN_GAME, evt.Params["errorMessage"].ToString());
                Debug.Log(evt.Params["errorMessage"].ToString());
            }
        }

        private static void OnInfoLogMessage(BaseEvent evt)
        {
            string message = (string)evt.Params["message"];
            Debug.Log("[SFS2X INFO] " + message);                           // .Net / Unity

        }

        private static void OnSocketError(BaseEvent evt)
        {
            Debug.Log("On Socket Error " + (string)evt.Params["message"]);
        }

        private static void OnModMessage(BaseEvent evt)
        {
            string message = (string)evt.Params["message"];
            Debug.Log("Admin message: " + message);

            adminMessage = message;
            if (message == "already login")
            {
                adminMessage = "Same account launched from different device";
            }
        }

        public static void Logout()
        {
            if (sfs != null)
            {
                sfs.Disconnect();
            }
            RunSceneLogin();
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
#if PIRATERA_QC || PIRATERA_DEV || UNITY_WEBGL
                DoLogin();
#else
                new CustomCryptoInitializerV2(sfs).Run();
#endif

            }
            else Debug.Log("Connection failed; is the server running at all?");
        }

        // Send a login request
        private static void DoLogin()
        {


            SFSObject sfso = new SFSObject();
            sfso.PutUtfString("passwd", loginData.Password);
            sfso.PutUtfString("client_info", new LoginLogData().ToJson());
            sfso.PutInt("loginType", (int)loginData.Type);
            sfs.Send(new LoginRequest(loginData.Username, "", Zone, sfso));
#if PIRATERA_DEV || PIRATERA_QC
            Debug.Log("Send Login " + loginData.Username + "-" + loginData.Password + "-" + loginData.Type);
#endif
        }

        private static void ShowDisconnect(string reason)
        {
            Debug.Log("Disconnect");
            reset();
            SceneTransition.Instance.ShowWaiting(false);
            if (reason != ClientDisconnectionReason.MANUAL)
            {
                string text = "Server Disconnected";
                if (reason == ClientDisconnectionReason.BAN)
                {
                    text = "You are banned from Server";
                }

                if (reason == ClientDisconnectionReason.KICK)
                {
                    if (string.IsNullOrEmpty(adminMessage))
                    {
                        text = "You have been kicked by Server";

                    }
                    else
                    {
                        text = adminMessage;
                        adminMessage = "";
                    }

                }

                if (reason == ClientDisconnectionReason.UNKNOWN)
                {
                    if (!string.IsNullOrEmpty(adminMessage))
                    {
                        text = adminMessage;
                        adminMessage = "";
                    }
                }


                GuiManager.Instance.ShowPopupNotification(text, RunSceneLogin);
            }
            else
            {
                RunSceneLogin();
            }
        }

        protected static void OnConnectionLost(BaseEvent evt)
        {
        
         
            string reason = (string)evt.Params["reason"];

            ShowDisconnect(reason);


        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                if (sfs != null && !sfs.IsConnected && needCheck)
                {
                    ShowDisconnect(ClientDisconnectionReason.MANUAL);
                }

            } else
            {
                if (sfs != null && sfs.IsConnected)
                {
                    needCheck = true;
                }
            }
          
        }
        public static void SendSurrenderPVEToSever()
        {
            SceneTransition.Instance.ShowWaiting(true);
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutBool("accept", false);
            Send(SFSAction.PVE_SURRENDER, sfsObject);
        }
        private static void OnLogin(BaseEvent evt)
        {
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
                SFSErrorCode errorCode = (SFSErrorCode)packet.GetShort(ERROR_CODE);
                if (errorCode != SFSErrorCode.SUCCESS)
                {
                    GameUtils.ShowPopupPacketError(errorCode, action);
                }
                OnReceiveServerAction(action, errorCode, packet);
            }
            else if (cmd == MAINTAINANCE_NOTI)
            {
                long startTime = packet.GetLong("startMaintainTime");
                long endTime = packet.GetLong("endMaintainTime");
                string message = packet.GetUtfString("note");

                Debug.Log("maintain " + startTime + " " + endTime + " " + message);
                MaintainManager.OnReceiveMaintainInfo(startTime, endTime, message);
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
                GuiManager.Instance.ShowPopupNotification("Server Disconnected", RunSceneLogin);
            }
        }
        public static void Send(SFSAction action)
        {
            Debug.Log(action.ToString());
            Send(action, new SFSObject());
        }
        protected static void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
        {

            switch (action)
            {

                case SFSAction.COMBAT_BOT:
                    {
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            TempCombatData.Instance.LoadCombatDataFromSfs(packet);

                            SceneTransition.Instance.LoadScene("SceneCombat2D", TransitionType.BATTLE);
                        }
                        break;
                    }
                case SFSAction.COMBAT_DATA:
                case SFSAction.PVP_CONFIRM:
                    {
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            TempCombatData.Instance.LoadCombatDataFromSfs(packet);
                            SceneTransition.Instance.LoadScene("SceneCombat2D", TransitionType.BATTLE);
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
                case SFSAction.PVP_COMBAT_PREPARE:
                    {
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            TeamPvPCombatPrepareData.Instance.NewFromSFSObject(packet);
                            SceneManager.LoadScene("ScenePreparePvP");
                            SceneTransition.Instance.ShowWaiting(false);
                        }
                        break;
                    }
                case SFSAction.PVP_PLAY:
                    {
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            PvPData.Instance.Ticket = packet.GetInt("ticket");
                        }
                        break;
                    }
                case SFSAction.GET_STAMINA_PACK:
                    {
                        SceneTransition.Instance.ShowWaiting(false);
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            GameObject GO = GuiManager.Instance.AddGui("Prefap/GuiBuyStamina");
                            GuiBuyStamina popup = GO.GetComponent<GuiBuyStamina>();
                            popup.InitPackData(packet.GetLong("cost"), packet.GetLong("quantity"));
                        }
                        break;
                    }
                case SFSAction.PIRATE_WHEEL_DATA:
                    {
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            PirateWheelData.Instance.NewFromSFSObject(packet);
                        }
                        break;
                    }
                case SFSAction.PIRATE_WHEEL:
                    {
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            PirateWheelData.Instance.ReceiveGiftPack(packet);
                        }
                        break;
                    }
                case SFSAction.GET_LINEUP_SLOT_PACK:
                    {
                        SceneTransition.Instance.ShowWaiting(false);
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            GameObject GO = GuiManager.Instance.AddGui("Prefap/GuiBuySlot");
                            GuiBuySlot popup = GO.GetComponent<GuiBuySlot>();
                            popup.InitPackData(packet.GetLong("cost"));
                        }
                        break;
                    }
#if PIRATERA_QC || PIRATERA_DEV
                case SFSAction.CHEAT_RANK:
                    {
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            GuiManager.Instance.ShowPopupNotification($"Cheat Rank Success, Rank {packet.GetInt("rank")}");
                        }
                        break;
                    }
#endif
                case SFSAction.GET_SERVER_TIME:
                    {
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            GameTimeMgr.SetServerTime(packet.GetLong("time"));
                            // Instance.Invoke("GetServerTime", 10f);
                        }
                        break;
                    }
                case SFSAction.JOIN_ZONE_SUCCESS:
                    {
                        if (Instance != null)
                        {
                            Instance.GetServerTime();
                        }
                        break;
                    }
                case SFSAction.PVP_DATA:
                    {
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            PvPData.Instance.NewFromSFSObject(packet);
                        }
                        break;
                    }
                case SFSAction.PVP_WATCH_HISTORY:
                    {
                        SceneTransition.Instance.ShowWaiting(false);
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            string json = packet.GetUtfString("combat_data");
                            TempCombatData.Instance.LoadCombatDataFromSfs(SFSObject.NewFromJsonData(json));
                            TempCombatData.Instance.isReplayMatch = true;
                            SceneManager.LoadScene("SceneCombat2D");
                        }
                        break;
                    }
                case SFSAction.TRAIN_SAILORS_REMAIN:
                    {
                        SceneTransition.Instance.ShowWaiting(false);
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            UserData.Instance.TrainedToday = packet.GetIntArray("trained_today");
                            for (int i = 0; i < UserData.Instance.TrainedToday.Length; i++)
                            {
                                Debug.Log("81: " + UserData.Instance.TrainedToday[i]);
                            }
                        }
                        break;
                    }
                case SFSAction.TRAIN_SAILORS:
                    {
                        SceneTransition.Instance.ShowWaiting(false);
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            UserData.Instance.TrainedToday = packet.GetIntArray("trained_today");
                            for (int i = 0; i < UserData.Instance.TrainedToday.Length; i++)
                            {
                                Debug.Log("81: " + UserData.Instance.TrainedToday[i]);
                            }
                        }
                        break;
                    }
                case SFSAction.SHIP_DATA:
                    {
                        SceneTransition.Instance.ShowWaiting(false);
                        if (errorCode == SFSErrorCode.SUCCESS)
                        {
                            UserData.Instance.SailLevel = packet.GetInt("sail_level");
                            UserData.Instance.HelmLevel = packet.GetInt("helm_level");
                        }
                        break;
                    }
            }
            if (errorCode != SFSErrorCode.SUCCESS)
            {
                Debug.LogWarning($"Packet {action} Fail, Error Code: {errorCode}");
            }
            foreach (NetworkActionListenerDelegate listener in new List<NetworkActionListenerDelegate>(serverActionListeners))
            {
                listener(action, errorCode, packet);
            }
        }
        public static void Listen(NetworkActionListenerDelegate listener)
        {
            serverActionListeners.Add(listener);
        }
        public static void RemoveListener(NetworkActionListenerDelegate listener)
        {
            serverActionListeners.Remove(listener);
        }
        public void GetServerTime()
        {
            Send(SFSAction.GET_SERVER_TIME);
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
