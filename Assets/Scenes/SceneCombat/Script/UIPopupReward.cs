using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPopupReward : MonoBehaviour
{
    [SerializeField]
    Text[] texts;

    public void SetReward(CombatReward r)
    {
        texts[0].text = r.team_win == 1 ? "lose" : "win";
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
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        foreach (var item in texts)
        {
            item.text = "";
        }
    }
    public void ClickReceive()
    {
        SceneManager.LoadScene("SceneLobby");
    }
}
