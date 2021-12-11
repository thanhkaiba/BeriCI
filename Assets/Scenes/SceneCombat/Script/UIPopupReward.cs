using Sfs2X.Entities.Data;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

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
            GameObject _typeWin = null;
            switch (r.type)
            {
                case RankBonus.Excellent:
                    _typeWin = typeWin[0];
                    break;
                case RankBonus.Overpower:
                    _typeWin = typeWin[1];
                    break;
                case RankBonus.Quell:
                    _typeWin = typeWin[2];
                    break;
                case RankBonus.Close:
                    _typeWin = typeWin[3];
                    break;
                default:
                    break;
            }
            if (_typeWin != null)
            {
                _typeWin.SetActive(true);
                _typeWin.transform.localScale = new Vector3(3, 3, 3);
                var image = _typeWin.GetComponent<Image>();
                var color = image.color;
                color.a = 0;
                image.color = color;
                Sequence seq = DOTween.Sequence();
                seq.Insert(0.5f, _typeWin.transform.DOScale(1, 0.6f).SetEase(Ease.InCirc));
                seq.Insert(0.5f, image.DOFade(1, 0.3f).SetEase(Ease.InSine));
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
        SceneManager.LoadScene("SceneLobby");
    }
    public void ConfirmSur()
    {
       SFSObject sfsObject = new SFSObject();
       sfsObject.PutBool("accept", false);
       NetworkController.Send(SFSAction.PVE_SURRENDER, sfsObject);
    }
}
