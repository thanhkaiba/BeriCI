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
            ShowListRewards(data.list, cell.transform);
        }
        {
            var cell = Instantiate(cellTop, content);
            ShowListRewards(config.general_rewards, cell.transform);
            var textPosition = cell.transform.FindDeepChild("textRank").GetComponent<Text>();
            cell.transform.FindDeepChild("top").gameObject.SetActive(false);
            textPosition.text = "...";
            textPosition.fontSize = 42;
        }
    }
    public void ShowListRewards(string[] list, Transform cell)
    {
        for (int j = 0; j < list.Length; j++)
        {
            var stringReward = list[j];
            if (stringReward.StartsWith("beri"))
            {
                var split = stringReward.Split(":");
                var item = Instantiate(itemBeri, cell);
                item.GetComponent<ItemRewards>().ShowBeri(int.Parse(split[1]));
            }
            else if (stringReward.StartsWith("piece_sailor"))
            {
                var split = stringReward.Split(":");
                var item = Instantiate(itemSailorPiece, cell);
                item.GetComponent<ItemRewards>().ShowSailorPiece(split[1], int.Parse(split[2]));
            }
            else if (stringReward.StartsWith("piece_poster"))
            {
                var split = stringReward.Split(":");
                var item = Instantiate(itemPosterPiece, cell);
                item.GetComponent<ItemRewards>().ShowPosterPiece(split[1], int.Parse(split[2]));
            }
            else if (stringReward.StartsWith("ava"))
            {
                var split = stringReward.Split(":");
                var item = Instantiate(itemAvt, cell);
                item.GetComponent<ItemRewards>().ShowAvatar(int.Parse(split[1]));
            }
        }
    }
    public void Close()
    {
        Destroy(gameObject);
    }
}
