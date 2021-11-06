using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;
using System.Collections;
using UnityEngine.Networking;
using Sfs2X.Entities.Data;
using System.Collections.Generic;

public class LobbyController : BaseController
{

	//----------------------------------------------------------
	// UI elements
	//----------------------------------------------------------

	[SerializeField]
	private Text userName;

	[SerializeField]
	private Image userAvatar;

	[SerializeField]
	private Text userLevel;

	[SerializeField]
	private Text userBeri;

	[SerializeField]
	private Text userStamina;


	private void Start()
    {
		userName.text = UserData.Instance.Username;
		userLevel.text = "Level: " + UserData.Instance.Level;
		userBeri.text = "Beri: " + UserData.Instance.Beri;
		userStamina.text = "Stamina: " + UserData.Instance.Stamina;
		StartLoadAvatar(UserData.Instance.Avatar);

		sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
		sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);


		// Join the lobby Room (must exist in the Zone!)
		// sfs.Send(new JoinRoomRequest("The Lobby"));

		SmartFoxConnection.Send(SFSAction.LOAD_LIST_HERO_INFO);
	}

    // Update is called once per frame
    protected override void Update()
	{
		if (sfs != null)
			sfs.ProcessEvents();

		
	}

	void StartLoadAvatar(string url)
	{
		StartCoroutine(LoadAvatar(url));
	}

	IEnumerator LoadAvatar(string url)
	{
		using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
		{
			yield return uwr.SendWebRequest();

			if (uwr.result != UnityWebRequest.Result.Success)
			{
				Debug.Log(uwr.error);
			}
			else
			{

				// Get downloaded asset bundle
				Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
				userAvatar.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));

			}
		}
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
		SceneManager.LoadScene("SceneCombat2d");

	}

	public void OnButtonPickTeamClick()
    {
		reset();
		SceneManager.LoadScene("ScenePickTeam");
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

	protected override void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
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

}
