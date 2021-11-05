using UnityEngine;
using UnityEngine.SceneManagement;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;

public class BaseController : MonoBehaviour
{

	//----------------------------------------------------------
	// Private properties
	//----------------------------------------------------------

	protected SmartFox sfs;
	private bool shuttingDown;

	//----------------------------------------------------------
	// Unity callback methods
	//----------------------------------------------------------

	protected virtual void Awake()
	{
		Application.runInBackground = true;

		if (SmartFoxConnection.IsInitialized)
		{
			sfs = SmartFoxConnection.Connection;
		}
		else
		{
			SceneManager.LoadScene("Scenes/SceneLogin/SceneLogin");
			return;
		}


		// Register event listeners
		sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtentionResponse);

	}

	// Update is called once per frame
	protected virtual void Update()
	{
		if (sfs != null)
			sfs.ProcessEvents();
	}

	void OnApplicationQuit()
	{
		shuttingDown = true;
	}

	//----------------------------------------------------------
	// Private helper methods
	//----------------------------------------------------------

	protected virtual void reset()
	{
		// Remove SFS2X listeners
		sfs.RemoveAllEventListeners();
	}


	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------

	protected virtual void OnConnectionLost(BaseEvent evt)
	{
		// Remove SFS2X listeners
		reset();

		if (shuttingDown == true)
			return;

		// Return to login scene
		SceneManager.LoadScene("Scenes/SceneLogin/SceneLogin");
	}

	protected virtual void OnExtentionResponse(BaseEvent evt)
    {

		ISFSObject packet = (ISFSObject)evt.Params["params"];

		string cmd = (string)evt.Params["cmd"];
		if (cmd == SmartFoxConnection.CLIENT_REQUEST)
		{
			Debug.Log("response:" + packet.GetDump());

			SFSAction action = (SFSAction)packet.GetInt(SmartFoxConnection.ACTION_INCORE);
			SFSErrorCode errorCode = (SFSErrorCode)packet.GetByte(SmartFoxConnection.ERROR_CODE);
			OnReceiveServerAction(action, errorCode, packet);
		}
	}

	protected virtual void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode , ISFSObject packet)
    {

    }

}
