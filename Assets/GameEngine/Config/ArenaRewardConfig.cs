using System;
using System.Collections.Generic;
using UnityEngine;
public class ArenaRewardsConfig
{
    public List<Season> list = new List<Season>();

    public class Season
    {
        public int season_index;
        public Rewards[] top_rewards;
        public string[] general_rewards;
    }
    public class Rewards
    {
        public int from;
        public int to;
        public string[] list;
    }
    public Season GetSeason(int seasonId)
    {
        return list.Find(i => {
            return i.season_index == seasonId;
        });
    }
}

