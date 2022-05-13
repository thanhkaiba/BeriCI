using System.Collections.Generic;

public class UpgradeShipConf
{
    public List<int> sail_cost;
    public List<int> sail_stamina_capacity;
    public List<int> helm_cost;
    public List<int> helm_pvp_ticket_capacity;

    public int GetMaxLevel()
    {
        return sail_stamina_capacity.Count - 1;
    }
    public int GetSailNextLevelPrice(int level)
    {
        return sail_cost[level];
    }
    public int GetStaminaCapacity(int level)
    {
        return sail_stamina_capacity[level];
    }
    // helm
    public int GetHelmMaxLevel()
    {
        return helm_pvp_ticket_capacity.Count - 1;
    }
    public int GetHelmNextLevelPrice(int level)
    {
        return helm_cost[level];
    }
    public int GetTicketCapacity(int level)
    {
        return helm_pvp_ticket_capacity[level];
    }
}
