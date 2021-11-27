using DG.Tweening;
using Piratera.GUI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Piratera.Utils;
using System.Collections.Generic;
using UnityEngine.Events;

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

	[SerializeField]
	private Button[] leftButtons;

	[SerializeField]
	private Button[] rightButtons;

	[SerializeField]
	private Button buttonFight;

	[SerializeField]
	private Image background;

 

    // Start is called before the first frame update
    void Start()
    {
		
		StartLoadAvatar(UserData.Instance.Avatar);
		UpdateUserInfo();
		GameEvent.UserDataChanged.AddListener(UpdateUserInfo);
		GameEvent.UserBeriChanged.AddListener(OnBeriChanged);
		GameEvent.UserStaminaChanged.AddListener(OnStaminaChanged);
		RunAppearAction();
	}

    private void OnBeriChanged(long oldValue, long newValue)
    {
		DoTweenUtils.UpdateNumber(userBeri, oldValue, newValue, x => StringUtils.ShortNumber(x));

	}

	private void OnStaminaChanged(int oldValue, int newValue)
	{
		DoTweenUtils.UpdateNumber(userStamina, oldValue, newValue, x => UserData.Instance.GetStaminaFormat((int)x));
	}

	private void OnDestroy()
    {
		GameEvent.UserDataChanged.RemoveListener(UpdateUserInfo);
		GameEvent.UserBeriChanged.RemoveListener(OnBeriChanged);
		GameEvent.UserStaminaChanged.RemoveListener(OnStaminaChanged);
	}

    void UpdateUserInfo(List<string> changedVars)
    {
		
		if (changedVars.Contains(UserInfoPropertiesKey.USERNAME))
        {
			userName.DOText(UserData.Instance.Username.LimitLength(11), 0.5f).SetEase(Ease.InOutCubic);
		}
		if (changedVars.Contains(UserInfoPropertiesKey.LEVEL))
		{
			userLevel.DOText(UserData.Instance.Level.ToString(), 0.5f, false, ScrambleMode.Numerals).SetEase(Ease.Linear);
		}
		if (changedVars.Contains(UserInfoPropertiesKey.EXP))
		{
			userExp.DOValue(UserData.Instance.GetExpProgress(), 0.6f);
		}

	}

	void UpdateUserInfo()
	{
		userExp.DOValue(UserData.Instance.GetExpProgress(), 0.6f);
		userName.DOText(UserData.Instance.Username.LimitLength(11), 0.5f).SetEase(Ease.InOutCubic);
		userLevel.DOText(UserData.Instance.Level.ToString(), 0.5f, false, ScrambleMode.Numerals).SetEase(Ease.Linear);

		DoTweenUtils.UpdateNumber(userBeri, 0, UserData.Instance.Beri, x => StringUtils.ShortNumber(x));
		DoTweenUtils.UpdateNumber(userStamina, 0, UserData.Instance.Stamina, x => UserData.Instance.GetStaminaFormat((int)x));
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
		 NetworkController.Logout();
	}
	public void OnStartNewGameButtonClick()
	{
		OnButtonPickTeamClick();
		return;
		SceneManager.LoadScene("SceneSelectMode");
	}
	public void OnButtonPickTeamClick()
	{
		SceneManager.LoadScene("ScenePickTeam");
	}

	public void ShowStaminaPack()
    {
		GuiManager.Instance.AddGui<GuiBuyStamina>("Prefap/GuiBuyStamina", LayerId.GUI);
	}

	public void OnBuyBeri()
    {
		Application.OpenURL("https://piratera.io/");
	}

	public void ShowCommingSoon()
	{
		GuiManager.Instance.ShowPopupNotification("Coming soon");

	}

	private void Update()
	{
		if (UserData.Instance.IsRecorveringStamina())
		{
			TimeSpan remaining = TimeSpan.FromMilliseconds(UserData.Instance.TimeToHaveNewStamina());
			userStaminaCountDown.text = string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds);
		} else
        {
			userStaminaCountDown.text = "";

		}
	}

	private void RunAppearAction()
    {

		Vector3 originScale = background.transform.localScale;
		background.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
		background.transform.DOScale(originScale, 0.4f);
		
        foreach (Button button in leftButtons)
        {

			DoTweenUtils.FadeAppearX(button, -Screen.width/2, 1f, UnityEngine.Random.Range(0.4f, 0.5f));
			
			
        }

		foreach (Button button in rightButtons)
		{
			DoTweenUtils.FadeAppearX(button, Screen.width/2, 1f, UnityEngine.Random.Range(0.4f, 0.5f));
		}

		DoTweenUtils.ButtonBigAppear(buttonFight, 0.5f, Vector3.one ,1f);
	}

	public void ShowGuiCheat()
	{
		GuiManager.Instance.AddGui<PopupCheatGame>("Cheat/PopupCheat");
	}
}
