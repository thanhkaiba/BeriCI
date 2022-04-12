using System.Collections.Generic;

public class PvPConfig
{
    public int max_ticket = 5;
    public int start_elo = 1000;
    public int enemy_elo_around = 100;
    public int K = 32;
    public int find_opp_retry = 3;

    public List<HomefieldAdvantage> start_advantage;
    public int advantage_price = 500;
}
