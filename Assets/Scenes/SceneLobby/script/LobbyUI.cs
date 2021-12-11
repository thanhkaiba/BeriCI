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
using Piratera.Sound;

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
	private Text userBeri;

	[SerializeField]
	private Text userStamina;

	[SerializeField]
	private Text userStaminaCountDown;


	[SerializeField]
	private Button[] leftButtons;

	[SerializeField]
	private Button[] rightButtons;

	[SerializeField]
	private Button buttonAdventure;

	[SerializeField]
	private LoadAvatarUtils loadAvatarUtils;

	[SerializeField]
	private List<Transform> nodeSailors;

	[SerializeField]
	private Transform buttonCol;
	[SerializeField]
	private Transform sail;
	[SerializeField]
	private Transform background;

	// Start is called before the first frame update
	void Start()
    {
		SoundMgr.PlayBGMusic(PirateraMusic.LOBBY);
		loadAvatarUtils.LoadAvatar(userAvatar, UserData.Instance.Avatar);
		UpdateUserInfo();
		GameEvent.UserDataChanged.AddListener(UpdateUserInfo);
		GameEvent.UserBeriChanged.AddListener(OnBeriChanged);
		GameEvent.UserStaminaChanged.AddListener(OnStaminaChanged);
		RunAppearAction();
		ShowListSailors();
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
	}

	void UpdateUserInfo()
	{
		userName.DOText(UserData.Instance.Username.LimitLength(11), 0.5f).SetEase(Ease.InOutCubic);
		DoTweenUtils.UpdateNumber(userBeri, 0, UserData.Instance.Beri, x => StringUtils.ShortNumber(x));
		DoTweenUtils.UpdateNumber(userStamina, 0, UserData.Instance.Stamina, x => UserData.Instance.GetStaminaFormat((int)x));
	}

	public void OnLogoutButtonClick()
	{
		GuiManager.Instance.AddGui<GuiSetting>("Prefap/GuiSetting", LayerId.GUI);
	}
	public void OnStartNewGameButtonClick()
	{
		//NetworkController.Send(SFSAction.COMBAT_PREPARE);
		NetworkController.Send(SFSAction.PVE_PLAY);
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
		GuiManager.Instance.ShowPopupNotification("Coming Soon!");
	}

	public void ShowSceneCrew()
    {
		SceneManager.LoadScene("SceneCrew");
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
		for (int i = 0; i < leftButtons.Length; i++)
		{
			DoTweenUtils.FadeAppearY(leftButtons[i], -200, 0.4f, 1.2f + i * 0.2f, Ease.OutCirc);
		}

		DoTweenUtils.ButtonBigAppear(buttonAdventure, 0.6f, Vector3.one, 0.7f);

		buttonCol.Translate(200, 0, 0);
		buttonCol.DOMove(new Vector3(-200, 0, 0), 0.8f).SetRelative().SetEase(Ease.OutCirc);

		sail.Translate(50, 180, 0);
		sail.DOMove(new Vector3(-50, -180, 0), 0.8f).SetRelative().SetEase(Ease.OutCirc);

		var scale = new Vector3(0.6f, 0.6f, 0.6f);
		background.localScale += scale;
		background.DOScale(-scale, 0.8f).SetRelative().SetEase(Ease.OutCirc);
	}
	private void ShowListSailors()
	{
		for (int i = 0; i < nodeSailors.Count; i++)
		{
			if (i >= CrewData.Instance.Sailors.Count) break;
			var model = CrewData.Instance.Sailors[i];
			Instantiate(model.config_stats.model, nodeSailors[i]);
		}
	}
	public void ShowGuiCheat()
	{
		GuiManager.Instance.AddGui<PopupCheatGame>("Cheat/PopupCheat");
	}
}
