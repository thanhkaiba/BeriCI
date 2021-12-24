using Sfs2X.Entities.Data;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Piratera.Sound;
using Piratera.Network;
using Piratera.GUI;
using Spine.Unity;

public class GuiReward : BaseGui
{
    [SerializeField]
    TextMeshProUGUI[] texts;
    [SerializeField]
    GameObject[] typeWin;
    [SerializeField]
    private Transform background;
    [SerializeField]
    private SkeletonGraphic anim;

    protected override void Start()
    {
     
        base.Start();
        Appear();
    }

    public void SetReward(GameEndData r)
    {
        anim.initialSkinName = "";
        texts[0].text = "x" + (r.mode_reward + r.hard_bonus + r.win_rank_bonus).ToString();
        texts[1].text = r.mode_reward.ToString();
        texts[2].text = r.win_rank_bonus.ToString();
        texts[3].text = r.hard_bonus.ToString();
        if (r.team_win == 0)
        {
            SoundMgr.PlaySound(PirateraSoundEffect.WIN);
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

            anim.initialSkinName = "win";
            anim.Initialize(true);
            ChangeStateAnim();
        }
        else if (r.team_win == 1)
        {
            SoundMgr.PlaySound(PirateraSoundEffect.LOSE);
            anim.initialSkinName = "lose";
            anim.Initialize(true);
            ChangeStateAnim();
        }
        else
        {
            SoundMgr.PlaySound(PirateraSoundEffect.DRAW);
            anim.initialSkinName = "draw";
            anim.Initialize(true);
            ChangeStateAnim();
        }

    }

    private void Appear()
    {
        Sequence s = DOTween.Sequence();
        var canvasGroup = background.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.DOFade(1, 0.2f);
        s.AppendCallback(() => canvasGroup.interactable = true);
        background.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        background.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.OutBack);
        var fog = GetComponent<HaveFog>();
        if (fog) fog.FadeIn(0.3f);
    }

    void ChangeStateAnim()
    {
        Sequence sq = DOTween.Sequence();
        sq.AppendInterval(1);
        sq.AppendCallback(() =>
        {
            anim.startingAnimation = "idle";
            anim.Initialize(true);
        });
    }
    public void ClickReceive()
    {
        SceneManager.LoadScene("SceneLobby");
    }
 
}
