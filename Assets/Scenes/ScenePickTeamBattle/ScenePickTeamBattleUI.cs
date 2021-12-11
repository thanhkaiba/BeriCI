using DG.Tweening;
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
                    if(countdown == 0) OnTimeOut();
                }
              
            })
            .SetLoops(countdown).SetLink(gameObject).SetTarget(transform);
    }

    public void OnTimeOut()
    {
        //NetworkController.Send(SFSAction.PVE_SURRENDER);
        SendStartCombat();
    }

    public void SendStartCombat()
    {
        SFSObject sfsObject = new SFSObject();
        sfsObject.PutSFSArray("fgl", TeamCombatPrepareData.Instance.YourFightingLine.ToSFSArray());
        NetworkController.Send(SFSAction.PVE_CONFIRM, sfsObject);
    }


    public void Surrender()
    {
        UIManager.Instance.reward.gameObject.SetActive(true);
        UIManager.Instance.reward.transform.GetChild(0).gameObject.SetActive(true);
    }
}
