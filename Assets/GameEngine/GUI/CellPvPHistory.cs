using DG.Tweening;
using Piratera.Network;
using Sfs2X.Entities.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class CellPvPHistory : MonoBehaviour
    {
        [SerializeField]
        private UserAvatar avatar;
        [SerializeField]
        private Text textName, textElo;
        [SerializeField]
        private Image iconPos, iconResult;
        [SerializeField]
        private Sprite iconAttack, iconDefense, iconWin, iconLose, iconDraw;

        PopupArenaHistory.PvPHistory data;

        public void SetData(PopupArenaHistory.PvPHistory history)
        {
            data = history;
            avatar.LoadAvatar(history.OpponentAvatar);
            textName.text = history.Opponent; ;
            textElo.text = (history.EloDelta > 0 ? "+" + history.EloDelta.ToString() : history.EloDelta.ToString()) + " elo";
            textElo.color = history.EloDelta < 0 ? new Color32(0, 132, 183, 255) : new Color32(123, 179, 57, 255);

            if (history.Position == "attacker") iconPos.sprite = iconAttack;
            else iconPos.sprite = iconDefense;

            if (history.Result == "Victory") iconResult.sprite = iconWin;
            else if (history.Result == "Draw") iconResult.sprite = iconDraw;
            else iconResult.sprite = iconLose;
        }
        public void OnReplay()
        {
            GuiManager.Instance.ShowGuiWaiting(true);
            SFSObject sfsObject = new();
            sfsObject.PutLong("match", data.matchId);
            NetworkController.Send(SFSAction.PVP_WATCH_HISTORY, sfsObject);
        }
    }
}

