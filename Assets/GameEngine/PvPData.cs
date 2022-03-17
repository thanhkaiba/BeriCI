using Sfs2X.Entities.Data;


public class PvPData : Singleton<PvPData>
{
    public DefenseCrewData DefenseCrew = new();
    public bool HaveJoin;
    public int Ticket;
    public int[] OpenedAdvantage;
    public int SelectingAdvantage;
    public int Rank = 0;
    public long Elo = 0;

    public void NewFromSFSObject(ISFSObject packet)
    {
        HaveJoin = packet.GetBool("have_join");
        Ticket = packet.GetInt("ticket");
        OpenedAdvantage = packet.GetIntArray("oppened_advantage");
        SelectingAdvantage = packet.GetInt("selecting_advantage");
        try
        {
            Rank = packet.GetInt("rank");
            Elo = packet.GetLong("elo");
        } catch
        {

        }
       
        DefenseCrew.NewFromSFSObject(packet);
    }

}

