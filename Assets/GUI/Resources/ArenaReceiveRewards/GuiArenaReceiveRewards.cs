using Piratera.Config;
using Sfs2X.Entities.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiArenaReceiveRewards : MonoBehaviour
{
    [SerializeField]
    private Transform nodeGift;
    [SerializeField]
    private GameObject itemAvt, itemSailorPiece, itemPosterPiece, itemBeri;
    public void ShowGift(ISFSArray data)
    {
        var config = GlobalConfigs.ArenaRewards.GetSeason(0);
        var length = data.Count;
        int row_num = (int)Mathf.Ceil(length / 5);
        int idx = 0;
        foreach (SFSObject obj in data)
        {
            var type = obj.GetUtfString("reward_type");
            var meta = obj.GetUtfString("reward_meta");
            var amount = (int)obj.GetLong("amount");

            GameObject item = null;
            if (type == "BERI")
            {
                item = Instantiate(itemBeri, nodeGift);
                item.GetComponent<ItemRewards>().ShowBeri(amount);
            }
            else if (type == "PIECE_SAILOR")
            {
                item = Instantiate(itemSailorPiece, nodeGift);
                item.GetComponent<ItemRewards>().ShowSailorPiece(meta, amount);
            }
            else if (type == "PIECE_POSTER")
            {
                item = Instantiate(itemPosterPiece, nodeGift);
                item.GetComponent<ItemRewards>().ShowPosterPiece(meta, amount);
            }
            else if (type == "AVATAR")
            {
                item = Instantiate(itemAvt, nodeGift);
                item.GetComponent<ItemRewards>().ShowAvatar(int.Parse(meta));
            }
            if (item != null)
            {
                int row = (int)Mathf.Ceil(idx / 5);
                int col = idx % 5;
                int number_in_row = Math.Min(5, length - row * 5);
                item.transform.localPosition = new Vector3((col - number_in_row / 2f + 0.5f) * 150, (-row + row_num / 2f - 0.5f) * 150, 0);
                var zoom = item.AddComponent<AppearZoom>();
                zoom.delayTime = idx * 0.3f;
                var fade = item.AddComponent<AppearFade>();
                fade.delayTime = idx * 0.3f;
            }
            idx++;
        }
    }
    public void ClickReceive()
    {
        Close();
    }
    public void Close()
    {
        Destroy(gameObject);
    }
}
