using Sfs2X.Entities.Data;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIPopupReward : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI[] texts;
    [SerializeField]
    GameObject[] results, typeWin, backLight;
    public void SetReward(byte yourTeamIndex, GameEndData r)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        texts[0].text = "x" + (r.mode_reward + r.hard_bonus + r.win_rank_bonus).ToString();
        texts[1].text = r.mode_reward.ToString();
        texts[2].text = r.win_rank_bonus.ToString();
        texts[3].text = r.hard_bonus.ToString();
        if (r.team_win == 0)
        {
            switch (r.type)
            {
                case RankBonus.Excellent:
                    typeWin[0].SetActive(true);
                    typeWin[0].transform.DOPunchScale(new Vector3(1.5f, 1.5f), 1);
                    break;
                case RankBonus.Overpower:
                    typeWin[1].SetActive(true);
                    typeWin[1].transform.DOPunchScale(new Vector3(1.5f, 1.5f), 1);
                    break;
                case RankBonus.Quell:
                    typeWin[2].SetActive(true);
                    typeWin[2].transform.DOPunchScale(new Vector3(1.5f, 1.5f), 1);
                    break;
                case RankBonus.Close:
                    typeWin[3].SetActive(true);
                    typeWin[3].transform.DOPunchScale(new Vector3(1.5f, 1.5f), 1);
                    break;
                default:
                    break;
            }
            results[0].SetActive(true);
            backLight[0].SetActive(true);
        }
        else if (r.team_win == 1)
        {
            results[1].SetActive(true);
        }
        else results[2].SetActive(true);
      
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
        transform.GetChild(0).gameObject.SetActive(false);
        foreach (var item in results)
        {
            item.SetActive(false);
        }
        foreach (var item in typeWin)
        {
            item.SetActive(false);
        }
        foreach (var item in backLight)
        {
            item.SetActive(false);
        }
    }
    public void SetRewardSurrender()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        texts[0].text = "x" + TempCombatData.Instance.beri;
        texts[1].text = "0";
        texts[2].text = "0";
        texts[3].text = "0";
        results[1].SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
    }
    public void ClickReceive()
    {
        NetworkController.Instance.countDownPickTeam = true;
        SceneManager.LoadScene("SceneLobby");
    }
    public void ConfirmSur()
    {
       SFSObject sfsObject = new SFSObject();
       sfsObject.PutBool("accept", false);
       NetworkController.Send(SFSAction.PVE_SURRENDER, sfsObject);
       NetworkController.Instance.countDownPickTeam = false;

    }
}
