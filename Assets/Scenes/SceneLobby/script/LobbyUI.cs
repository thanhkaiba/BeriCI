using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
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
	private Text userStaminaCountDown;

	[SerializeField]
	private Slider userExp;
	// Start is called before the first frame update
	void Start()
    {
		userName.text = UserData.Instance.Username;
		userLevel.text = "" + UserData.Instance.Level;
		userBeri.text = "" + UserData.Instance.Beri;
		userStamina.text = "" + UserData.Instance.Stamina;
		userExp.value = UserData.Instance.GetExpProgress();
		StartLoadAvatar(UserData.Instance.Avatar);
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
		SceneManager.LoadScene("ScenePickTeamBattle");
	}

	private void Update()
	{
		if (UserData.Instance.IsRecorveringStamina())
		{
			TimeSpan remaining = TimeSpan.FromMilliseconds(UserData.Instance.TimeToHaveNewStamina());
			userStaminaCountDown.text = string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds);
		}
	}
}
