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
    public class GuiRewardPvP : MonoBehaviour
    {
        [SerializeField]
        Text textElo;
        [SerializeField]
        private Transform background;
        [SerializeField]
        private Image coin;
        [SerializeField]
        private SkeletonGraphic anim;
        [SerializeField]
        private ItemRewards sailor_piece, beri;
        [SerializeField]
        private GameObject panel_gift;

        public bool isWatchReplay = false;

        protected void Start()
        {
            Appear();
        }

        public void SetReward(GameEndPvPData r)
        {
            textElo.text = "";
            if (r.elo_delta >= 0)
                textElo.color = new Color32(10, 218, 255, 255);
            else
                textElo.color = new Color32(255, 66, 142, 255);
            var canvas = GuiManager.Instance.GetCanvas().GetComponent<Canvas>();
            Sequence seqe = DOTween.Sequence();
            coin.transform.localScale = Vector3.zero;
            seqe.AppendInterval(0.2f);
            seqe.Join(coin.DOFade(1, 0.2f));
            seqe.Join(coin.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));
            seqe.AppendCallback(() =>
            {
                DoTweenUtils.UpdateNumber(textElo, 0, r.elo_delta, x => ((x > 0 ? "+" + x : "" + x) + " ELO"));
            });
            seqe.SetLink(coin.gameObject).SetTarget(coin.transform);
            anim.initialSkinName = "";
            ShowBonus(r.bonus, r.team_bonus);

            if (r.team_win == 0)
            {
                SoundMgr.PlaySound(PirateraSoundEffect.WIN);
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
            if (isWatchReplay)
            {
                SceneTransition.Instance.LoadScene("SceneArena", TransitionType.ARENA);
                return;
            }
            if (TempCombatData.Instance.modeID == ModeID.PvE)
            {
                LoadServerDataUI.NextScene = "SceneLobby";
            } else
            {
                LoadServerDataUI.NextScene = "SceneArena";
            }
            SceneManager.LoadScene("SceneLoadServerData");
        }
        private void ShowBonus(Rewards rewards, int beriQuantity)
        {
            bool no_reward = true;
            panel_gift.SetActive(true);
            sailor_piece.gameObject.SetActive(false);
            beri.gameObject.SetActive(false);
            if (rewards.type.Equals("PIECE_SAILOR"))
            {
                sailor_piece.gameObject.SetActive(true);
                sailor_piece.ShowSailorPiece(rewards.meta, (int)rewards.amount);
                no_reward = false;
            }
            if (beriQuantity > 0)
            {
                beri.gameObject.SetActive(true);
                beri.ShowBeri(beriQuantity, "team bonus");
                no_reward = false;
            }
            if (no_reward)
            {
                panel_gift.SetActive(false);
                textElo.transform.position -= new Vector3(0, textElo.rectTransform.rect.height / 4, 0);
            }
        }
    }
}
