using DG.Tweening;
using Sfs2X.Entities.Data;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Piratera.GUI
{
    public class PopupArenaHistory : MonoBehaviour
    {
        public class PvPHistory
        {
            public long matchId;
            public string attacker;
            public string defender;
            public int attackerEloDelta;
            public int defenderEloDelta;
            public long timestamp;
            public string winner = "";
            public int attackerAvatar;
            public int defenderAvatar;
            public string attackerUsername;
            public string defenderUsername;
            
            public string Position
            {
                get { return attacker == UserData.Instance.UID ? "attacker" : "defender"; }
            }
            public string OpponentUID
            {
                get { return Position == "attacker" ? defender : attacker; }
            }
            public int OpponentAvatar
            {
                get { return Position == "attacker" ? defenderAvatar : attackerAvatar; }
            }
            public string OpponentUsername
            {
                get { return Position == "attacker" ? defenderUsername : attackerUsername; }
            }
            public int EloDelta { get { return Position == "attacker" ? attackerEloDelta : defenderEloDelta; } }
            public string Opponent
            {
                get { return Position == "attacker" ? defender : attacker; }
            }
            public string Result { get
                {
                    if (winner == UserData.Instance.UID)
                    {
                        return "Victory";
                    }
                    else if (winner == "")
                    {
                        return "Draw";
                    }
                    else
                    {
                        return "Defeat";
                    }
                } 
            }
            public PvPHistory(ISFSObject packet)
            {
                NewFromSFSObject(packet);
            }

            public void NewFromSFSObject(ISFSObject packet)
            {
                matchId = packet.GetLong("match");
                attacker = packet.GetUtfString("attacker");
                defender = packet.GetUtfString("defender");
                attackerEloDelta = packet.GetInt("attacker_elo_delta");
                defenderEloDelta = packet.GetInt("defender_elo_delta");
                timestamp = packet.GetLong("timestamp");
                winner = packet.GetUtfString("winner");
                attackerAvatar = packet.GetInt("attacker_avatar");
                defenderAvatar = packet.GetInt("defender_avatar");
                attackerUsername = packet.GetUtfString("attacker_nickname");
                defenderUsername = packet.GetUtfString("defender_nickname");
            }
        }

        List<PvPHistory> Historys = new();
        [SerializeField] RectTransform content;
        [SerializeField] GameObject cellPrefap;

        [SerializeField]
        private Transform background;
        protected void Start()
        {
            Appear();
        }
        public void OnOK()
        {
            ClosePopup();
        }
        public void NewFromSFSObject(ISFSArray packet)
        {
            foreach (SFSObject obj in packet)
            {
                Historys.Add(new PvPHistory(obj));
            }

            RenderCells();
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
            background.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.3f).SetEase(Ease.OutBack).SetLink(background.gameObject).SetTarget(background.transform);

            var fog = GetComponent<HaveFog>();
            if (fog) fog.FadeIn(0.3f);
        }
        private void ClosePopup()
        {
            background.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.1f).SetEase(Ease.OutSine);
            var canvasGroup = background.GetComponent<CanvasGroup>();
            Sequence s = DOTween.Sequence();
            s.Append(canvasGroup.DOFade(0, 0.1f));
            s.AppendCallback(() => Destroy(gameObject));

            var fog = GetComponent<HaveFog>();
            if (fog) fog.FadeOut(0.1f);
        }

        protected void RenderCells()
        {
           
            for (int i = 0; i < Historys.Count; i++)
            {
                GameObject imgObject = Instantiate(cellPrefap, content);
                CellPvPHistory cell = imgObject.GetComponent<CellPvPHistory>();
                cell.SetData(Historys[i]);
                
            }
        }
    }
}

