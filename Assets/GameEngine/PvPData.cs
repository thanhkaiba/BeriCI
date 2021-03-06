using Piratera.Config;
using Sfs2X.Entities.Data;
using System.Collections.Generic;
using UnityEngine;
public enum HomefieldAdvantage
{
    SWEET_HOME,
    ELECTRONIC,
    ARMOR,
    CANNON,
    SPEED,
};
public class PvPData : Singleton<PvPData>
{
    public DefenseCrewData DefenseCrew = new DefenseCrewData();
    public bool HaveJoin;
    public int Ticket;
    public List<HomefieldAdvantage> OpenedAdvantage = new List<HomefieldAdvantage>();
    public HomefieldAdvantage SelectingAdvantage;
    public int Rank = 0;
    public long Elo = 0;
    public long StartSeason = 0;
    public long EndSeason = 0;
    public int SeasonId = 0;
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
        var adv_int = packet.GetIntArray("oppened_advantage");
        OpenedAdvantage = new List<HomefieldAdvantage>();
        Debug.Log("oppened_advantage Length: " + adv_int.Length);
        for (int i = 0; i < adv_int.Length; i++)
        {
            Debug.Log("(HomefieldAdvantage)adv_int[i]: " + (HomefieldAdvantage)adv_int[i]);
            OpenedAdvantage.Add((HomefieldAdvantage)adv_int[i]);
        }
        SelectingAdvantage = (HomefieldAdvantage)packet.GetInt("selecting_advantage");
        Debug.Log("SelectingAdvantage: " + SelectingAdvantage.ToString());
        Rank = packet.GetInt("rank");
        Elo = packet.GetLong("elo");

        StartSeason = packet.GetLong("start_season");
        EndSeason = packet.GetLong("end_season");
        SeasonId = packet.GetInt("season_id");
        Debug.Log("StartSeason: " + StartSeason);
        Debug.Log("EndSeason: " + EndSeason);
        Debug.Log("SeasonId: " + SeasonId);
        
        DefenseCrew.NewFromSFSObject(packet);
    }
    public bool HaveEnoughSailor()
    {
        return CrewData.Instance.GetNonTrialModelList().Count >= DefenseCrew.SailorLimit;
    }
    public Sprite GetAdvantageBackgroundSprite(HomefieldAdvantage ha)
    {
        Debug.Log("HomefieldAdvantage : " + ha);
        var src = "";
        switch(ha)
        {
            case HomefieldAdvantage.SWEET_HOME:
                src = "Background/battlefield/orange";
                break;
            case HomefieldAdvantage.ARMOR:
                src = "Background/battlefield/shield";
                break;
            case HomefieldAdvantage.SPEED:
                src = "Background/battlefield/sea";
                break;
            case HomefieldAdvantage.CANNON:
                src = "Background/battlefield/sand";
                break;
            case HomefieldAdvantage.ELECTRONIC:
                src = "Background/battlefield/lab";
                break;
            default:
                src = "Background/battlefield/shield";
                break;
        }
        return Resources.Load<Sprite>(src);
    }
    public Sprite GetAdvantageBackgroundSprite()
    {
        return GetAdvantageBackgroundSprite(SelectingAdvantage);
    }
    public int GetTicketCap()
    {
        return GlobalConfigs.PvP.max_ticket + GlobalConfigs.UpgradeShipConfig.GetTicketCapacity(UserData.Instance.HelmLevel);
    }
}

