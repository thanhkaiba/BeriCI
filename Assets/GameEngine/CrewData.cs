using Sfs2X.Entities.Data;
using System.Collections.Generic;

public class CrewData : Singleton<CrewData>
{
    public List<SailorModel> Sailors = new List<SailorModel>();
    public FightingLine FightingTeam = new FightingLine();


    protected override void OnAwake()
    {
        ResetData();
        GameEvent.SquadChange.AddListener(OnUpdateSquad);
    }

    private void OnUpdateSquad()
    {
        SmartFoxConnection.Send(SFSAction.TEAM_COMMIT, FightingTeam.ToSFSObject());
    }



    private void ResetData()
    {
        Sailors.Clear();
        FightingTeam.Clean();
        
    }


    public void NewFromSFSObject(ISFSObject packet)
    {
        ResetData();
        ISFSArray sFSArray = packet.GetSFSArray("sailors");
        foreach (ISFSObject obj in sFSArray)
        {
            SailorModel model = new SailorModel(obj.GetUtfString("id"), obj.GetUtfString("name"))
            { quality = obj.GetInt("quality"), level = obj.GetInt("level"), exp = obj.GetInt("exp") };
            Sailors.Add(model);
        }

        FightingTeam.NewFromSFSObject(packet.GetSFSArray("fighting_lines"));
    }


    public SailorModel GetSailorModel(string id)
    {
        return Sailors.Find(s => s.id == id);
    }

    public void Swap(string sailorA, string sailorB)
    {
        if (FightingTeam.Swap(sailorA, sailorB))
        {
            GameEvent.SquadChange.Invoke();
        }

    }

    public void Occupie(string sailorId,short slot)
    {
        if (GetSailorModel(sailorId) == null)
        {
            return;
        }

        if (FightingTeam.Occupie(sailorId, slot))
        {
            GameEvent.SquadChange.Invoke();
        }

    }

    public void Replace(string subSailor, short slot)
    {
        if (GetSailorModel(subSailor) == null)
        {
            return;
        }
        
        if (FightingTeam.Replace(subSailor, slot))
        {
            GameEvent.SquadChange.Invoke();
        }
    }


    
    public List<SailorModel> GetSquadModelList()
    {
        List<SailorModel> result = new List<SailorModel>();
        List<string> values = FightingTeam.GetValues();
        foreach (string val in values)
        {
           if (val != "")
            {
                result.Add(GetSailorModel(val));
            }
        }
        return result;
    }

    public List<SailorModel> GetSubstituteSailors()
    {
        List<SailorModel> result = new List<SailorModel>();
        foreach (SailorModel model in Sailors)
        {
            if (!FightingTeam.IsInSquad(model.id))
            {
                result.Add(model);
            }
        }
        return result;
    }

    /// <summary>
    /// Get a sailor in squad by a combatPosition {x, y}
    /// </summary>
    /// <param name="combatPosition"></param>
    /// <returns>return null if not found</returns>
    public SailorModel SailorAt(CombatPosition combatPosition)
    {  
        return GetSailorModel(FightingTeam.SailorIdAt(combatPosition));
    }

   

 
}
