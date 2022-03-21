using DG.Tweening;
using Sfs2X.Entities.Data;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Piratera.GUI
{
    
    public class PopupTopArena : BaseGui
    {
        public class PVPRankPlayer
        {
            public string name;
            public int rank;
            public int elo;
            public string avatar;

            public PVPRankPlayer(ISFSObject packet)
            {
                NewFromSFSObject(packet);
            }

            public void NewFromSFSObject(ISFSObject packet)
            {
                name = packet.GetUtfString("name");
                rank = packet.GetInt("rank");
                elo = (int)packet.GetLong("elo");
                avatar = packet.GetUtfString("avatar");
                
            }
        }

        List<PVPRankPlayer> Players = new();
        [SerializeField] RectTransform content;
        [SerializeField] GameObject cellPrefap;

        [SerializeField]
        private Transform background;
        protected override void Start()
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
                Players.Add(new PVPRankPlayer(obj));
            }
            Players = Players.OrderBy(x => x.rank).ToList();
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
            background.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.OutBack).SetLink(background.gameObject).SetTarget(background.transform);

            var fog = GetComponent<HaveFog>();
            if (fog) fog.FadeIn(0.3f);
        }
        private void ClosePopup()
        {
            background.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.1f).SetEase(Ease.OutSine);
            var canvasGroup = background.GetComponent<CanvasGroup>();
            Sequence s = DOTween.Sequence();
            s.Append(canvasGroup.DOFade(0, 0.1f));
            s.AppendCallback(DestroySelf);

            var fog = GetComponent<HaveFog>();
            if (fog) fog.FadeOut(0.1f);
        }

        protected void RenderCells()
        {
           
            for (int i = 0; i < Players.Count; i++)
            {
                GameObject imgObject = Instantiate(cellPrefap, content);
                CellPvPRank cell = imgObject.GetComponent<CellPvPRank>();
                cell.SetData(Players[i]);
                
            }
        }
    }
}

