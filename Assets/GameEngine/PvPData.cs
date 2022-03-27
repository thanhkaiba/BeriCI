using Sfs2X.Entities.Data;
using System.Collections.Generic;
using UnityEngine;
public enum HomefieldAdvantage
{
    SWEET_HOME,
    ELECTRONIC,
    ARMOR,
    CANNON,
    DESERT,
    SPEED,
};
public class PvPData : Singleton<PvPData>
{
    public DefenseCrewData DefenseCrew = new DefenseCrewData();
    public bool HaveJoin;
    public int Ticket;
    public List<HomefieldAdvantage> OpenedAdvantage;
    public HomefieldAdvantage SelectingAdvantage;
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
        var adv_int = packet.GetIntArray(" ");
        //OpenedAdvantage = new List<HomefieldAdvantage>();
        //for (int i = 0; i < adv_int.Length; i++)
        //{
        //    OpenedAdvantage.Add((HomefieldAdvantage)adv_int[i]);
        //}
        // SelectingAdvantage = (HomefieldAdvantage) packet.GetInt("selecting_advantage");
        SelectingAdvantage = HomefieldAdvantage.DESERT;
        Rank = packet.GetInt("rank");
        Elo = packet.GetLong("elo");

        DefenseCrew.NewFromSFSObject(packet);
    }
    public bool HaveEnoughSailor()
    {
        return CrewData.Instance.GetNonTrialModelList().Count >= DefenseCrew.SailorLimit;
    }
    public Sprite GetAdvantageBackgroundSprite(HomefieldAdvantage ha)
    {
        var src = "";
        switch(ha)
        {
            case HomefieldAdvantage.ARMOR:
                src = "Background/battlefield/shield";
                break;
            case HomefieldAdvantage.SPEED:
                src = "Background/battlefield/sea";
                break;
            case HomefieldAdvantage.DESERT:
                src = "Background/battlefield/sand";
                break;
            default:
                src = "Background/line_up";
                break;
        }
        return Resources.Load<Sprite>(src);
    }
    public Sprite GetAdvantageBackgroundSprite()
    {
        return GetAdvantageBackgroundSprite(SelectingAdvantage);
    }
}

