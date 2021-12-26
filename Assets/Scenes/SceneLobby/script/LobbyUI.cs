using DG.Tweening;
using Piratera.Cheat;
using Piratera.Constance;
using Piratera.GUI;
using Piratera.Network;
using Piratera.Sound;
using Piratera.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
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
    private UserAvatar userAvatar;

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
    private List<Transform> nodeSailors;

    [SerializeField]
    private Transform buttonCol;
    [SerializeField]
    private Transform sail;
    [SerializeField]
    private Transform nodeUser;
    [SerializeField]
    private Transform background;

    [SerializeField]
    private Button buttonCheat;

    // Start is called before the first frame update
    private void Awake()
    {
#if PIRATERA_DEV || PIRATERA_QC
        buttonCheat.gameObject.SetActive(true);
#else
        buttonCheat.gameObject.SetActive(false);
#endif
    }
    void Start()
    {
        SoundMgr.PlayBGMusic(PirateraMusic.LOBBY);
        userAvatar.LoadAvatar(UserData.Instance.Avatar);
        PresentData();
        GameEvent.UserDataChanged.AddListener(UpdateUserInfo);
        GameEvent.UserBeriChanged.AddListener(OnBeriChanged);
        GameEvent.UserStaminaChanged.AddListener(OnStaminaChanged);
        RunAppearAction();
        ShowListSailors();
    }

    private void OnBeriChanged(long oldValue, long newValue)
    {
        DoTweenUtils.UpdateNumber(userBeri, oldValue, newValue, x => StringUtils.ShortNumber(x, 6));

    }

    public void OnStaminaChanged(int oldValue, int newValue)
    {
        DoTweenUtils.UpdateNumber(userStamina, oldValue, newValue, x => StaminaData.Instance.GetStaminaFormat((int)x));
    }

    private void OnDestroy()
    {
        GameEvent.UserDataChanged.RemoveListener(UpdateUserInfo);
        GameEvent.UserBeriChanged.RemoveListener(OnBeriChanged);
        GameEvent.UserStaminaChanged.RemoveListener(OnStaminaChanged);
    }

    void UpdateUserInfo(List<string> changedVars)
    {
        userName.DOText(UserData.Instance.Username.LimitLength(18), 0.5f).SetEase(Ease.InOutCubic);
    }

    void PresentData()
    {
        userName.text = UserData.Instance.Username.LimitLength(18);
        userBeri.text = StringUtils.ShortNumber(UserData.Instance.Beri, 6);
        userStamina.text = StaminaData.Instance.GetCurrentStaminaFormat();
    }

    public void OnLogoutButtonClick()
    {
        GuiManager.Instance.AddGui<GuiSetting>("Prefap/GuiSetting", LayerId.GUI);
    }
    public void OnStartPVEMode()
    {
        if (CrewData.Instance.IsEmpty())
        {
            GuiManager.Instance.ShowPopupBuySailor();
            return;
        }
        GameObject go = GuiManager.Instance.AddGui<GuiConfirmPVE>("Prefap/GuiConfirmPVE", LayerId.GUI);
        go.GetComponent<GuiConfirmPVE>().lobby = this;
    }
    public void OnButtonPickTeamClick()
    {
        if (CrewData.Instance.IsEmpty())
        {
            GuiManager.Instance.ShowPopupBuySailor();
            return;
        }
        SceneManager.LoadScene("ScenePickTeam");
    }

    public void ShowStaminaPack()
    {
        GuiManager.Instance.ShowGuiWaiting(true);
        NetworkController.Send(SFSAction.GET_STAMINA_PACK);

    }

    public void OnBuyBeri()
    {
        Application.OpenURL(GameConst.MARKET_URL);
    }

    public void ShowCommingSoon()
    {
        GuiManager.Instance.ShowPopupNotification("Coming Soon!");
    }

    public void ShowSceneCrew()
    {
        if (CrewData.Instance.IsEmpty())
        {
            GuiManager.Instance.ShowPopupBuySailor();
            return;
        }
        SceneManager.LoadScene("SceneCrew");
    }

    private void Update()
    {
        if (StaminaData.Instance.IsRecorveringStamina())
        {
            TimeSpan remaining = TimeSpan.FromMilliseconds(StaminaData.Instance.TimeToHaveNewStamina());
            userStaminaCountDown.text = string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds);
        }
        else
        {
            userStaminaCountDown.text = "";

        }
    }

    private void RunAppearAction()
    {
        for (int i = 0; i < leftButtons.Length; i++)
        {
            DoTweenUtils.FadeAppearY(leftButtons[i], -200, 0.4f, 0.2f + i * 0.1f, Ease.OutCirc);
        }

        nodeUser.Translate(-250, 0, 0);
        nodeUser.DOMove(new Vector3(250, 0, 0), 0.8f).SetRelative().SetEase(Ease.OutCirc);

        DoTweenUtils.ButtonBigAppear(buttonAdventure, 0.6f, Vector3.one, 0.7f);

        buttonCol.Translate(250, 0, 0);
        buttonCol.DOMove(new Vector3(-250, 0, 0), 0.8f).SetRelative().SetEase(Ease.OutCirc);

        sail.Translate(50, 180, 0);
        sail.DOMove(new Vector3(-50, -180, 0), 0.8f).SetRelative().SetEase(Ease.OutCirc);

        var scale = new Vector3(0.6f, 0.6f, 0.6f);
        background.localScale += scale;
        background.DOScale(-scale, 0.8f).SetRelative().SetEase(Ease.OutCirc);
    }
    private void ShowListSailors()
    {
        var listLineUp = CrewData.Instance.GetSquadModelList();
        listLineUp.Sort();
        listLineUp.Reverse();
        for (int i = 0; i < nodeSailors.Count; i++)
        {
            if (i >= listLineUp.Count) break;
            var model = listLineUp[i];
            Instantiate(model.config_stats.model, nodeSailors[i]);
        }
    }
    public void ShowGuiCheat()
    {
#if PIRATERA_DEV || PIRATERA_QC
        GuiManager.Instance.AddGui<PopupCheatGame>("Cheat/PopupCheat");
#endif
    }
}
