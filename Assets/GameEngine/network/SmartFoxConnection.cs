using UnityEngine;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;

/**
 * Singleton class with static fields to hold a reference to SmartFoxServer connection.
 * It is useful to access the SmartFox class from anywhere in the game.
 */
public class SmartFoxConnection : MonoBehaviour
{
	private static SmartFoxConnection mInstance; 
	private static SmartFox sfs;
	public const string CLIENT_REQUEST = "clrq";
    public static string ACTION_INCORE = "acc";
	public static string ERROR_CODE = "error_code";

	public static SmartFox Connection {
		get {
			if (mInstance == null) {
				mInstance = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
			}
			return sfs;
		}
		set {
			if (mInstance == null) {
				mInstance = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
			}
			sfs = value;
		} 
	}
	
	public static bool IsInitialized {
		get { 
			return (sfs != null); 
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

	// Handle disconnection auto magically
	// ** Important for Windows users - can cause crashes otherwise
	void OnApplicationQuit() { 
		if (sfs.IsConnected) {
			sfs.Disconnect();
		}
	} 
}


