using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class CellPvPRank : MonoBehaviour
    {
        [SerializeField]
        private UserAvatar avatar;
        [SerializeField]
        private Text textName;
        [SerializeField]
        private Text textRank;
        [SerializeField]
        private Text textElo;
        public void SetData(PopupTopArena.PVPRankPlayer player)
        {
            avatar.ShowAvatar(player.avatar);
            textName.text = player.name;
            textRank.text = "" + player.rank;
            textElo.text = "Elo: " + player.elo;
            var btn = gameObject.AddComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                var gui = GuiManager.Instance.AddGui("UserInfo/PopupUserInfo");
                gui.GetComponent<PopupUserInfo>().SetUID(player.uid);
            });
        }
    }
}

