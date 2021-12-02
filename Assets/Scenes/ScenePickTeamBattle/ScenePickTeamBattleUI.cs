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
    [SerializeField]
    SquadAContainer squaA;
    [Header("UserB Info")]
    [SerializeField]
    private Text userNameB;
    [SerializeField]
    private UserAvatar avatarB;
    private bool countDown = true;

    private void Start()
    {
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
                if (!countDown)
                    return;
                if (countdown > 0)
                {
                    countdown--;
                    textCountDown.text = "" + countdown;
                    if(countdown == 0) OnTimeOut();
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
        Debug.Log(NetworkController.showCombat);
        SFSObject sfsObject = new SFSObject();
        sfsObject.PutBool("accept", true);
        sfsObject.PutSFSArray("fgl", TeamCombatPrepareData.Instance.YourFightingLine.ToSFSArray());
        NetworkController.showCombat = squaA.HaveSailor();
        NetworkController.Send(SFSAction.COMBAT_COMFIRM, sfsObject);
        countDown = false;
    }


    public void Surrender()
    {
        
        SFSObject sfsObject = new SFSObject();
        sfsObject.PutBool("accept", false);
        sfsObject.PutSFSArray("fgl", TeamCombatPrepareData.Instance.YourFightingLine.ToSFSArray());
        NetworkController.showCombat = false;
        NetworkController.Send(SFSAction.COMBAT_COMFIRM, sfsObject);
        countDown = false;
    }
}
