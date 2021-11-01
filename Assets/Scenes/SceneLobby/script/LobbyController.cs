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
using Sfs2X.Requests;

public class LobbyController : MonoBehaviour
{

	//----------------------------------------------------------
	// UI elements
	//----------------------------------------------------------

	public Text loggedInText;

	//----------------------------------------------------------
	// Private properties
	//----------------------------------------------------------

	private SmartFox sfs;
	private bool shuttingDown;

	//----------------------------------------------------------
	// Unity callback methods
	//----------------------------------------------------------

	void Awake()
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

		loggedInText.text = "Logged in as " + sfs.MySelf.Name;

		// Register event listeners
		sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
		sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);


		// Join the lobby Room (must exist in the Zone!)
		sfs.Send(new JoinRoomRequest("The Lobby"));
	
	}

	// Update is called once per frame
	void Update()
	{
		if (sfs != null)
			sfs.ProcessEvents();
	}

	void OnApplicationQuit()
	{
		shuttingDown = true;
	}



	public void OnLogoutButtonClick()
	{
		// Disconnect from server
		sfs.Disconnect();
	}



	public void OnStartNewGameButtonClick()
	{

		//TODO Request Game creation to server
		reset();
		SceneManager.LoadScene("Scenes/SceneCombat/SceneGame");

	}

	public void OnButtonPickTeamClick()
    {
		reset();
		SceneManager.LoadScene("Scenes/ScenePickTeam/ScenePickTeam");
	}

	//----------------------------------------------------------
	// Private helper methods
	//----------------------------------------------------------

	private void reset()
	{
		// Remove SFS2X listeners
		sfs.RemoveAllEventListeners();
	}

	
	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------

	private void OnConnectionLost(BaseEvent evt)
	{
		// Remove SFS2X listeners
		reset();

		if (shuttingDown == true)
			return;

		// Return to login scene
		SceneManager.LoadScene("Scenes/SceneLogin/SceneLogin");
	}

	private void OnRoomJoin(BaseEvent evt)
	{
		Room room = (Room)evt.Params["room"];

		// Show system message
		Debug.Log("\nYou joined a Room: " + room.Name);
	}

	private void OnRoomJoinError(BaseEvent evt)
	{
		// Show error message
		Debug.Log("Room join failed: " + (string)evt.Params["errorMessage"]);
	}

}
