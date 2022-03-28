using Piratera.GUI;
using Piratera.Network;
using Piratera.Sound;
using Piratera.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenePreparePvPUI : MonoBehaviour
{
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
    [SerializeField]
    private SpriteRenderer battlefield;
    public PvPSquadAContainer squaA;

    [Header("UI")]
    [SerializeField]
    private Button[] buttons;
    private float remainTime;
    private float maxTime;
    private bool counting = false;

    private void Awake()
    {
        Input.multiTouchEnabled = false;
        NetworkController.AddServerActionListener(OnReceiveServerAction);
    }
    private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        switch (action)
        {
            case SFSAction.PVP_CONFIRM:
                GuiManager.Instance.ShowGuiWaiting(false);
                break;
        }
    }
    private void OnDestroy()
    {
        NetworkController.RemoveServerActionListener(OnReceiveServerAction);
    }
    private void Start()
    {
        SoundMgr.PlayBGMusic(PirateraMusic.COMBAT);
        PvPSquadAContainer.Draging = false;
        Init();
        UpdateBattleFieldImage();
    }
    public void Init()
    {
        TeamPvPCombatPrepareData data = TeamPvPCombatPrepareData.Instance;
        remainTime = maxTime = data.countdown;
        if (data.countdown > 0)
        {
            userNameA.text = data.YourName.LimitLength(30);
            userNameB.text = data.OpponentName.LimitLength(30);

            avatarA.LoadAvatar(data.YourAvatar);
            avatarB.LoadAvatar(data.OpponentAvatar);
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
        SendStartCombat();
    }
    public void SendStartCombat()
    {
        if (!squaA.HaveSailor())
        {
            GuiManager.Instance.ShowPopupNotification("Need at least one Sailor");
        }
        else
        {
            GuiManager.Instance.ShowGuiWaiting(true);
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutSFSArray("fgl", TeamPvPCombatPrepareData.Instance.YourFightingLine.ToSFSArray());
            NetworkController.Send(SFSAction.PVP_CONFIRM, sfsObject);
        }
    }
    private void UpdateBattleFieldImage()
    {
        battlefield.sprite = PvPData.Instance.GetAdvantageBackgroundSprite(TeamPvPCombatPrepareData.Instance.defense_advantage);
    }
}
