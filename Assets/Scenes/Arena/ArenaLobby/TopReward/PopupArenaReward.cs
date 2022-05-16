using Piratera.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupArenaReward : MonoBehaviour
{
    [SerializeField]
    private GameObject cellTop, itemAvt, itemSailorPiece, itemPosterPiece, itemBeri;
    [SerializeField]
    private Transform content;
    [SerializeField]
    private Text title;
    private void Start()
    {
        int ssId = PvPData.Instance.SeasonId;
        title.text = $"Season {ssId} - rewards";
        var config = GlobalConfigs.ArenaRewards.GetSeason(ssId);
        for (int i = 0; i < config.top_rewards.Length; i++)
        {
            var data = config.top_rewards[i];
            var cell = Instantiate(cellTop, content);
            var textPosition = cell.transform.FindDeepChild("textRank").GetComponent<Text>();
            if (data.from == data.to)
                textPosition.text = "" + data.from;
            else
            {
                textPosition.text = data.from + "-" + data.to;
                textPosition.fontSize = 52;
                if (i > 4) textPosition.fontSize = 48;
                else if (i > 5) textPosition.fontSize = 42;
            }
            var slot_top = cell.transform.FindDeepChild("slot_top").GetComponent<RawImage>();
            var slot_gift = cell.transform.FindDeepChild("slot_gift").GetComponent<Image>();
            if (PvPData.Instance.Rank >= data.from && PvPData.Instance.Rank <= data.to)
            {
                slot_top.color = new Color32(255, 255, 0, 255);
                slot_gift.color = new Color32(255, 255, 0, 255);
            }
            else
            {
                slot_top.color = new Color32(255, 255, 255, 255);
                slot_gift.color = new Color32(255, 255, 255, 255);
            }

            for (int j = 0; j < data.list.Length; j++)
            {
                var stringReward = data.list[j];
                if (stringReward.StartsWith("beri"))
                {
                    var split = stringReward.Split(":");
                    var item = Instantiate(itemBeri, cell.transform);
                    item.GetComponent<ItemRewards>().ShowBeri(int.Parse(split[1]));
                }
                else if (stringReward.StartsWith("piece_sailor"))
                {
                    var split = stringReward.Split(":");
                    var item = Instantiate(itemSailorPiece, cell.transform);
                    item.GetComponent<ItemRewards>().ShowSailorPiece(split[1], int.Parse(split[2]));
                }
                else if (stringReward.StartsWith("piece_poster"))
                {
                    var split = stringReward.Split(":");
                    var item = Instantiate(itemPosterPiece, cell.transform);
                    item.GetComponent<ItemRewards>().ShowPosterPiece(split[1], int.Parse(split[2]));
                }
                else if (stringReward.StartsWith("ava"))
                {
                    var split = stringReward.Split(":");
                    var item = Instantiate(itemAvt, cell.transform);
                    item.GetComponent<ItemRewards>().ShowAvatar(int.Parse(split[1]));
                }
                //ItemRewards
            }
        }
    }
    public void Close()
    {
        Destroy(gameObject);
    }
}
