using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPopupReward : MonoBehaviour
{
    [SerializeField]
    Text[] texts;

    [SerializeField]
    GameObject overPower;

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
                overPower.SetActive(true);
                break;
            case RankBonus.Overpower:
                overPower.SetActive(true);
                break;
            case RankBonus.Quell:
                overPower.SetActive(false);
                break;
            case RankBonus.Close:
                overPower.SetActive(false);
                break;
            default:
                break;
        }
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        overPower.SetActive(false);
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
