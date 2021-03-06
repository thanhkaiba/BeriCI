using DG.Tweening;
using Sfs2X.Entities.Data;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class PopupTopArena : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> slotTop;
        [SerializeField]
        private GameObject popupRewards;
        public class PVPRankPlayer
        {
            public string uid;
            public string name;
            public int rank;
            public long elo;
            public int avatar;
            public PVPRankPlayer(ISFSObject packet)
            {
                NewFromSFSObject(packet);
            }
            public void NewFromSFSObject(ISFSObject packet)
            {
                uid = packet.GetUtfString("uid");
                name = packet.GetUtfString("name");
                rank = packet.GetInt("rank");
                elo = packet.GetLong("elo");
                avatar = packet.GetInt("avatar");
            }
        }

        List<PVPRankPlayer> Players = new();
        [SerializeField] Transform content;
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
            background.DOScale(Vector3.one * 1.2f, 0.3f).SetEase(Ease.OutBack).SetLink(background.gameObject).SetTarget(background.transform);

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
            for (int j = 0; j < slotTop.Count; j++)
            {
                Debug.Log("Players.Count: " + Players.Count);
                var slot = slotTop[j];
                if (Players.Count < j + 1)
                {
                    slot.gameObject.SetActive(false);
                    continue;
                }
                var player = Players[j];
                slot.gameObject.SetActive(true);
                slot.Find("textName").GetComponent<Text>().text = player.name;
                slot.Find("textElo").GetComponent<Text>().text = "Elo: " + player.elo;
                slot.Find("avt").GetComponent<UserAvatar>().ShowAvatar(player.avatar);
                Debug.Log("Players[j].avatar: " + player.avatar);
                slot.gameObject.AddComponent<Button>().onClick.AddListener(() =>
                {
                    var gui = GuiManager.Instance.AddGui("UserInfo/PopupUserInfo");
                    gui.GetComponent<PopupUserInfo>().SetUID(player.uid);
                });
            }
            for (int i = 3; i < Players.Count; i++)
            {
                GameObject imgObject = Instantiate(cellPrefap, content);
                CellPvPRank cell = imgObject.GetComponent<CellPvPRank>();
                cell.SetData(Players[i]);
            }
        }
        public void ShowPopupRewards()
        {
            GuiManager.Instance.AddGui(popupRewards);
        }
    }
}

