using Sfs2X.Entities.Data;


public class PvPData : Singleton<PvPData>
{
    public DefenseCrewData DefenseCrew = new();
    public bool HaveJoin;
    public int Ticket;
    public int[] OpenedAdvantage;
    public int SelectingAdvantage;

    public void NewFromSFSObject(ISFSObject packet)
    {
        HaveJoin = packet.GetBool("have_join");
        Ticket = packet.GetInt("ticket");
        OpenedAdvantage = packet.GetIntArray("oppened_advantage");
        SelectingAdvantage = packet.GetInt("selecting_advantage");
        DefenseCrew.NewFromSFSObject(packet);
    }

}

