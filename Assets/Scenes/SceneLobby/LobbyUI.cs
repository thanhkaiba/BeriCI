using DG.Tweening;
using Piratera.Cheat;
using Piratera.Config;
using Piratera.Engine;
using Piratera.GUI;
using Piratera.Network;
using Piratera.Sound;
using Piratera.Utils;
using Sfs2X.Entities.Data;
using Spine;
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
    private Text maintainText;

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
    private Text arenaSeason, areaTime;
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
    public MultiRoyalCollectingController royal;
    [SerializeField]
    private Button buttonCheat;

    [SerializeField]
    private GameObject popupLibrary;
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
        Application.targetFrameRate = 60;
        SoundMgr.PlayBGMusic(PirateraMusic.LOBBY);
        userAvatar.ShowAvatar(UserData.Instance.AvtId);
        PresentData();
        GameEvent.UserDataChanged.AddListener(UpdateUserInfo);
        GameEvent.UserBeriChanged.AddListener(OnBeriChanged);
        GameEvent.UserStaminaChanged.AddListener(OnStaminaChanged);
        GameEvent.FlyStamina.AddListener(FlyStamina);
        GameEvent.FlyBeri.AddListener(FlyBeri);
        GameEvent.MaintainDataChanged.AddListener(UpdateMaintain);
        GameEvent.UserAvtChange.AddListener(UpdateAvatar);
        UpdateMaintain();
        RunAppearAction();
        ShowListSailors();
        Time.timeScale = 1;

        if (TutorialMgr.Instance.CheckTutOpenSlot())
        {
            ShowFocusLineUp();
        }

        if (TutorialMgr.Instance.CheckTutStartUp())
        {
            if (TutorialMgr.Instance.CheckTutStartUp_Greeting()) ShowNPCTut();
            else
            {
                ShowFocusPvE();
                TutorialMgr.Instance.CompleteStartUp();
            }
        }
        var pvpData = PvPData.Instance;
        var startTime = GameUtils.FromUnixTime(pvpData.StartSeason);
        var endTime = GameUtils.FromUnixTime(pvpData.EndSeason);
        arenaSeason.text = "Season " + pvpData.SeasonId;
        areaTime.text = startTime.ToString("dd/MM")
            + " - " + endTime.ToString("dd/MM");
    }
    void UpdateMaintain()
    {
        DOTween.Kill(maintainText.gameObject);
        if (MaintainManager.GetRemainTimeToMaintain() > 0)
        {
            Sequence s = DOTween.Sequence();
            s.AppendCallback(() => maintainText.text = MaintainManager.GetMaintainMessage());
            s.AppendInterval(1);
            s.SetTarget(maintainText.gameObject).SetLink(maintainText.gameObject);
            s.SetLoops(-1);
        }
        else
        {
            maintainText.text = "";
        }
    }
    private void OnBeriChanged(long oldValue, long newValue)
    {
        DoTweenUtils.UpdateNumber(userBeri, oldValue, newValue, x => StringUtils.ShortNumber(x, 6));
    }

    public void OnStaminaChanged(int oldValue, int newValue)
    {
        DoTweenUtils.UpdateNumber(userStamina, oldValue, newValue, x => StaminaData.Instance.GetStaminaFormat(StringUtils.ShortNumber(x, 6)));
    }
    public void FlyStamina()
    {
        royal.CollectItem(1, 5, 1, () => { });
    }

    public void FlyBeri()
    {
        royal.CollectItem(0, 5, 1, () => { });
    }
    private void OnDestroy()
    {
        GameEvent.UserDataChanged.RemoveListener(UpdateUserInfo);
        GameEvent.UserBeriChanged.RemoveListener(OnBeriChanged);
        GameEvent.UserStaminaChanged.RemoveListener(OnStaminaChanged);
        GameEvent.FlyStamina.RemoveListener(FlyStamina);
        GameEvent.FlyBeri.RemoveListener(FlyBeri);
        GameEvent.MaintainDataChanged.RemoveListener(UpdateMaintain);
        GameEvent.UserAvtChange.RemoveListener(UpdateAvatar);
    }

    void UpdateUserInfo(List<string> changedVars)
    {
        userName.DOText(UserData.Instance.Username.LimitLength(18), 0.5f).SetEase(Ease.InOutCubic);
    }
    private void UpdateAvatar()
    {
        userAvatar.ShowAvatar(UserData.Instance.AvtId);
    }
    void PresentData()
    {
        userName.text = UserData.Instance.Username.LimitLength(15);
        userBeri.text = StringUtils.ShortNumber(UserData.Instance.Beri, 6);
        userStamina.text = StaminaData.Instance.GetStaminaFormat(StringUtils.ShortNumber(StaminaData.Instance.Stamina, 6));
        //prizes.text = PirateWheelData.Instance.getPrize();


        //for (int i = 0; i < PirateWheelData.Instance.Length; i++)
        //{
        //    Debug.Log(ps[i]);
        //}
    }

    public void OnLogoutButtonClick()
    {
        GuiManager.Instance.AddGui("Prefap/GuiSetting");
    }
    public void OnStartPVEMode()
    {
        var hand = GameObject.Find("hand_pve");
        if (hand) Destroy(hand);
        if (!MaintainManager.CanPlay())
        {
            GuiManager.Instance.ShowPopupNotification("This function is locked due to upcoming server maintenance");
            return;
        }

        if (CrewData.Instance.IsEmpty())
        {
            GuiManager.Instance.ShowPopupBuySailor();
            return;
        }

        GameObject go = GuiManager.Instance.AddGui("Prefap/GuiConfirmPVE");
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
    public void OnButtonShipClick()
    {
        SceneManager.LoadScene("SceneUpgradeShip");
    }

    public void AvatarClick()
    {
        var gui = GuiManager.Instance.AddGui("UserInfo/PopupUserInfo");
        gui.GetComponent<PopupUserInfo>().SetUID(UserData.Instance.UID);
    }
    public void ShowStaminaPack()
    {
        SceneTransition.Instance.ShowWaiting(true);
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
    public void ClickGuide()
    {
        GuiManager.Instance.AddGui(popupLibrary);
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
    private float timeSeqChat = 3.0f;
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
        timeSeqChat -= Time.deltaTime;
        if (timeSeqChat < 0)
        {
            ShowRandomChat();
            timeSeqChat = MathUtils.RandomInt(5, 7);
        }
    }

    public void OnArenaClick()
    {
        if (!PvPData.Instance.HaveEnoughSailor())
        {
            GuiManager.Instance.ShowPopupNotification("You need at least 1 non-trial sailor to enter the Arena");
            return;
        }

        if (UserData.Instance.NumSlot < GlobalConfigs.LineUp.max)
        {
            GuiManager.Instance.ShowPopupNotification("You must have 5 slots to join Arena!");
            return;
        }

        if (PvPData.Instance.ShowedTutorial < PvPData.PVP_TURORIAL_STEP.POPUP_WELCOME_ARENA)
        {
            GuiManager.Instance.AddGui("Prefap/PopupWelcomeArena");
        } else if (!PvPData.Instance.HaveJoin)
        {
            //SceneManager.LoadScene("SceneLineUpDefense");
            SceneTransition.Instance.LoadScene("SceneLineUpDefense", TransitionType.ARENA);
        }
        else
        {
            //SceneManager.LoadScene("SceneArena");
            SceneTransition.Instance.LoadScene("SceneArena", TransitionType.ARENA);
        }
    }
    private void RunAppearAction()
    {
        for (int i = 0; i < leftButtons.Length; i++)
        {
            DoTweenUtils.FadeAppearY(leftButtons[i], -200, 0.4f, 0.2f + i * 0.1f, Ease.OutCirc);
        }

        nodeUser.Translate(-250, 0, 0);
        nodeUser.DOMove(new Vector3(250, 0, 0), 0.8f).SetRelative().SetEase(Ease.OutCirc).SetTarget(nodeUser).SetLink(nodeUser.gameObject);

        DoTweenUtils.ButtonBigAppear(buttonAdventure, 0.6f, Vector3.one, 0.2f);

        buttonCol.Translate(250, 0, 0);
        buttonCol.DOMove(new Vector3(-250, 0, 0), 0.8f).SetRelative().SetEase(Ease.OutCirc).SetTarget(buttonCol).SetLink(buttonCol.gameObject);

        /* sail.Translate(50, 180, 0);
         sail.DOMove(new Vector3(-50, -180, 0), 0.8f).SetRelative().SetEase(Ease.OutCirc).SetTarget(sail).SetLink(sail.gameObject);*/

        var scale = new Vector3(0.6f, 0.6f, 0.6f);
        background.localScale += scale;
        background.DOScale(-scale, 0.8f).SetRelative().SetEase(Ease.OutCirc).SetTarget(background).SetLink(background.gameObject);
    }
    private List<Transform> sailors = new List<Transform>(); 
    private void ShowListSailors()
    {
        var listLineUp = CrewData.Instance.GetSquadModelList();
        listLineUp.Sort();
        listLineUp.Reverse();
        for (int i = 0; i < nodeSailors.Count; i++)
        {
            if (i >= listLineUp.Count) break;
            var model = listLineUp[i];
            GameObject GO = Instantiate(GameUtils.GetSailorModelPrefab(model.config_stats.root_name), nodeSailors[i]);
            Transform modelTransform = GO.transform.FindDeepChild("model");
            modelTransform.GetComponent<Renderer>().sortingOrder = 3;
            modelTransform.Find("shadow").GetComponent<Renderer>().sortingOrder = 3;
            sailors.Add(GO.transform);
        }
    }
    private void ShowRandomChat()
    {
        if (sailors.Count == 0) return;
        int idx = MathUtils.RandomInt(0, 30);
        string text = "...";
        switch(idx)
        {
            case 0:
                text = "I want to swim...";
                break;
            case 1:
                text = "The weather is so nice!";
                break;
            case 2:
                text = "I saw a monster yesterday";
                break;
            case 3:
                text = "Wind is blowing";
                break;
            case 4:
                text = "Achoo!!";
                break;
            case 5:
                text = "I love sunshine";
                break;
            case 6:
                text = "Let's go";
                break;
            case 7:
                text = "Who wants ice-cream?";
                break;
            case 8:
                text = "Miss you so much\nmy love...";
                break;
            case 9:
                text = "Oh cool";
                break;
            case 10:
                text = "Today I feel so good";
                break;
            case 11:
                text = "I don't want to say anything";
                break;
            case 12:
                text = "I don't want to say anything";
                break;
            case 13:
                text = "Make-up! We are coming to town";
                break;
            case 14:
                text = "Dock to that island";
                break;
            case 15:
                text = "No";
                break;
            case 16:
                text = "3 days on hunger-strike. I want an octopus";
                break;
            case 17:
                text = "I need vitamin C";
                break;
            case 18:
                text = "Bloodthirsty";
                break;
            case 19:
                text = "Taking my heart plz";
                break;
            case 20:
                text = "Let's party\nI'm dying...";
                break;
            case 21:
                text = "I don't feel so good";
                break;
            case 22:
                text = "hmm";
                break;
            case 23:
                text = "...";
                break;
            case 24:
                text = "Do not copycat me";
                break;
            case 25:
                text = "It's cold\nI'm sick";
                break;
            case 26:
                text = "Stop saying stupid things";
                break;
            case 27:
                text = "Need a bowl of Pho";
                break;
            case 28:
                text = "How about Banh Mi";
                break;
            case 29:
                text = "All good??";
                break;
            case 30:
                text = "Is that Ha Long bay?";
                break;
        }
        var sailor = sailors[MathUtils.RandomInt(0, sailors.Count - 1)];
        var chat = GameUtils.ShowChat(sailor, text, 5);
        chat.GetComponent<Canvas>().sortingOrder = 5;
        chat.transform.localScale = chat.transform.localScale * 0.64f;
    }
    private void ShowNPCTut()
    {
        var go = Resources.Load<GameObject>("Prefap/Tuts/NPCTut");
        GameObject hand = Instantiate(go, GuiManager.Instance.GetCanvas().transform);
    }
    public void ShowTutOpenCrew()
    {
        var go = Resources.Load<GameObject>("Prefap/Tuts/hand");
        GameObject hand = Instantiate(go, GuiManager.Instance.GetCanvas().transform);
        var pos = GameObject.Find("ButtonCrew").transform.position;
        hand.transform.position = pos;
    }
    public void ShowFocusPvE()
    {
        var blockScene = AddBlockScene();
        Sequence s = DOTween.Sequence();
        s.AppendInterval(1);
        s.AppendCallback(() => {
            Destroy(blockScene);
            var go = Resources.Load<GameObject>("Prefap/Tuts/hand");
            GameObject hand = Instantiate(go, GuiManager.Instance.GetCanvas().transform);
            hand.transform.position = GameObject.Find("ButtonPVE").transform.position;
            hand.name = "hand_pve";
        });
    }
    private void ShowFocusLineUp()
    {
        var blockScene = AddBlockScene();
        Sequence s = DOTween.Sequence();
        s.AppendInterval(1);
        s.AppendCallback(() => {
            Destroy(blockScene);
            var go = Resources.Load<GameObject>("Prefap/Tuts/hand");
            GameObject hand = Instantiate(go, GuiManager.Instance.GetCanvas().transform);
            var pos = GameObject.Find("ButtonLineUp").transform.position;
            hand.transform.position = pos;
        });
    }
    private GameObject AddBlockScene()
    {
        var blockScene = new GameObject();
        blockScene.transform.SetParent(GuiManager.Instance.GetCanvas().transform);
        blockScene.transform.localPosition = Vector3.zero;
        blockScene.AddComponent<CanvasRenderer>();
        RectTransform rectTransform = blockScene.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Screen.width * 2, Screen.height * 2);
        blockScene.transform.SetAsFirstSibling();
        Image image = blockScene.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0);
        image.raycastTarget = true;
        return blockScene;
    }
    public void ShowGuiCheat()
    {
#if PIRATERA_DEV || PIRATERA_QC
        GuiManager.Instance.AddGui("Cheat/PopupCheat");
#endif
    }
}
