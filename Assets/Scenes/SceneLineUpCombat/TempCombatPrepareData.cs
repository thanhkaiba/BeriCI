using Sfs2X.Entities.Data;
using System.Collections.Generic;
using UnityEngine;

public class TeamCombatPrepareData : Singleton<TeamCombatPrepareData>
{
    public ModeID modeID;
    private byte yourTeamIndex;
    private string userName0;
    private string userName1;
    private int avt0;
    private int avt1;
    public int combatId;
    public List<SailorModel> sailors_0 = new List<SailorModel>();
    public List<SailorModel> sailors_1 = new List<SailorModel>();
    private FightingLine fgl0 = new FightingLine();
    private FightingLine fgl1 = new FightingLine();
    public byte countdown;

    public string YourName
    {
        get
        {
            if (yourTeamIndex == 0)
            {
                return userName0;
            }

            return userName1;
        }
    }

    public string OpponentName
    {
        get
        {
            if (yourTeamIndex == 0)
            {
                return userName1;
            }

            return userName0;
        }
    }

    public int YourAvatar
    {
        get
        {
            if (yourTeamIndex == 0)
            {
                return avt0;
            }

            return avt1;
        }
    }

    public int OpponentAvatar
    {
        get
        {
            if (yourTeamIndex == 0)
            {
                return avt1;
            }

            return avt0;
        }
    }

    public FightingLine YourFightingLine
    {
        get
        {
            if (yourTeamIndex == 0)
            {
                return fgl0;
            }

            return fgl1;
        }
    }

    public FightingLine OpponentFightingLine
    {
        get
        {
            if (yourTeamIndex == 0)
            {
                return fgl1;
            }

            return fgl0;
        }
    }


    private List<SailorModel> YourSailors
    {
        get
        {
            if (yourTeamIndex == 0)
            {
                return sailors_0;
            }

            return sailors_1;
        }
    }

    private List<SailorModel> OpponentSailors
    {
        get
        {
            if (yourTeamIndex == 0)
            {
                return sailors_1;
            }

            return sailors_0;
        }
    }

    private void Clean()
    {
        yourTeamIndex = 0;
        userName0 = "";
        userName1 = "";
        avt0 = 0;
        avt1 = 0;
        fgl0.Clean();
        fgl1.Clean();
        countdown = 0;
        sailors_0.Clear();
        sailors_1.Clear();
        countdown = 0;
    }

    public void NewFromSFSObject(ISFSObject packet)
    {
        Clean();
        modeID = (ModeID)packet.GetByte("mode_id");
        yourTeamIndex = packet.GetByte("your_team_idx");
        userName0 = packet.GetUtfString("username_0");
        userName1 = packet.GetUtfString("username_1");
        avt0 = packet.GetInt("avt_0");
        avt1 = packet.GetInt("avt_1");
        combatId = packet.GetInt("combat_id");
        ISFSArray sFSSailors = packet.GetSFSArray("sailors_0");
        foreach (ISFSObject obj in sFSSailors)
        {
            SailorModel model = new SailorModel(obj);
            sailors_0.Add(model);
        }
        sFSSailors = packet.GetSFSArray("sailors_1");
        foreach (ISFSObject obj in sFSSailors)
        {
            SailorModel model = new SailorModel(obj);
            sailors_1.Add(model);
        }

        fgl0.NewFromSFSObject(packet.GetSFSArray("fgl_0"));
        fgl1.NewFromSFSObject(packet.GetSFSArray("fgl_1"));

        fgl0.NumSlot = packet.GetInt("num_positions_0");
        fgl1.NumSlot = packet.GetInt("num_positions_1");

        countdown = packet.GetByte("countdown");
    }

    public SailorModel GetYourSailorModel(string id)
    {
        return YourSailors.Find(sailor => sailor.id == id);
    }

    public SailorModel GetOpponentSailorModel(string id)
    {
        return OpponentSailors.Find(sailor => sailor.id == id);
    }

    public void Swap(string sailorA, string sailorB)
    {
        if (YourFightingLine.Swap(sailorA, sailorB))
        {
            GameEvent.PrepareSquadChanged.Invoke();
        }

    }

    public void Occupie(string sailorId, short slot)
    {
        if (GetYourSailorModel(sailorId) == null)
        {
            return;
        }

        if (YourFightingLine.Occupie(sailorId, slot))
        {
            GameEvent.PrepareSquadChanged.Invoke();
        }

    }

    public void Replace(string subSailor, short slot)
    {
        if (GetYourSailorModel(subSailor) == null)
        {
            return;
        }

        if (YourFightingLine.Replace(subSailor, slot))
        {
            GameEvent.PrepareSquadChanged.Invoke();
        }
    }

    public void UnEquip(string sailorId)
    {
        if (GetYourSailorModel(sailorId) == null)
        {
            return;
        }

        if (YourFightingLine.UnEquip(sailorId))
        {
            GameEvent.PrepareSquadChanged.Invoke();
        }
    }
    public List<SailorModel> GetSquadModelList(bool yourTeam)
    {
        List<SailorModel> result = new List<SailorModel>();
        List<string> values = yourTeam ? fgl0.GetValues() : fgl1.GetValues();
        foreach (string val in values)
        {
            if (val != "")
            {
                result.Add(yourTeam ? GetYourSailorModel(val) : GetOpponentSailorModel(val));
            }
        }
        return result;
    }

    public List<SailorModel> GetSubstituteSailors()
    {
        List<SailorModel> result = new List<SailorModel>();
        bool isShowFav = PlayerPrefs.GetInt("is_show_fav", 0) == 1;
        foreach (SailorModel model in YourSailors)
        {
            if (!YourFightingLine.IsInSquad(model.id))
            {
                if (!isShowFav) result.Add(model);
                else if (GameUtils.IsSailorFavorite(model.id)) result.Add(model);
            }
        }
        result.Sort();
        result.Reverse();
        return result;
    }
}