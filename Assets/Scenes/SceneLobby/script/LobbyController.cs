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

public class LobbyController : MonoBehaviour
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

	[SerializeField]
	private Slider userExp;

	public static LobbyController Instance;
	private void Awake() { Instance = this; }
	private void OnDestroy() { Instance = null; }
	private void Start()
    {
		userName.text = UserData.Instance.Username;
		userLevel.text = "" + UserData.Instance.Level;
		userBeri.text = "" + UserData.Instance.Beri;
		userStamina.text = "" + UserData.Instance.Stamina;
		//userExp.value = UserData.Instance.GetExpProgress();
		StartLoadAvatar(UserData.Instance.Avatar);

		// Join the lobby Room (must exist in the Zone!)
		// sfs.Send(new JoinRoomRequest("The Lobby"));

		NetworkController.Instance.Send(SFSAction.LOAD_LIST_HERO_INFO);
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
		NetworkController.Instance.Logout();
	}
	public void OnStartNewGameButtonClick()
	{
		//SceneManager.LoadScene("SceneCombat2d");
		NetworkController.Instance.Send(SFSAction.COMBAT_BOT);
	}
	public void OnButtonPickTeamClick()
    {
		SceneManager.LoadScene("ScenePickTeam");
	}
	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------
	public void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
	{
		switch (action)
		{
			
		}
	}
}
