﻿using DG.Tweening;
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
        private Text textName;
        [SerializeField]
        private Text textElo;

        [SerializeField]
        private Text textResult;

        [SerializeField]
        private Text textPos;

        PopupArenaHistory.PvPHistory data;

        public void SetData(PopupArenaHistory.PvPHistory history)
        {
            data = history;
            avatar.LoadAvatar(history.OpponentAvatar);
            textName.text = history.Opponent; ;
            textElo.text = history.EloDelta.ToString();
            textElo.color = history.EloDelta < 0 ? Color.red : Color.green;
            textPos.text = history.Position.ToString();
            
            
            textPos.color = history.Position == "attacker" ? Color.gray : Color.blue;

            textResult.text = history.Result;
            textResult.color = history.Result == "Victory" ? Color.green : history.Result == "Draw" ? Color.magenta : Color.red;
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
