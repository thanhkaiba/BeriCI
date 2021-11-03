using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;

public class LobbyController : BaseController
{

	//----------------------------------------------------------
	// UI elements
	//----------------------------------------------------------

	public Text loggedInText;


    private void Start()
    {
		loggedInText.text = "Logged in as " + sfs.MySelf.Name;

		sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
		sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);


		// Join the lobby Room (must exist in the Zone!)
		// sfs.Send(new JoinRoomRequest("The Lobby"));
	}

    // Update is called once per frame
    protected override void Update()
	{
		if (sfs != null)
			sfs.ProcessEvents();

		
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
	// SmartFoxServer event listeners
	//----------------------------------------------------------

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
