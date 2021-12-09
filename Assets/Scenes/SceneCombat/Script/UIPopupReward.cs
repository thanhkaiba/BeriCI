using Sfs2X.Entities.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPopupReward : MonoBehaviour
{
    [SerializeField]
    Text[] texts;

    public void SetReward(byte yourTeamIndex, GameEndData r)
    {
        Debug.Log("yourTeamIndex " + yourTeamIndex);
        Debug.Log("r.team_win " + r.team_win);
        texts[0].text = r.team_win == yourTeamIndex ? "win" : "lose";
        texts[1].text = "x" + (r.mode_reward + r.hard_bonus + r.win_rank_bonus).ToString();
        texts[2].text = "win:   "+ r.mode_reward.ToString();
        texts[3].text = "overpower:   " + r.win_rank_bonus.ToString();
        texts[4].text = "hard:   " + r.hard_bonus.ToString();
        switch (r.type)
        {
            case RankBonus.Excellent:
                texts[5].text = "Excellent";
                break;
            case RankBonus.Overpower:
                texts[5].text = "Overpower";
                break;
            case RankBonus.Quell:
                texts[5].text = "Quell";
                break;
            case RankBonus.Close:
                texts[5].text = "Close";
                break;
            default:
                break;
        }
        texts[5].transform.parent.gameObject.SetActive(r.team_win == 1 ? false : true);
        transform.GetChild(1).gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        foreach (var item in texts)
        {
            item.text = "";
        }
        transform.GetChild(1).gameObject.SetActive(false);
        texts[5].transform.parent.gameObject.SetActive(false);
    }
    public void ClickReceive()
    {
        NetworkController.Instance.countDownPickTeam = true;
        SceneManager.LoadScene("SceneLobby");
        transform.GetChild(1).gameObject.SetActive(false);
    }
    public void ConfirmSur()
    {
       SFSObject sfsObject = new SFSObject();
       sfsObject.PutBool("accept", false);
       sfsObject.PutSFSArray("fgl", TeamCombatPrepareData.Instance.YourFightingLine.ToSFSArray());
       NetworkController.Send(SFSAction.COMBAT_COMFIRM, sfsObject);
       NetworkController.Instance.countDownPickTeam = false;
       transform.GetChild(1).gameObject.SetActive(true);
    }
}
