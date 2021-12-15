using DG.Tweening;
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
    [SerializeField]
    private Button[] buttons;

    private float remainTime;
    private float maxTime;
    private bool counting = false;
    private void Start()
    {
        if (SoundMgr.Instance != null) SoundMgr.PlayBGMusic(PirateraMusic.COMBAT);
        Init();
    }

    public void OnJoinBattle()
    {
        //  SceneManager.LoadScene("SceneCombat2D");
    }

    public void OnSurrender()
    {
        // NetworkController.OnExtentionResponse();
    }

    public void Init()
    {
        TeamCombatPrepareData data = TeamCombatPrepareData.Instance;
        remainTime = maxTime = data.countdown;
        if (data.countdown > 0)
        {
            userNameA.text = data.YourName;
            userNameB.text = data.OpponentName;

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
        if (UIManager.Instance.reward.gameObject.activeInHierarchy)
            UIManager.Instance.reward.ConfirmSur();
        else if (!squaA.HaveSailor()) GuiManager.Instance.ShowPopupNotification("Lose because there are no Sailors", UIManager.Instance.reward.SendSurrenderToSever);
        else SendStartCombat();

           
    }

    public void SendStartCombat()
    {
        if (!squaA.HaveSailor()) GuiManager.Instance.ShowPopupNotification("Need at least one Sailor");
        else
        {
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutSFSArray("fgl", TeamCombatPrepareData.Instance.YourFightingLine.ToSFSArray());
            NetworkController.Send(SFSAction.PVE_CONFIRM, sfsObject);
            UIManager.Instance.reward.gameObject.SetActive(false);
        }
    }

    public void Surrender()
    {
        UIManager.Instance.reward.gameObject.SetActive(true);
        UIManager.Instance.reward.transform.GetChild(0).gameObject.SetActive(true);
    }
}
