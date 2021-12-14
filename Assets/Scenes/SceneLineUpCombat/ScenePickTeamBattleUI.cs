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
    private void Start()
    {
        if (SoundMgr.Instance != null)
        {
            SoundMgr.PlayBGMusic(PirateraMusic.COMBAT);
        }
        UpdateUI();
    }

    public void OnJoinBattle()
    {
        //  SceneManager.LoadScene("SceneCombat2D");
    }

    public void OnSurrender()
    {
        // NetworkController.OnExtentionResponse();
    }

    public void UpdateUI()
    {
        TeamCombatPrepareData data = TeamCombatPrepareData.Instance;
        if (data.countdown > 0)
        {
            userNameA.text = data.YourName;
            userNameB.text = data.OpponentName;

            avatarA.LoadAvatar(data.YourAvatar);
            avatarB.LoadAvatar(data.OpponentAvatar);
            buttons[0].interactable = true;
            buttons[1].interactable = true;
            RunCountDown(data.countdown);
        }
    }

    public void RunCountDown(byte countdown)
    {
        textCountDown.text = "" + countdown;
        Sequence mySequence = DOTween.Sequence();
        mySequence.AppendInterval(1f)
            .AppendCallback(() =>
            {

                if (countdown > 0)
                {
                    countdown--;
                    textCountDown.text = "" + countdown;
                    if(countdown < 3)
                    {
                        buttons[0].interactable = false;
                        buttons[1].interactable = false;
                    }
                    if (countdown == 0) OnTimeOut();
                }

            })
            .SetLoops(countdown).SetLink(gameObject).SetTarget(transform);
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
