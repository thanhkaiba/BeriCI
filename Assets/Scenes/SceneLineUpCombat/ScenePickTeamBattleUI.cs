using Piratera.GUI;
using Piratera.Sound;
using Piratera.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenePickTeamBattleUI : MonoBehaviour
{
    [SerializeField]
    private Text textTeamBonus;
    [SerializeField]
    private Text textCountDown;
    [SerializeField]
    private Slider sliderCountDown;
    [Header("UserA Info")]
    [SerializeField]
    private Text userNameA;
    [SerializeField]
    private UserAvatar avatarA;
    [Header("UserB Info")]
    [SerializeField]
    private Text userNameB;
    [SerializeField]
    private UserAvatar avatarB;
    public SquadAContainer squaA;

    [Header("UI")]
    [SerializeField]
    private Button[] buttons;
    private float remainTime;
    private float maxTime;
    private bool counting = false;


    private void Awake()
    {
        Input.multiTouchEnabled = false;
        NetworkController.Listen(OnReceiveServerAction);

        textTeamBonus.text = "Team bonus: " + GameUtils.GetTeamBonus(TeamCombatPrepareData.Instance.YourFightingLine).ToString("N0");
        GameEvent.PrepareSquadChanged.AddListener(() =>
            textTeamBonus.text = "Team bonus: " + GameUtils.GetTeamBonus(TeamCombatPrepareData.Instance.YourFightingLine).ToString("N0"));
    }

    private void OnReceiveServerAction(Action action, SFSErrorCode errorCode, ISFSObject packet)
    {
        switch (action)
        {
            case Action.PVE_SURRENDER:
            case Action.PVE_CONFIRM:
                SceneTransition.Instance.ShowWaiting(false);
                break;
        }
    }

    private void OnDestroy()
    {
        NetworkController.RemoveListener(OnReceiveServerAction);
    }

    private void Start()
    {
        SoundMgr.PlayBGMusic(PirateraMusic.COMBAT);
        SquadAContainer.Draging = false;
        Init();
    }



    public void Init()
    {
        TeamCombatPrepareData data = TeamCombatPrepareData.Instance;
        remainTime = maxTime = data.countdown;
        if (data.countdown > 0)
        {
            userNameA.text = data.YourName.LimitLength(30);
            userNameB.text = data.OpponentName.LimitLength(30);

            avatarA.ShowAvatar(data.YourAvatar);
            avatarB.ShowAvatar(data.OpponentAvatar);
            buttons[0].interactable = true;
            buttons[1].interactable = true;
        }
        counting = true;
    }
    private void LateUpdate()
    {
        if (!counting) return;
        if (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            textCountDown.text = "" + Mathf.Ceil(remainTime);
            sliderCountDown.value = remainTime / maxTime;
        }
        else
        {
            OnTimeOut();
            counting = false;
        }
    }

    public void OnTimeOut()
    {

        if (!squaA.HaveSailor())
        {

            NetworkController.SendSurrenderPVEToSever();
            GuiManager.Instance.ShowPopupNotification("Lose because there are no Sailors", () => SceneManager.LoadScene("SceneLobby"));
        }
        else SendStartCombat();


    }

    public void SendStartCombat()
    {
            SceneTransition.Instance.ShowWaiting(true, false);
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutSFSArray("fgl", TeamCombatPrepareData.Instance.YourFightingLine.ToSFSArray());
            NetworkController.Send(Action.PVE_CONFIRM, sfsObject);
    }   
    public void Surrender()
    {
        GuiManager.Instance.AddGui("Prefap/GuiSurrender");
    }
}
