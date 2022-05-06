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
    private void Start()
    {
        var config = GlobalConfigs.ArenaRewards.GetSeason(0);
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
                    var split1 = stringReward.Split(":");
                    var split2 = split1[0].Split("_");
                    var item = Instantiate(itemSailorPiece, cell.transform);
                    item.GetComponent<ItemRewards>().ShowSailorPiece(split2[2], int.Parse(split1[1]));
                }
                else if (stringReward.StartsWith("piece_poster"))
                {
                    var split1 = stringReward.Split(":");
                    var split2 = split1[0].Split("_");
                    var item = Instantiate(itemPosterPiece, cell.transform);
                    item.GetComponent<ItemRewards>().ShowPosterPiece(split2[2], int.Parse(split1[1]));
                }
                else if (stringReward.StartsWith("ava"))
                {
                    var split = stringReward.Split("_");
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
