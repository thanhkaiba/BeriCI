using DG.Tweening;
using Piratera.GUI;
using Piratera.Sound;
using Piratera.Utils;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class GuiReward : MonoBehaviour
    {
        [SerializeField]
        Text[] texts;
        [SerializeField]
        GameObject[] typeWin;
        [SerializeField]
        private Transform background;
        [SerializeField]
        private Image coin;
        [SerializeField]
        private SkeletonGraphic anim;
        [SerializeField]
        private ItemRewards beri, sailor_piece;

        protected void Start()
        {
            Appear();
        }
        public void SetReward(GameEndData r)
        {
            int total = r.mode_reward + r.hard_bonus + r.win_rank_bonus + r.team_bonus;
            beri.ShowBeri(total);
            ShowBonus(r.bonus);
            anim.initialSkinName = "";
            texts[0].text = r.mode_reward.ToString();
            texts[1].text = r.win_rank_bonus.ToString();
            texts[2].text = r.hard_bonus.ToString();
            texts[3].text = r.team_bonus.ToString();
            if (r.team_win == 0)
            {
                SoundMgr.PlaySound(PirateraSoundEffect.WIN);
                GameObject _typeWin = null;
                _typeWin = typeWin[(int)r.type];
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
                    seq.SetLink(image.gameObject).SetTarget(image.transform);
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
        private void ShowBonus(Rewards rewards)
        {
            Debug.Log("rewards.type: " + rewards.type);
            Debug.Log("rewards.type.Equals(PIECE_SAILOR): " + rewards.type.Equals("PIECE_SAILOR"));
            if (rewards.type.Equals("PIECE_SAILOR"))
            {
                sailor_piece.gameObject.SetActive(true);
                sailor_piece.ShowSailorPiece(rewards.meta, (int)rewards.amount);
                return;
            }
            sailor_piece.gameObject.SetActive(false);
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
            background.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.OutBack).SetLink(background.gameObject).SetTarget(background.transform);
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
            if (TempCombatData.Instance.modeID == ModeID.PvE)
            {
                LoadServerDataUI.NextScene = "SceneLobby";
            } else
            {
                LoadServerDataUI.NextScene = "SceneArena";
            }
            SceneManager.LoadScene("SceneLoadServerData");
        }

    }
}
