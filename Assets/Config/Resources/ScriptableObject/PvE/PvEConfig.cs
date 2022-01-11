using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PvE config", menuName = "config/PvE")]
public class PvEConfig : ScriptableObjectPro
{
    public int stamina_cost = 3;
    public int preparing_time = 60;
    public int lose_reward = 10;
    public int win_reward = 30;
    public List<int> excellent_bonus;
    public List<int> excellent_health_remain;
    public float hard_bonus_ratio = 0.2f; // beri = (int) rank/5;

    public List<float> excellent_rank_inc;
    public float lose_rank_dec;
    public float min_rank = 0f;
    public float max_rank = 100f;

    public float rank_bot_around = 5f;
}
