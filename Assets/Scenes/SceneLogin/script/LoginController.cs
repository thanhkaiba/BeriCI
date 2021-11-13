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
	public static LoginController Instance;
	protected void Awake()
	{
		Instance = this;
		Application.runInBackground = true;
		// Enable interface
		enableLoginUI(true);
	}
	protected void OnDestroy()
	{
		Instance = null;
	}

	//----------------------------------------------------------
	// Public interface methods for UI
	//----------------------------------------------------------

	public void OnLoginButtonClick()
	{
		enableLoginUI(false);
		NetworkController.Instance.LoginToServer(new LoginData(nameInput.text, ""));
	}

	public void ReceiveJoinZoneSuccess(SmartFox sfs, SFSErrorCode errorCode, ISFSObject packet)
	{
		if (errorCode == SFSErrorCode.SUCCESS)
		{
			User user = sfs.MySelf;
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
			case SFSAction.LOAD_LIST_HERO_INFO:
				{
					if (errorCode == SFSErrorCode.SUCCESS)
					{
						CrewData.Instance.NewFromSFSObject(packet);
					}
					break;
				}
		}
	}
	private void OpenLobby()
    {
		SceneManager.LoadScene("Scenes/SceneLobby/SceneLobby");
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
	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------

	

}
