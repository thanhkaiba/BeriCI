using System.Collections.Generic;

public class PvEConfig
{
    public int stamina_cost = 3;
    public int preparing_time = 60;
    public int lose_reward = 10;
    public int win_reward = 30;
    public List<int> excellent_bonus;
    public List<int> excellent_health_remain;
    public float hard_bonus_ratio = 0.2f; // beri = (int) rank/5;

    public List<float> excellent_rank_inc;
    public List<int> sailor_rank_bonus;
    public float lose_rank_dec;
    public float min_rank = 0f;
    public float max_rank = 100f;

    public float rank_bot_around = 5f;

    public int sailor_exp_win = 150;
    public int sailor_exp_lose = 100;

    public int default_exp = 20;
    public float hard_exp_ratio = 1;
    public float win_exp_ratio = 1;
    public float lose_exp_ratio = 0.5f;
}
