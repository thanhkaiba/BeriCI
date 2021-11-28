using DG.Tweening;
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

    private void Start()
    {
        UpdateUI();
    }

    public void OnJoinBattle()
    {
        SceneManager.LoadScene("SceneCombat2D");
    }

    public void OnSurrender()
    {
        SceneManager.LoadScene("SceneLobby");
    }

    public void UpdateUI()
    {
        TeamCombatPrepareData data = TeamCombatPrepareData.Instance;
        userNameA.text = data.YourName; 
        userNameB.text = data.OpponentName;

        avatarA.LoadAvatar(data.YourAvatar);
        avatarB.LoadAvatar(data.OpponentAvatar);
        RunCountDown(60);
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
                }
                else
                {
                    OnTimeOut();
                }
            })
            .SetLoops(countdown);
    }

    public void OnTimeOut()
    {
        SendStartCombat();
    }

    public void SendStartCombat()
    {
        SFSObject sfsObject = new SFSObject();
        sfsObject.PutBool("accept", true);
        sfsObject.PutSFSArray("fgl", TeamCombatPrepareData.Instance.YourFightingLine.ToSFSArray());
        NetworkController.Send(SFSAction.COMBAT_COMFIRM, sfsObject);
    }


    public void Surrender()
    {
        SFSObject sfsObject = new SFSObject();
        sfsObject.PutBool("accept", false);
        NetworkController.Send(SFSAction.COMBAT_COMFIRM, sfsObject);
    }
}
