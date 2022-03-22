using Sfs2X.Entities.Data;
using UnityEngine;

public class PvPData : Singleton<PvPData>
{
    public DefenseCrewData DefenseCrew = new();
    public bool HaveJoin;
    public int Ticket;
    public int[] OpenedAdvantage;
    public int SelectingAdvantage;
    public int Rank = 0;
    public long Elo = 0;
    
    public enum PVP_TURORIAL_STEP {
        POPUP_DEFENSE_LINEUP = 2,
        POPUP_DEFENSE_LINEUP_DONE = 3,
        POPUP_WELCOME_ARENA = 1,
        POPUP_WELCOME_ARENA_DONE = 4,

    }
    
    public PVP_TURORIAL_STEP ShowedTutorial
    {
        get
        {
            return (PVP_TURORIAL_STEP)PlayerPrefs.GetInt("Showed-ARENA_TUT", 0);
        }
        
        set
        {
            PlayerPrefs.SetInt("Showed-ARENA_TUT", (int)value);
        }
    }

    public void NewFromSFSObject(ISFSObject packet)
    {
        HaveJoin = packet.GetBool("have_join");
        Ticket = packet.GetInt("ticket");
        OpenedAdvantage = packet.GetIntArray("oppened_advantage");
        SelectingAdvantage = packet.GetInt("selecting_advantage");
        Rank = packet.GetInt("rank");
        Elo = packet.GetLong("elo");

        DefenseCrew.NewFromSFSObject(packet);
    }

    public bool HaveEnoughSailor()
    {
        return CrewData.Instance.GetNonTrialModelList().Count >= DefenseCrew.SailorLimit;
    }

}

