using UnityEngine;
using UnityEngine.SceneManagement;
using Sfs2X;
using Sfs2X.Core;

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

}
